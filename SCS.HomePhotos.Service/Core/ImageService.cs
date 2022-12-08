using MetadataExtractor.Formats.Exif;

using Microsoft.Extensions.Logging;

using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Service.Workers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Core
{
    /// <summary>
    /// Image service.
    /// </summary>
    public class ImageService : HomePhotosService, IImageService
    {
        private readonly Random _randomNum = new();

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
        /// <param name="imageinfoProvider">The image info provider.</param>
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
        /// Gets the original sub-folder of an image file, relative to the root folder of the mobile or regular index folder.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="isMobile">if set to <c>true</c> file is a mobile upload.</param>
        /// <param name="imageFilePath">The image file path.</param>
        /// <returns>The original sub-folder name.</returns>
        public static string GetOriginalFolder(IDynamicConfig config, bool isMobile, string imageFilePath)
        {
            var subPath = isMobile
                ? imageFilePath.Replace(config.MobileUploadsFolder, string.Empty, StringComparison.InvariantCultureIgnoreCase)
                : imageFilePath.Replace(config.IndexPath, string.Empty, StringComparison.InvariantCultureIgnoreCase);

            return Path.GetDirectoryName(subPath.Trim('\\'));
        }

        /// <summary>
        /// Queues the image to be resized for mobile display.
        /// </summary>
        /// <param name="uploadedBy">User that uploaded file.</param>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>The cached file path.</returns>
        public async Task<string> QueueMobileResize(User uploadedBy, string imageFilePath, List<Tag> tags = null)
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

                        var firstTag = tags.Any() ? tags.First().TagName : null;
                        var finalPath = GetMobileUploadPath(imageFilePath, uploadedBy.UserName, firstTag);
                        _fileSystemService.MoveFile(imageFilePath, finalPath, true);
                        tags.Add(new Tag { TagName = $"{uploadedBy.UserName} Upload", UserId = null }); // system tag

                        var imageInfo = GetImageInfo(metadata);
                        SavePhotoAndTags(existingPhoto, finalPath, cacheFilePath, checksum, imageLayoutInfo, imageInfo, tags);

                        notifier.ItemProcessed(new TaskCompleteInfo(TaskType.ProcessMobilePhoto, uploadedBy.UserName, true, imageFilePath));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process image: {imageFilePath}.", imageFilePath);
                        notifier.ItemProcessed(new TaskCompleteInfo(TaskType.ProcessMobilePhoto, uploadedBy.UserName, false, imageFilePath));
                    }
                });
            });

            return cacheFilePath;
        }

        /// <summary>
        /// Gets the mobile upload path.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="uploadedBy">The username who uploaded file.</param>
        /// <param name="firstTag">The first tag for uploaded file.</param>
        /// <returns>The full mobile upload directory for uploaded file.</returns>
        private string GetMobileUploadPath(string sourcePath, string uploadedBy, string firstTag)
        {
            //var subfolder = DateTime.Today.ToString("yyyy-MM");
            var fullDir = Path.Combine(_dynamicConfig.MobileUploadsFolder, uploadedBy, firstTag?.CleanForFileName());

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
        /// <param name="exifDataList">The EXIF data list.</param>
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
        /// <param name="cachPath">The cache path.</param>
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
        /// <param name="imageInfo">The image metadata.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>The updated/created photo.</returns>
        public Photo SavePhotoAndTags(Photo existingPhoto, string imageFilePath, string cacheFilePath, string checksum,
            ImageLayoutInfo imageLayoutInfo, ImageInfo imageInfo, List<Tag> tags)
        {
            _logger.LogInformation("Saving photo with checksum {Checksum}.", checksum);

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
            photo.OriginalFolder = GetOriginalFolder(_dynamicConfig, photo.MobileUpload, imageFilePath);

            _photoService.AssociateUser(photo, tags);
            tags.ForEach(t => t.UserId = photo.UserId);

            _photoService.SavePhoto(photo);
            _photoService.AssociateTags(photo, tags);            

            _logger.LogInformation("Saved photo to database.");

            return photo;
        }

        /// <summary>
        /// Gets basic image information.
        /// </summary>
        /// <param name="exifDataList">The EXIF data list.</param>
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
                    _logger.LogError(ex, "Failed to parse EXIF date/time taken '{dateTaken}'.", dateTaken);
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

            foreach (var exifData in new ExifDirectoryBase[] { exifData1, exifData2 })
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
