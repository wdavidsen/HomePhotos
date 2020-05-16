using MetadataExtractor.Formats.Exif;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Workers;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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

        public ImageService(IImageTransformer imageResizer, IFileSystemService imageinfoProvider, IPhotoService photoService, IDynamicConfig dynamicConfig, IBackgroundTaskQueue queue, ILogger<ImageService> logger)
        {
            _imageTransformer = imageResizer;
            _fileSystemService = imageinfoProvider;
            _photoService = photoService;
            _dynamicConfig = dynamicConfig;
            _queue = queue;
            _logger = logger;
        }

        public async Task<string> QueueMobileResize(string imageFilePath, bool copyToTempFolder = true, params string[] tags)
        {
            var checksum = _fileSystemService.GetChecksum(imageFilePath);
            var existingPhoto = await _photoService.GetPhotoByChecksum(checksum);

            if (existingPhoto != null)
            {
                return Path.Combine(existingPhoto.CacheFolder, existingPhoto.FileName);
            }
            var cacheFilePath = CreateCachePath(checksum, Path.GetExtension(imageFilePath));

            _queue.QueueBackgroundWorkItem((token) =>
            {
                try
                {
                    token.ThrowIfCancellationRequested();

                    var directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(imageFilePath);
                    var exifData = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                    OrientImage(imageFilePath, exifData);

                    var imageLayoutInfo = GetImageLayoutInfo(imageFilePath);
                    var fullImagePath = CreateFullImage(imageFilePath, cacheFilePath);
                    var smallImagePath = CreateSmallImage(fullImagePath, cacheFilePath);
                    CreateThumbnail(smallImagePath, cacheFilePath);
                    SavePhotoAndTags(imageFilePath, cacheFilePath, checksum, imageLayoutInfo, exifData);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to process image: {imageFilePath}");
                }
                return Task.CompletedTask;
            });

            return cacheFilePath;
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

        public void OrientImage(string imageFilePath, ExifSubIfdDirectory exifData)
        {
            if (exifData != null)
            {
                if (exifData.HasTagName(ExifDirectoryBase.TagOrientation))
                {
                    var orientation = exifData.GetDescription(ExifDirectoryBase.TagOrientation);

                    if (orientation != null)
                    {
                        var parts = orientation.Split(' ');

                        if (parts.Length == 2)
                        {
                            if (int.TryParse(parts[0], out var degrees) && parts[0] == "CW")
                            {
                                _logger.LogInformation($"Orienting image {orientation}.");
                                var stopwatch = Stopwatch.StartNew();

                                _imageTransformer.Rotate(imageFilePath, degrees * -1);

                                stopwatch.Stop();
                                _logger.LogInformation("Oriented image in {ElapsedMilliseconds}.", stopwatch.ElapsedMilliseconds);
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

        public Photo SavePhotoAndTags(string imageFilePath, string cacheFilePath, string checksum, 
            ImageLayoutInfo imageLayoutInfo, ExifSubIfdDirectory exifData, params string[] tags)
        {
            _logger.LogInformation("Saving photo with checksum {Checksum}.", checksum);

            var dirTags = _fileSystemService.GetDirectoryTags(imageFilePath);
            var imageInfo = GetImageInfo(exifData);

            var photo = new Photo
            {
                Name = Path.GetFileName(imageFilePath),
                FileName = Path.GetFileName(cacheFilePath),
                Checksum = checksum,
                CacheFolder = Path.GetDirectoryName(cacheFilePath),
                DateFileCreated = File.GetCreationTime(imageFilePath),
                DateTaken = imageInfo.DateTaken,
                ImageHeight = imageLayoutInfo.Height,
                ImageWidth = imageLayoutInfo.Width
            };

            var photoTags = dirTags.ToArray();

            if (tags != null && tags.Length > 0)
            {
                Array.Copy(tags, photoTags, tags.Length);
            }

            _photoService.SavePhoto(photo);
            _photoService.AssociateTags(photo, photoTags);

            _logger.LogInformation("Saved photo to database.");

            return photo;
        }

        public ImageInfo GetImageInfo(ExifSubIfdDirectory exifData)
        {
            var imageInfo = new ImageInfo();

            if (exifData != null)
            {
                if (exifData.HasTagName(ExifDirectoryBase.TagDateTimeOriginal))
                {
                    var dateTaken = exifData.GetDescription(ExifDirectoryBase.TagDateTimeOriginal);
                    var dateParts = dateTaken.Split(':', '-', '.', ' ', 'T');
                    imageInfo.DateTaken = new DateTime(int.Parse(dateParts[0]), int.Parse(dateParts[1]), int.Parse(dateParts[2]),
                        int.Parse(dateParts[3]), int.Parse(dateParts[4]), int.Parse(dateParts[5]));
                }

                var exifTag = exifData.GetDescription(ExifDirectoryBase.TagModel);

                if (exifTag != null)
                {
                    imageInfo.Tags.Add(exifTag);
                }
            }
            return imageInfo;
        }
    }
}
