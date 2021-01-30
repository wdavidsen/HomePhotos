using MetadataExtractor.Formats.Exif;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Workers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service
{
    public class ImageService : IImageService
    {
        private Random _randomNum = new Random();

        private readonly IImageTransformer _imageTransformer;
        private readonly IFileSystemService _fileSystemService;
        private readonly IPhotoService _photoService;
        private readonly IDynamicConfig _dynamicConfig;
        private readonly IBackgroundTaskQueue _queue;
        private readonly ILogger<ImageService> _logger;
        private readonly IImageMetadataService _metadataService;

        public ImageService(IImageTransformer imageResizer, IFileSystemService imageinfoProvider, IPhotoService photoService, IDynamicConfig dynamicConfig,
            IBackgroundTaskQueue queue, ILogger<ImageService> logger, IImageMetadataService metadataService)
        {
            _imageTransformer = imageResizer;
            _fileSystemService = imageinfoProvider;
            _photoService = photoService;
            _dynamicConfig = dynamicConfig;
            _queue = queue;
            _logger = logger;
            _metadataService = metadataService;
        }

        public async Task<string> QueueMobileResize(string contextUserName, string imageFilePath, params string[] tags)
        {
            var checksum = _fileSystemService.GetChecksum(imageFilePath);
            var existingPhoto = await _photoService.GetPhotoByChecksum(checksum);

            if (existingPhoto != null)
            {
                return Path.Combine(existingPhoto.CacheFolder, existingPhoto.FileName);
            }
            var cacheFilePath = CreateCachePath(checksum, Path.GetExtension(imageFilePath));

            _queue.QueueBackgroundWorkItem((token, notifier) =>
            {
                return Task.Run(() =>
                {
                    try
                    {
                        token.ThrowIfCancellationRequested();

                        var metadata = _metadataService.GetExifData(imageFilePath);
                        var fullImagePath = CreateFullImage(imageFilePath, cacheFilePath);

                        OrientImage(fullImagePath, metadata);
                        var imageLayoutInfo = GetImageLayoutInfo(fullImagePath);

                        var smallImagePath = CreateSmallImage(fullImagePath, cacheFilePath);
                        CreateThumbnail(smallImagePath, cacheFilePath);

                        var finalPath = GetMobileUploadPath(imageFilePath);
                        _fileSystemService.MoveFile(imageFilePath, finalPath, true);

                        SavePhotoAndTags(existingPhoto, finalPath, cacheFilePath, checksum, imageLayoutInfo, metadata, tags);

                        notifier.ItemProcessed(new TaskCompleteInfo(TaskType.ProcessMobilePhoto, contextUserName, true, imageFilePath));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to process image: {imageFilePath}");
                        notifier.ItemProcessed(new TaskCompleteInfo(TaskType.ProcessMobilePhoto, contextUserName, false, imageFilePath));
                    }
                });
            });

            return cacheFilePath;
        }

        private string GetMobileUploadPath(string sourcePath)
        {
            var subfolder = DateTime.Today.ToString("yyyy-MM");
            var fullDir = Path.Combine(_dynamicConfig.MobileUploadsFolder, subfolder);

            Directory.CreateDirectory(fullDir);

            var fullPath = Path.Combine(fullDir, Path.GetFileName(sourcePath));

            return fullPath;
        }

        public ImageLayoutInfo GetImageLayoutInfo(string sourcePath)
        {
            return _imageTransformer.GetImageLayoutInfo(sourcePath);
        }

        [SuppressMessage("Security", "SCS0005:Weak random generator", Justification = "Random number is not being used for security purposes.")]
        public string CreateCachePath(string checksum, string extension)
        {
            var cachePath = Path.Combine(checksum.Substring(0, 1) + _randomNum.Next(1, 10).ToString(), Guid.NewGuid() + extension);

            return cachePath;
        }

        public string GetFullCachePath(string cachePath, ImageSizeType imageType)
        {
            var dir = Path.Combine(_dynamicConfig.CacheFolder, Path.GetDirectoryName(cachePath), imageType.ToString());
            _fileSystemService.CreateDirectory(dir);

            return Path.Combine(dir, Path.GetFileNameWithoutExtension(cachePath) + Path.GetExtension(cachePath));
        }

        public string CreateSmallImage(string imageFilePath, string cachePath)
        {
            _logger.LogInformation("Creating small image.");

            var stopwatch = Stopwatch.StartNew();

            var savePath = GetFullCachePath(cachePath, ImageSizeType.Small);
            _imageTransformer.ResizeImageByGreatestDimension(imageFilePath, savePath, _dynamicConfig.SmallImageSize);

            stopwatch.Stop();

            _logger.LogInformation("Created small image in {ElapsedMilliseconds} milliseconds at {SavePath}.",
                stopwatch.ElapsedMilliseconds, savePath);

            return savePath;
        }

        public string CreateFullImage(string imageFilePath, string cachePath)
        {
            _logger.LogInformation("Creating full image.");

            var stopwatch = Stopwatch.StartNew();

            var savePath = GetFullCachePath(cachePath, ImageSizeType.Full);
            _imageTransformer.ResizeImageByGreatestDimension(imageFilePath, savePath, _dynamicConfig.LargeImageSize);

            stopwatch.Stop();

            _logger.LogInformation("Created full image in {ElapsedMilliseconds} milliseconds at {SavePath}.",
                stopwatch.ElapsedMilliseconds, savePath);

            return savePath;
        }

        public void OrientImage(string imageFilePath, IEnumerable<ExifDirectoryBase> exifDataList)
        {
            foreach (var exifData in exifDataList)
            {
                if (exifData != null)
                {
                    if (exifData.HasTagName(ExifDirectoryBase.TagOrientation))
                    {
                        var orientation = exifData.GetDescription(ExifDirectoryBase.TagOrientation);

                        if (orientation != null)
                        {
                            var regex = new Regex(@"\(Rotate \d+ \D+\)", RegexOptions.IgnoreCase);

                            if (regex.IsMatch(orientation))
                            {
                                var parts = regex.Match(orientation).Value.Trim('(', ')').Split(' ');

                                if (parts.Length == 3)
                                {
                                    if (parts[2] == "CW" && int.TryParse(parts[1], out var degrees))
                                    {
                                        _logger.LogInformation($"Orienting image {orientation}.");
                                        var stopwatch = Stopwatch.StartNew();

                                        //var rotateDegrees = degrees == 270 ? 90 : (degrees == 90 ? 270 : 0);
                                        _imageTransformer.Rotate(imageFilePath, degrees);

                                        stopwatch.Stop();
                                        _logger.LogInformation("Oriented image in {ElapsedMilliseconds}.", stopwatch.ElapsedMilliseconds);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public string CreateThumbnail(string imageFilePath, string cachPath)
        {
            _logger.LogInformation("Creating thumbnail image.");

            var stopwatch = Stopwatch.StartNew();

            var savePath = GetFullCachePath(cachPath, ImageSizeType.Thumb);
            _imageTransformer.ResizeImageByGreatestDimension(imageFilePath, savePath, _dynamicConfig.ThumbnailSize);

            stopwatch.Stop();

            _logger.LogInformation("Created small image in {ElapsedMilliseconds} milliseconds at {SavePath}.",
                stopwatch.ElapsedMilliseconds, savePath);

            return savePath;
        }

        public Photo SavePhotoAndTags(Photo existingPhoto, string imageFilePath, string cacheFilePath, string checksum,
            ImageLayoutInfo imageLayoutInfo, IEnumerable<ExifDirectoryBase> exifDataList, params string[] tags)
        {
            _logger.LogInformation("Saving photo with checksum {Checksum}.", checksum);

            var dirTags = _fileSystemService.GetDirectoryTags(imageFilePath);
            var imageInfo = GetImageInfo(exifDataList);
            var photo = existingPhoto ?? new Photo();

            photo.Name = Path.GetFileName(imageFilePath);
            photo.FileName = Path.GetFileName(cacheFilePath);
            photo.Checksum = checksum;
            photo.CacheFolder = Path.GetDirectoryName(cacheFilePath);
            photo.DateFileCreated = File.GetCreationTime(imageFilePath);
            photo.DateTaken = imageInfo.DateTaken;
            photo.ImageHeight = imageLayoutInfo.Height;
            photo.ImageWidth = imageLayoutInfo.Width;
            photo.ReprocessCache = false;

            _photoService.SavePhoto(photo);

            if (existingPhoto == null)
            {
                var photoTags = dirTags.ToArray();

                if (tags != null && tags.Length > 0)
                {
                    Array.Copy(tags, photoTags, tags.Length);
                }
                _photoService.AssociateTags(photo, photoTags);
            }

            _logger.LogInformation("Saved photo to database.");

            return photo;
        }

        public ImageInfo GetImageInfo(IEnumerable<ExifDirectoryBase> exifDataList)
        {
            var imageInfo = new ImageInfo();
            var dateTaken = GetExifTimeTaken(exifDataList);

            if (dateTaken != null)
            {
                try
                {
                    var dateParts = dateTaken.Split(':', '-', '.', ' ', 'T');
                    imageInfo.DateTaken = new DateTime(int.Parse(dateParts[0]), int.Parse(dateParts[1]), int.Parse(dateParts[2]),
                        int.Parse(dateParts[3]), int.Parse(dateParts[4]), int.Parse(dateParts[5]));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to parse EXIF date/time taken {dateTaken}.", dateTaken);
                }
            }

            var exifTag = GetExifValue(exifDataList, ExifDirectoryBase.TagModel);

            if (exifTag != null)
            {
                imageInfo.Tags.Add(exifTag);
            }
            return imageInfo;
        }

        private string GetExifTimeTaken(IEnumerable<ExifDirectoryBase> exifDataList)
        {
            var value = "";

            foreach (var exifData in exifDataList)
            {
                value = exifData.GetDescription(ExifDirectoryBase.TagDateTime) ?? exifData.GetDescription(ExifDirectoryBase.TagDateTimeOriginal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }
            }
            return value;
        }

        private string GetExifValue(IEnumerable<ExifDirectoryBase> exifDataList, int id)
        {
            var value = "";

            foreach (var exifData in exifDataList)
            {
                value = exifData.GetDescription(id);

                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }
            }
            return value;
        }
    }
}
