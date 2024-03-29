using AutoFixture;
using MetadataExtractor.Formats.Exif;
using Microsoft.Extensions.Logging;
using Moq;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Service.Core;
using SCS.HomePhotos.Service.Workers;
using SCS.HomePhotos.Workers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SCS.HomePhotos.Service.Test
{
    public class ImageServiceTests
    {
        private readonly Fixture _fixture;

        private readonly ImageService _imageService;
        private readonly IBackgroundTaskQueue _backgroundQueue;

        private readonly Mock<IImageTransformer> _imageTransformer;
        private readonly Mock<IFileSystemService> _fileSystemService;
        private readonly Mock<IPhotoService> _photoService;
        private readonly Mock<IDynamicConfig> _dynamicConfig;
        private readonly Mock<ILogger<ImageService>> _logger;
        private readonly Mock<IImageMetadataService> _metadataService;

        public ImageServiceTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _imageTransformer = new Mock<IImageTransformer>();
            _fileSystemService = new Mock<IFileSystemService>();
            _photoService = new Mock<IPhotoService>();
            _dynamicConfig = new Mock<IDynamicConfig>();
            _logger = new Mock<ILogger<ImageService>>();
            _metadataService = new Mock<IImageMetadataService>();

            _dynamicConfig.SetupGet(p => p.MobileUploadsFolder).Returns(@"c:\homephotos\mobileuploads");
            _dynamicConfig.SetupGet(p => p.IndexPath).Returns(@"c:\homephotos\root");

            _backgroundQueue = new BackgroundTaskQueue();
            _imageService = new ImageService(_imageTransformer.Object, _fileSystemService.Object, _photoService.Object, _dynamicConfig.Object, _backgroundQueue,
                _logger.Object, _metadataService.Object);
        }

        [Fact]
        public async Task QueueMobileResize()
        {
            var filePath = Path.Combine("MobileUploads", DateTime.Now.ToString("yyyy-MM"), "birthday.jpg");
            var cacheDir = Path.Combine("homePhotos", "cache");
            var checksum = "abc123";

            _fileSystemService.Setup(m => m.GetChecksum(It.IsAny<string>())).Returns(checksum);            
            _photoService.Setup(m => m.GetPhotoByChecksum(It.IsAny<string>())).ReturnsAsync(default(Photo));

            _dynamicConfig.SetupGet(o => o.CacheFolder).Returns(cacheDir);
            _dynamicConfig.SetupGet(o => o.LargeImageSize).Returns(800);
            _dynamicConfig.SetupGet(o => o.ThumbnailSize).Returns(200);
            _dynamicConfig.SetupGet(o => o.MobileUploadsFolder).Returns("MobileUploads");

            var user = new User { UserId = 1, UserName = "wdavidsen" };
            var cachePath = await _imageService.QueueMobileResize(user, filePath);

            try
            {
                var token = new CancellationTokenSource().Token;
                var workItem = await _backgroundQueue.DequeueAsync(token);
                await workItem(token, new QueueEvents { ItemProcessed = (info) => { } });
            }
            catch (TaskCanceledException) { }

            _fileSystemService.Verify(m => m.GetChecksum(filePath),
                Times.Once);
                        
            _photoService.Verify(m => m.GetPhotoByChecksum(checksum),
                Times.Once);

            _imageTransformer.Verify(m => m.ResizeImageByGreatestDimension(It.IsAny<string>(), It.IsAny<string>(), 200),
                Times.Exactly(1));

            _imageTransformer.Verify(m => m.ResizeImageByGreatestDimension(filePath, It.IsAny<string>(), 800),
                Times.Exactly(1));

            _photoService.Verify(m => m.SavePhoto(It.IsAny<Photo>()),
                Times.Once);

            _photoService.Verify(m => m.AssociateTags(It.IsAny<Photo>(), It.IsAny<IEnumerable<Tag>>()),
                Times.Once);

            _logger.Verify(m => m.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()),
                Times.Never);

            Assert.NotNull(cachePath);
            Assert.NotEmpty(cachePath);

            var fileParts = cachePath.Split('/', '\\');

            Assert.Equal(2, fileParts[0].Length);
            Assert.Equal(checksum[..1], fileParts[0][..1]);
            Assert.Equal(Path.GetExtension(filePath), Path.GetExtension(fileParts[1]));
        }

        [Fact]
        public async Task QueueMobileResizeEventFired()
        {
            var filePath = Path.Combine("MobileUploads", DateTime.Now.ToString("yyyy-MM"), "birthday.jpg");
            var cacheDir = Path.Combine("homePhotos", "cache");
            var checksum = "abc123";
            var completeInfo = new TaskCompleteInfo(TaskType.ProcessMobilePhoto, "wdavidsen", true);

            _fileSystemService.Setup(m => m.GetChecksum(It.IsAny<string>())).Returns(checksum);            
            _photoService.Setup(m => m.GetPhotoByChecksum(It.IsAny<string>())).ReturnsAsync(default(Photo));

            _dynamicConfig.SetupGet(o => o.CacheFolder).Returns(cacheDir);
            _dynamicConfig.SetupGet(o => o.LargeImageSize).Returns(800);
            _dynamicConfig.SetupGet(o => o.ThumbnailSize).Returns(200);
            _dynamicConfig.SetupGet(o => o.MobileUploadsFolder).Returns("MobileUploads");

            var user = new User { UserId = 1, UserName = "wdavidsen" };
            var cachePath = await _imageService.QueueMobileResize(user, filePath);

            var queueEvents = new Mock<IQueueEvents>();
            queueEvents.SetupGet(m => m.ItemProcessed)
                .Returns((info) =>
                {
                    Assert.Equal(completeInfo.Type, info.Type);
                    Assert.Equal(completeInfo.Success, info.Success);
                    Assert.Equal(completeInfo.ContextUserName, info.ContextUserName);
                    Assert.NotNull(info.Data);
                    Assert.Equal(filePath, info.Data.ToString());
                });

            try
            {
                var token = new CancellationTokenSource().Token;
                var workItem = await _backgroundQueue.DequeueAsync(token);
                await workItem(token, queueEvents.Object);
            }
            catch (TaskCanceledException) { }
        }

        [Fact]
        public async Task QueueMobileResizeFullResizeFailed()
        {
            var filePath = Path.Combine("photos", "party", "birthday.jpg");
            var cacheDir = Path.Combine("home", "homePhotos", "cache");
            var checksum = "abc123";

            _fileSystemService.Setup(m => m.GetChecksum(It.IsAny<string>())).Returns(checksum);
            _photoService.Setup(m => m.GetPhotoByChecksum(It.IsAny<string>())).ReturnsAsync(default(Photo));
            _dynamicConfig.SetupGet(o => o.CacheFolder).Returns(cacheDir);
            _dynamicConfig.SetupGet(o => o.LargeImageSize).Returns(800);
            _dynamicConfig.SetupGet(o => o.ThumbnailSize).Returns(200);

            _imageTransformer.Setup(m => m.ResizeImageByGreatestDimension(filePath, It.IsAny<string>(), 800))
                .Throws<OutOfMemoryException>();

            var user = new User { UserId = 1, UserName = "wdavidsen" };
            var cachePath = await _imageService.QueueMobileResize(user, filePath);

            try
            {
                var token = new CancellationTokenSource().Token;
                var workItem = await _backgroundQueue.DequeueAsync(token);
                await workItem(token, new QueueEvents { ItemProcessed = (info) => { } });
            }
            catch (TaskCanceledException) { }

            _fileSystemService.Verify(m => m.GetChecksum(filePath),
                Times.Once);

            _photoService.Verify(m => m.GetPhotoByChecksum(checksum),
                Times.Once);

            _imageTransformer.Verify(m => m.ResizeImageByGreatestDimension(filePath, It.IsAny<string>(), 800),
               Times.Once);

            _imageTransformer.Verify(m => m.ResizeImageByGreatestDimension(filePath, It.IsAny<string>(), 200),
                Times.Never);

            _photoService.Verify(m => m.SavePhoto(It.IsAny<Photo>()),
                Times.Never);

            _photoService.Verify(m => m.AssociateTags(It.IsAny<Photo>(), It.IsAny<IEnumerable<Tag>>()),
                Times.Never);

            //_logger.Verify(m => m.Log<FormattedLogValues>(LogLevel.Error, 0, It.IsAny<string>()),
            //    Times.Once);
        }

        [Fact]
        public async Task QueueMobileResizeThumbnailResizeFailed()
        {
            var filePath = Path.Combine("photos", "party", "birthday.jpg");
            var cacheDir = Path.Combine("home", "homePhotos", "cache");
            var checksum = "abc123";

            _fileSystemService.Setup(m => m.GetChecksum(It.IsAny<string>())).Returns(checksum);
            _photoService.Setup(m => m.GetPhotoByChecksum(It.IsAny<string>())).ReturnsAsync(default(Photo));
            _dynamicConfig.SetupGet(o => o.CacheFolder).Returns(cacheDir);
            _dynamicConfig.SetupGet(o => o.LargeImageSize).Returns(800);
            _dynamicConfig.SetupGet(o => o.ThumbnailSize).Returns(200);

            _imageTransformer.Setup(m => m.ResizeImageByGreatestDimension(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Throws<OutOfMemoryException>();

            var user = new User { UserId = 1, UserName = "wdavidsen" };
            var cachePath = await _imageService.QueueMobileResize(user, filePath);

            try
            {
                var token = new CancellationTokenSource().Token;
                var workItem = await _backgroundQueue.DequeueAsync(token);
                await workItem(token, new QueueEvents { ItemProcessed = (info) => { } });
            }
            catch (TaskCanceledException) { }

            _fileSystemService.Verify(m => m.GetChecksum(filePath),
                Times.Once);

            _photoService.Verify(m => m.GetPhotoByChecksum(checksum),
                Times.Once);

            _imageTransformer.Verify(m => m.ResizeImageByGreatestDimension(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
               Times.Once);

            _photoService.Verify(m => m.SavePhoto(It.IsAny<Photo>()),
                Times.Never);

            _photoService.Verify(m => m.AssociateTags(It.IsAny<Photo>(), It.IsAny<IEnumerable<Tag>>()),
                Times.Never);

            //_logger.Verify(m => m.Log<FormattedLogValues>(LogLevel.Error, 0, It.IsAny<string>()),
            //    Times.Once);
        }

        [Theory]
        [InlineData(ImageSizeType.Full)]
        [InlineData(ImageSizeType.Thumb)]
        public void GetFullCachePath(ImageSizeType imageSize)
        {
            var cacheDir = Path.Combine("home", "homePhotos", "cache");
            var cacheFilePath = Path.Combine("c1", "983767D2-96B3-4372-89D1-74C190EE23B0.jpg");
            var expectedFullCachePath = Path.Combine(cacheDir, "c1", imageSize.ToString(), "983767D2-96B3-4372-89D1-74C190EE23B0.jpg");

            _dynamicConfig.SetupGet(o => o.CacheFolder).Returns(cacheDir);

            var actualFullCachePath = _imageService.GetFullCachePath(cacheFilePath, imageSize);

            _dynamicConfig.VerifyGet(o => o.CacheFolder, Times.Once);

            Assert.Equal(expectedFullCachePath, actualFullCachePath);
        }

        [Fact]
        public void CreateFullSizeImage()
        {
            var cacheDir = Path.Combine("home", "homePhotos", "cache");
            var imageFilePath = Path.Combine("home", "homePhotos", "uploads", "birthday.jpg");
            var cacheFilePath = Path.Combine("c1", "983767D2-96B3-4372-89D1-74C190EE23B0.jpg");
            var savePath = Path.Combine(cacheDir, "c1", "Full", "983767D2-96B3-4372-89D1-74C190EE23B0.jpg");

            _dynamicConfig.SetupGet(o => o.CacheFolder).Returns(cacheDir);
            _dynamicConfig.SetupGet(o => o.LargeImageSize).Returns(800);

            _imageTransformer.Setup(m => m.ResizeImageByGreatestDimension(imageFilePath, savePath, 800));

            _imageService.CreateFullImage(imageFilePath, cacheFilePath);

            _dynamicConfig.VerifyGet(o => o.CacheFolder, Times.Once);
            _dynamicConfig.VerifyGet(o => o.LargeImageSize, Times.Once);

            _imageTransformer.Verify(m => m.ResizeImageByGreatestDimension(imageFilePath, savePath, 800), Times.Once);
        }

        [Fact]
        public void CreateThumbnail()
        {
            var cacheDir = Path.Combine("home", "homePhotos", "cache");
            var imageFilePath = Path.Combine("home", "homePhotos", "uploads", "birthday.jpg");
            var cacheFilePath = Path.Combine("c1", "983767D2-96B3-4372-89D1-74C190EE23B0.jpg");
            var savePath = Path.Combine(cacheDir, "c1", "Thumb", "983767D2-96B3-4372-89D1-74C190EE23B0.jpg");

            _dynamicConfig.SetupGet(o => o.CacheFolder).Returns(cacheDir);
            _dynamicConfig.SetupGet(o => o.ThumbnailSize).Returns(200);

            _imageTransformer.Setup(m => m.ResizeImageByGreatestDimension(imageFilePath, savePath, 200));

            _imageService.CreateThumbnail(imageFilePath, cacheFilePath);

            _dynamicConfig.VerifyGet(o => o.CacheFolder, Times.Once);
            _dynamicConfig.VerifyGet(o => o.ThumbnailSize, Times.Once);

            _imageTransformer.Verify(m => m.ResizeImageByGreatestDimension(imageFilePath, savePath, 200), Times.Once);
        }

        [Fact]
        public void SavePhotoAndTags()
        {
            var checksum = "abc123";
            var fileNameOriginal = "bill-birthday.jpg";
            var fileNameCache = "983767D2-96B3-4372-89D1-74C190EE23B0.jpg";
            var cacheSubfolder = "c1";
            var cacheFilePath = Path.Combine(cacheSubfolder, fileNameCache);
            var imageFilePath = Path.Combine("home", "homePhotos", "parties", "birthdays", fileNameOriginal);
            var tags = new List<Tag> { new Tag { TagName = "Tag1" }, new Tag { TagName = "Tag2" } };
            var exifData = new List<ExifDirectoryBase> { { new ExifSubIfdDirectory() }, { new ExifIfd0Directory() } };
            var imageInfo = new ImageInfo();

            var imageLayoutInfo = new ImageLayoutInfo
            {
                Height = 1000,
                Width = 3000,
                LayoutType = ImageLayoutType.Landscape,
                Ratio = 0.33m
            };

            _photoService.Setup(m => m.SavePhoto(It.IsAny<Model.Photo>()))
                .Callback<Model.Photo>((photo) =>
                {
                    Assert.Equal(checksum, photo.Checksum);
                    Assert.Equal(fileNameOriginal, photo.Name);
                    Assert.Equal(fileNameCache, photo.FileName);
                    Assert.Equal(cacheSubfolder, photo.CacheFolder);
                });

            _photoService.Setup(m => m.AssociateTags(It.IsAny<Model.Photo>(), It.IsAny<IEnumerable<Tag>>()))
                .Callback<Model.Photo, IEnumerable<Tag>>((photo, tagList) =>
                {
                    Assert.Equal(checksum, photo.Checksum);
                    Assert.Equal(fileNameOriginal, photo.Name);
                    Assert.Equal(fileNameCache, photo.FileName);
                    Assert.Equal(cacheSubfolder, photo.CacheFolder);

                    Assert.NotNull(tagList);
                    Assert.True(tagList.Count() == 2);
                });

            //_imageTransformer.Setup(m => m.GetImageLayoutInfo(It.IsAny<string>())).Returns(imageLayoutInfo);

            var imageFileInfo = new ImageFileInfo(ImageFileSource.LocalDisk, imageFilePath, cacheFilePath, checksum);

            _imageService.SavePhotoAndTags(null, imageFileInfo, imageLayoutInfo, imageInfo, tags, null);

            _photoService.Verify(m => m.SavePhoto(It.IsAny<Model.Photo>()), Times.Once);
            _photoService.Verify(m => m.AssociateTags(It.IsAny<Model.Photo>(), It.IsAny<IEnumerable<Tag>>()), Times.Once);
        }
    }
}
