using MetadataExtractor.Formats.Exif;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Service.Workers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Core
{
    /// <summary>
    /// Image service.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageService"/> class.
        /// </summary>
        /// <param name="imageResizer">The image resizer.</param>
        /// <param name="imageinfoProvider">The imageinfo provider.</param>
        /// <param name="photoService">The photo service.</param>
        /// <param name="dynamicConfig">The dynamic configuration.</param>
        /// <param name="queue">The queue.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="metadataService">The metadata service.</param>
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

        /// <summary>
        /// Queues the image to be resized for mobile display.
        /// </summary>
        /// <param name="contextUserName">Name of the context user.</param>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>The cached file path.</returns>
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
                        _logger.LogError(ex, "Failed to process image: {imageFilePath}.", imageFilePath);
                        notifier.ItemProcessed(new TaskCompleteInfo(TaskType.ProcessMobilePhoto, contextUserName, false, imageFilePath));
                    }
                });
            });

            return cacheFilePath;
        }

        /// <summary>
        /// Gets the mobile upload path.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <returns></returns>
        private string GetMobileUploadPath(string sourcePath)
        {
            var subfolder = DateTime.Today.ToString("yyyy-MM");
            var fullDir = Path.Combine(_dynamicConfig.MobileUploadsFolder, subfolder);

            Directory.CreateDirectory(fullDir);

            var fullPath = Path.Combine(fullDir, Path.GetFileName(sourcePath));

            return fullPath;
        }

        /// <summary>
        /// Gets the image layout information.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <returns>
        /// The image layout information.
        /// </returns>
        public ImageLayoutInfo GetImageLayoutInfo(string sourcePath)
        {
            return _imageTransformer.GetImageLayoutInfo(sourcePath);
        }

        /// <summary>
        /// Creates the cache path.
        /// </summary>
        /// <param name="checksum">The checksum.</param>
        /// <param name="extension">The extension.</param>
        /// <returns>
        /// The image cache path.
        /// </returns>
        [SuppressMessage("Security", "SCS0005:Weak random generator", Justification = "Random number is not being used for security purposes.")]
        public string CreateCachePath(string checksum, string extension)
        {
            var cachePath = Path.Combine(string.Concat(checksum.AsSpan(0, 1), _randomNum.Next(1, 10).ToString()), Guid.NewGuid() + extension);

            return cachePath;
        }

        /// <summary>
        /// Gets the full-sized image cache path.
        /// </summary>
        /// <param name="cachePath">The cache path.</param>
        /// <param name="imageType">Type of the image.</param>
        /// <returns>
        /// The full-sized image cache path.
        /// </returns>
        public string GetFullCachePath(string cachePath, ImageSizeType imageType)
        {
            var dir = Path.Combine(_dynamicConfig.CacheFolder, Path.GetDirectoryName(cachePath), imageType.ToString());
            _fileSystemService.CreateDirectory(dir);

            return Path.Combine(dir, Path.GetFileNameWithoutExtension(cachePath) + Path.GetExtension(cachePath));
        }

        /// <summary>
        /// Creates the small image.
        /// </summary>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="cachePath">The cache path.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates the full image.
        /// </summary>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="cachePath">The cache path.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Orients the image for proper viewing.
        /// </summary>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="exifDataList">The exif data list.</param>
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
                                        _logger.LogInformation("Orienting image {orientation}.", orientation);
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

        /// <summary>
        /// Creates the thumbnail.
        /// </summary>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="cachPath">The cach path.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Saves the photo and tags.
        /// </summary>
        /// <param name="existingPhoto">The existing photo.</param>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="cacheFilePath">The cache file path.</param>
        /// <param name="checksum">The checksum.</param>
        /// <param name="imageLayoutInfo">The image layout information.</param>
        /// <param name="exifDataList">The exif data list.</param>
        /// <param name="tags">The tags.</param>
        /// <returns></returns>
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
            photo.MobileUpload = imageFilePath.Contains(_dynamicConfig.MobileUploadsFolder, StringComparison.InvariantCultureIgnoreCase);
            photo.OriginalFolder = photo.MobileUpload
                ? imageFilePath.Replace(_dynamicConfig.MobileUploadsFolder, string.Empty, StringComparison.InvariantCultureIgnoreCase)
                : imageFilePath.Replace(_dynamicConfig.IndexPath, string.Empty, StringComparison.InvariantCultureIgnoreCase);

            _photoService.SavePhoto(photo);

            if (existingPhoto == null)
            {
                var photoTags = dirTags.ToList();

                if (tags != null && tags.Length > 0)
                {
                    photoTags.AddRange(tags);
                }
                _photoService.AssociateTags(photo, photoTags.ToArray());
            }

            _logger.LogInformation("Saved photo to database.");

            return photo;
        }

        /// <summary>
        /// Gets basic image information.
        /// </summary>
        /// <param name="exifDataList">The exif data list.</param>
        /// <returns>
        /// Basic image information.
        /// </returns>
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

        private static string GetExifTimeTaken(IEnumerable<ExifDirectoryBase> directories)
        {
            var value = "";

            var exifData1 = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
            var exifData2 = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            
            foreach (var exifData in new ExifDirectoryBase[] { exifData1 , exifData2 })
            {
                if (exifData == null)
                {
                    continue;
                }

                value = exifData.GetDescription(ExifDirectoryBase.TagDateTime) ?? exifData.GetDescription(ExifDirectoryBase.TagDateTimeOriginal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }
            }
            return value;
        }

        private static string GetExifValue(IEnumerable<ExifDirectoryBase> exifDataList, int id)
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
