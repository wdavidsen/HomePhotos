using AutoFixture;

using Microsoft.Extensions.Logging;

using Moq;

using SCS.HomePhotos.Data;
using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Service.Core;
using SCS.HomePhotos.Service.Workers;
using SCS.HomePhotos.Workers;

using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace SCS.HomePhotos.Service.Test
{
    public class PhotoServiceTests
    {
        private readonly Fixture _fixture;

        private readonly PhotoService _photoService;
        private readonly Mock<IPhotoData> _photoData;
        private readonly Mock<ITagData> _tagData;
        private readonly Mock<IUserData> _userData;
        private readonly Mock<IPhotoTagData> _photoTagData;
        private readonly Mock<IFileExclusionData> _fileExclusionData;
        private readonly Mock<ILogger<PhotoService>> _logger;
        private readonly Mock<IAdminLogService> _adminLogger;
        private readonly Mock<IFileSystemService> _fileSystemService;
        private readonly Mock<IDynamicConfig> _dynamicCache;
        private readonly Mock<IPrincipal> _userContext;
        private readonly Mock<IIdentity> _identity;
        private readonly IBackgroundTaskQueue _backgroundQueue;

        public PhotoServiceTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _photoData = new Mock<IPhotoData>();
            _tagData = new Mock<ITagData>();
            _photoTagData = new Mock<IPhotoTagData>();
            _logger = new Mock<ILogger<PhotoService>>();
            _adminLogger = new Mock<IAdminLogService>();
            _fileExclusionData = new Mock<IFileExclusionData>();
            _userData = new Mock<IUserData>();
            _fileSystemService = new Mock<IFileSystemService>();
            _dynamicCache = new Mock<IDynamicConfig>();
            _userContext = new Mock<IPrincipal>();
            _identity = new Mock<IIdentity>();
            _backgroundQueue = new BackgroundTaskQueue();

            _identity.SetupGet(p => p.Name).Returns("wdavidsen");
            _userContext.SetupGet(p => p.Identity).Returns(_identity.Object);

            _photoService = new PhotoService(_photoData.Object, _tagData.Object, _photoTagData.Object, _fileExclusionData.Object, _userData.Object,
                _logger.Object, _adminLogger.Object, _fileSystemService.Object, _dynamicCache.Object, _backgroundQueue);

            _photoService.SetUserContext(_userContext.Object);
        }

        [Fact]
        public async Task GetPhotoByChecksum()
        {
            var photos = _fixture.CreateMany<Photo>(1);
            var checksum = _fixture.Create<string>();

            _photoData.Setup(m => m.GetListAsync(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(photos)
                .Callback<string, object>((where, p) =>
                {
                    Assert.StartsWith("WHERE ", where);
                    Assert.Equal(checksum, p.GetProperty("Checksum").ToString());
                });

            var result = await _photoService.GetPhotoByChecksum(checksum);

            _photoData.Verify(m => m.GetListAsync(It.IsAny<string>(), It.IsAny<object>()),
                Times.Once);

            Assert.Equal(photos.First(), result);
        }

        [Fact]
        public async Task GetLatestPhotos()
        {
            var photos = _fixture.CreateMany<Photo>(50);
            var userFilter = new UserFilter(1);

            _photoData.Setup(m => m.GetPhotos(It.IsAny<UserFilter>(), It.IsAny<DateRange>(), 1, 50))
                .ReturnsAsync(photos);

            var results = await _photoService.GetLatestPhotos(1, 50);

            _photoData.Verify(m => m.GetPhotos(It.IsAny<UserFilter>(), It.IsAny<DateRange>(), 1, 50),
                Times.Once);

            Assert.Equal(50, results.Count());
        }

        [Fact]
        public async Task GetPhotosByTag()
        {
            var photos = _fixture.CreateMany<Photo>(50);
            var tag = _fixture.Create<string>();
            var owner = "wdavidsen";
            var ownerId = 1 as int?;
            var userFilter = new UserFilter(1);

            _userData.Setup(m => m.GetUser(owner)).ReturnsAsync(new Model.User { UserName = owner, UserId = ownerId });

            _photoData.Setup(m => m.GetPhotos(It.IsAny<UserFilter>(), It.IsAny<string>(), 1, 50))
                .ReturnsAsync(photos)
                .Callback<string, int?, int, int>((t, o, pageNum, pageSize) =>
                {
                    Assert.Equal(1, pageNum);
                    Assert.Equal(50, pageSize);
                    Assert.Equal(tag, t);
                    Assert.Equal(ownerId, o);
                }); 

            var results = await _photoService.GetPhotosByTag(tag, owner, 1, 50);

            _photoData.Verify(m => m.GetPhotos(It.IsAny<UserFilter>(), It.IsAny<string>(), 1, 50),
                Times.Once);

            Assert.Equal(50, results.Count());
        }

        [Fact]
        public async Task GetPhotosByDateTaken()
        {
            var startDate = DateTime.Now.AddDays(-7);
            var endDate = DateTime.Now;
            var photos = _fixture.CreateMany<Photo>(50);
            var userFilter = new UserFilter(1);

            _photoData.Setup(m => m.GetPhotos(It.IsAny<UserFilter>(), It.IsAny<DateRange>(), 1, 50))
                .ReturnsAsync(photos);

            var results = await _photoService.GetPhotosByDate(new DateRange(startDate, endDate), null, 1, 50);

            _photoData.Verify(m => m.GetPhotos(It.IsAny<UserFilter>(), It.IsAny<DateRange>(), 1, 50),
                Times.Once);

            Assert.Equal(50, results.Count());
        }

        [Fact]
        public async Task GetTags()
        {
            var tags = _fixture.CreateMany<Tag>(10);

            _tagData.Setup(m => m.GetTags(It.IsAny<UserFilter>())).ReturnsAsync(tags);

            var results = await _photoService.GetTags(null);

            _tagData.Verify(m => m.GetTags(It.IsAny<UserFilter>()), Times.Once);

            Assert.Equal(tags.Count(), results.Count());
            Assert.IsType<Tag>(results.First());
        }

        [Fact]
        public async Task GetTagDoNotCreate()
        {
            var tagName = "B-Day";
            var tag = _fixture.Create<Tag>();
            tag.TagName = tagName;

            _tagData.Setup(m => m.GetTag(tagName, It.IsAny<int?>())).ReturnsAsync(tag);

            _tagData.Setup(m => m.SaveTag(It.IsAny<Tag>()));

            var result = await _photoService.GetTag(tagName, null, false);

            _tagData.Verify(m => m.GetTag(tagName, It.IsAny<int?>()),
                Times.Once);

            _tagData.Verify(m => m.SaveTag(It.IsAny<Tag>()),
                Times.Never);

            Assert.IsType<Tag>(result);
            Assert.Equal(tagName, result.TagName);
        }

        [Fact]
        public async Task GetTagCreateMissing()
        {
            var tagName = "B-Day";
            var tag = null as Tag;

            _tagData.Setup(m => m.GetTag(tagName, It.IsAny<int?>()))
                .ReturnsAsync(tag);

            _tagData.Setup(m => m.SaveTag(It.IsAny<Tag>()))
                .ReturnsAsync(new Tag { TagName = tagName });

            var result = await _photoService.GetTag(tagName, null, true);

            _tagData.Verify(m => m.GetTag(tagName, It.IsAny<int?>()),
                Times.Once);

            _tagData.Verify(m => m.SaveTag(It.IsAny<Tag>()),
                Times.Once);

            Assert.IsType<Tag>(result);
            Assert.Equal(tagName, result.TagName);
        }

        [Fact]
        public async Task SavePhoto()
        {
            var photo = _fixture.Create<Photo>();

            await _photoService.SavePhoto(photo);

            _photoData.Verify(m => m.SavePhoto(photo),
                Times.Once);
        }

        [Fact]
        public async Task AssociateTags()
        {
            var photo = _fixture.Create<Photo>();
            var tag = _fixture.Create<Tag>();

            var tags = _fixture.CreateMany<Tag>(3);

            _photoTagData.Setup(m => m.AssociatePhotoTag(It.IsAny<int>(), It.IsAny<int>()));

            _tagData.Setup(m => m.SaveTag(It.IsAny<Tag>()))
                .ReturnsAsync(tag);

            await _photoService.AssociateTags(photo, tags);

            _photoTagData.Verify(m => m.AssociatePhotoTag(It.IsAny<int>(), It.IsAny<int>()),
                Times.Exactly(3));
        }

        [Fact]
        public async Task FlagPhotosForReprocessingTest()
        {
            _photoData.Setup(m => m.FlagPhotosForReprocessing());

            await _photoService.FlagPhotosForReprocessing();

            _photoData.Verify(m => m.FlagPhotosForReprocessing(), Times.Once);
        }

        [Fact]
        public async Task DeletePhotoCache()
        {
            var completeInfo = new TaskCompleteInfo(TaskType.ClearCache, "wdavidsen", true);

            _photoData.Setup(m => m.DeletePhotos());
            _fileSystemService.Setup(m => m.DeleteDirectoryFiles(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()));

            await _photoService.ResetPhotosAndTags(completeInfo.ContextUserName);

            try
            {
                var token = new CancellationTokenSource().Token;
                var workItem = await _backgroundQueue.DequeueAsync(token);
                await workItem(token, new QueueEvents { ItemProcessed = (info) => { } });
            }
            catch (TaskCanceledException) { }

            _photoData.Verify(m => m.DeletePhotos(),
                Times.Once);
            _fileSystemService.Verify(m => m.DeleteDirectoryFiles(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()),
                Times.Once);
        }

        [Fact]
        public async Task DeletePhotoCacheEventFired()
        {
            var completeInfo = new TaskCompleteInfo(TaskType.ClearCache, "wdavidsen", true);

            _photoData.Setup(m => m.DeletePhotos());
            _fileSystemService.Setup(m => m.DeleteDirectoryFiles(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()));

            await _photoService.ResetPhotosAndTags(completeInfo.ContextUserName);

            var queueEvents = new Mock<IQueueEvents>();
            queueEvents.SetupGet(m => m.ItemProcessed).Returns((info) =>
            {
                Assert.Equal(completeInfo.Type, info.Type);
                Assert.Equal(completeInfo.Success, info.Success);
                Assert.Equal(completeInfo.ContextUserName, info.ContextUserName);
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
        public async Task DeletePhotoCacheHandlesException()
        {
            var completeInfo = new TaskCompleteInfo(TaskType.ClearCache, "wdavidsen", false);

            _photoData.Setup(m => m.DeletePhotos());

            _fileSystemService.Setup(m => m.DeleteDirectoryFiles(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Throws(new Exception("Some error"));

            /* _logger.Setup(m => m.Log<FormattedLogValues>(LogLevel.Error, 0, It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<Func<FormattedLogValues, Exception, string>>())); */

            await _photoService.ResetPhotosAndTags(completeInfo.ContextUserName);

            var queueEvents = new Mock<IQueueEvents>();
            queueEvents.SetupGet(m => m.ItemProcessed).Returns((info) =>
            {
                Assert.Equal(completeInfo.Type, info.Type);
                Assert.Equal(completeInfo.Success, info.Success);
                Assert.Equal(completeInfo.ContextUserName, info.ContextUserName);
            });

            try
            {
                var token = new CancellationTokenSource().Token;
                var workItem = await _backgroundQueue.DequeueAsync(token);
                await workItem(token, queueEvents.Object);
            }
            catch (TaskCanceledException) { }

            _photoData.Verify(m => m.DeletePhotos(),
                Times.Once);
            _fileSystemService.Verify(m => m.DeleteDirectoryFiles(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()),
                Times.Once);

            /* _logger.Verify(m => m.Log(It.IsAny<LogLevel>(), 0, It.IsAny<PhotoService>(), It.IsAny<Exception>(), It.IsAny<Func<PhotoService, Exception, string>>()),
                Times.Once); */

        }
    }
}
