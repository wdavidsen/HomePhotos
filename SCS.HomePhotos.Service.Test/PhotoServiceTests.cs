using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SCS.HomePhotos.Data;
using SCS.HomePhotos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SCS.HomePhotos.Service.Test
{
    public class PhotoServiceTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly PhotoService _photoService;
        private readonly Mock<IPhotoData> _photoData;
        private readonly Mock<ITagData> _tagData;
        private readonly Mock<ILogger<PhotoService>> _logger;

        public PhotoServiceTests()
        {
            _photoData = new Mock<IPhotoData>();
            _tagData = new Mock<ITagData>();
            _logger = new Mock<ILogger<PhotoService>>();

            _photoService = new PhotoService(_photoData.Object, _tagData.Object, _logger.Object, null, null, null);
        }

        [Fact]
        public async Task GetPhotoByChecksum()
        {
            var photos = _fixture.CreateMany<Photo>(1);
            var checksum = _fixture.Create<string>();

            _photoData.Setup(m => m.GetListAsync<Photo>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(photos)
                .Callback<string, object>((where, p) => {
                    Assert.StartsWith("WHERE ", where);
                    Assert.Equal(checksum, p.GetProperty("Checksum").ToString());
                });

            var result = await _photoService.GetPhotoByChecksum(checksum);

            _photoData.Verify(m => m.GetListAsync<Photo>(It.IsAny<string>(), It.IsAny<object>()),
                Times.Once);

            Assert.Equal(photos.First(), result);
        }

        [Fact]
        public async Task GetLatestPhotos()
        {
            var photos = _fixture.CreateMany<Photo>(50);

            _photoData.Setup(m => m.GetPhotos(DateTime.MinValue, DateTime.Today, true, 1, 50))
                .ReturnsAsync(photos);

            var results = await _photoService.GetLatestPhotos(1, 50);

            _photoData.Verify(m => m.GetPhotos(DateTime.MinValue, DateTime.Today, true, 1, 50),
                Times.Once);

            Assert.True(results.Count() == 50);
        }

        [Fact]
        public async Task GetPhotosByTag()
        {
            var photos = _fixture.CreateMany<Photo>(50);
            var tags = _fixture.CreateMany<string>(3);

            _photoData.Setup(m => m.GetPhotos(It.IsAny<string[]>(), 1, 50))
                .ReturnsAsync(photos)
                .Callback<string[], int, int>((t, p, s) => {
                    Assert.Equal(1, p);
                    Assert.Equal(50, s);
                    Assert.Equal(tags.Count(), t.Count());
                });

            var results = await _photoService.GetPhotosByTag(tags.ToArray(), 1, 50);

            _photoData.Verify(m => m.GetPhotos(It.IsAny<string[]>(), 1, 50),
                Times.Once);

            Assert.True(results.Count() == 50);
        }

        [Fact]
        public async Task GetPhotosByDateTaken()
        {
            var startDate = DateTime.Now.AddDays(-7);
            var endDate = DateTime.Now;
            var photos = _fixture.CreateMany<Photo>(50);

            _photoData.Setup(m => m.GetPhotos(startDate, endDate, false, 1, 50))
                .ReturnsAsync(photos);

            var results = await _photoService.GetPhotosByDateTaken(startDate, endDate, 1, 50);

            _photoData.Verify(m => m.GetPhotos(startDate, endDate, false, 1, 50),
                Times.Once);

            Assert.True(results.Count() == 50);
        }

        [Fact]
        public async Task GetTags()
        {
            var tags = _fixture.CreateMany<Tag>(10);

            _tagData.Setup(m => m.GetTags()).ReturnsAsync(tags);

            var results = await _photoService.GetTags(false);

            _tagData.Verify(m => m.GetTags(), Times.Once);

            Assert.Equal(tags.Count(), results.Count());
            Assert.IsType<Tag>(results.First());
        }

        [Fact]
        public async Task GetTagDoNotCreate()
        {
            var tag = "B-Day";
            var tags = new List<Tag> { new Tag { TagName = tag } };

           _tagData.Setup(m => m.GetListAsync<Tag>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(tags)
                .Callback<string, object>((where, p) => {
                    Assert.Equal("WHERE TagName = @TagName", where);
                    Assert.Equal(tag, p.GetProperty("TagName").ToString());
                });
            _tagData.Setup(m => m.SaveTag(It.IsAny<Tag>()));

            var result = await _photoService.GetTag(tag, false);

            _tagData.Verify(m => m.GetListAsync<Tag>(It.IsAny<string>(), It.IsAny<object>()),
                Times.Once);
            _tagData.Verify(m => m.SaveTag(It.IsAny<Tag>()),
                Times.Never);

            Assert.IsType<Tag>(result);
            Assert.Equal(tag, result.TagName);
        }

        [Fact]
        public async Task GetTagCreateMissing()
        {
            var tag = "B-Day";
            var tags = new List<Tag>();

            _tagData.Setup(m => m.GetListAsync<Tag>(It.IsAny<string>(), It.IsAny<object>()))
                 .ReturnsAsync(tags)
                 .Callback<string, object>((where, p) => {
                     Assert.Equal("WHERE TagName = @TagName", where);
                     Assert.Equal(tag, p.GetProperty("TagName").ToString());
                 });
            _tagData.Setup(m => m.SaveTag(It.IsAny<Tag>()))
                .ReturnsAsync(new Tag { TagName = tag });

            var result = await _photoService.GetTag(tag, true);

            _tagData.Verify(m => m.GetListAsync<Tag>(It.IsAny<string>(), It.IsAny<object>()),
                Times.Once);
            _tagData.Verify(m => m.SaveTag(It.IsAny<Tag>()),
                Times.Once);

            Assert.IsType<Tag>(result);
            Assert.Equal(tag, result.TagName);
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
            var tags = _fixture.CreateMany<Tag>(3);

            _tagData.Setup(m => m.GetListAsync<Tag>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(tags);

            _tagData.Setup(m => m.AssociatePhotoTag(It.IsAny<int>(), It.IsAny<int>()));

            await _photoService.AssociateTags(photo, tags.Select(t => t.TagName).ToArray());

            _tagData.Verify(m => m.GetListAsync<Tag>(It.IsAny<string>(), It.IsAny<object>()),
                Times.Exactly(3));
            _tagData.Verify(m => m.AssociatePhotoTag(It.IsAny<int>(), It.IsAny<int>()),
                Times.Exactly(3));
        }
    }
}
