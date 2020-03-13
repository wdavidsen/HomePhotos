using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Workers;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service
{
    public class ImageService : IImageService
    {
        private Random _randomNum = new Random();

        private readonly IImageResizer _imageResizer;
        private readonly IFileSystemService _fileSystemService;
        private readonly IPhotoService _photoService;
        private readonly IDynamicConfig _dynamicConfig;
        private readonly IBackgroundTaskQueue _queue;

        private readonly ILogger<ImageService> _logger;

        public ImageService(IImageResizer imageResizer, IFileSystemService imageinfoProvider, IPhotoService photoService, IDynamicConfig dynamicConfig, IBackgroundTaskQueue queue, ILogger<ImageService> logger)
        {
            _imageResizer = imageResizer;
            _fileSystemService = imageinfoProvider;
            _photoService = photoService;
            _dynamicConfig = dynamicConfig;
            _queue = queue;
            _logger = logger;
        }

        public async Task<string> QueueMobileResize(string imageFilePath, bool copyToTempFolder = true)
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

                    var fullImagePath = CreateFullImage(imageFilePath, cacheFilePath);
                    var smallImagePath = CreateSmallImage(fullImagePath, cacheFilePath);
                    CreateThumbnail(smallImagePath, cacheFilePath);
                    SavePhotoAndTags(imageFilePath, cacheFilePath, checksum);                       
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
            return _imageResizer.GetImageLayoutInfo(sourcePath);
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
            _imageResizer.ResizeImageByGreatestDimension(imageFilePath, savePath, _dynamicConfig.SmallImageSize);

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
            _imageResizer.ResizeImageByGreatestDimension(imageFilePath, savePath, _dynamicConfig.LargeImageSize);

            stopwatch.Stop();

            _logger.LogInformation("Created full image in {ElapsedMilliseconds} milliseconds at {SavePath}.",
                stopwatch.ElapsedMilliseconds, savePath);

            return savePath;
        }

        public string CreateThumbnail(string imageFilePath, string cachPath)
        {
            _logger.LogInformation("Creating thumbnail image.");

            var stopwatch = Stopwatch.StartNew();

            var savePath = GetFullCachePath(cachPath, ImageSizeType.Thumb);
            _imageResizer.ResizeImageByGreatestDimension(imageFilePath, savePath, _dynamicConfig.ThumbnailSize);

            stopwatch.Stop();

            _logger.LogInformation("Created small image in {ElapsedMilliseconds} milliseconds at {SavePath}.",
                stopwatch.ElapsedMilliseconds, savePath);

            return savePath;
        }

        public Photo SavePhotoAndTags(string imageFilePath, string cacheFilePath, string checksum)
        {
            _logger.LogInformation("Saving photo with checksum {Checksum}.", checksum);

            var imageInfo = _fileSystemService.GetInfo(imageFilePath);
            var photo = new Photo
            {
                Name = Path.GetFileName(imageFilePath),
                FileName = Path.GetFileName(cacheFilePath),
                Checksum = checksum,
                CacheFolder = Path.GetDirectoryName(cacheFilePath),
                DateFileCreated = File.GetCreationTime(imageFilePath),
                DateTaken = imageInfo.DateTaken
            };
            _photoService.SavePhoto(photo);
            _photoService.AssociateTags(photo, imageInfo.Tags.ToArray());

            _logger.LogInformation("Saved photo to database.");

            return photo;
        }
    }
}
