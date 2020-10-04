using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Web.Controllers;
using SCS.HomePhotos.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SCS.HomePhotos.Web.Test.Controllers
{
    public class TagsControllerTests : ControllerTestBase
    {
        private readonly Fixture _fixture;

        private readonly TagsController _tagsController;
        private readonly Mock<ILogger<TagsController>> _logger;
        private readonly Mock<IPhotoService> _photosService;

        public TagsControllerTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _logger = new Mock<ILogger<TagsController>>();
            _photosService = new Mock<IPhotoService>();

            _tagsController = new TagsController(_logger.Object, _photosService.Object);
        }

        [Fact]
        public async Task Get()
        {
            var tags = _fixture.CreateMany<Tag>(10);

            _photosService.Setup(m => m.GetTags(true)).ReturnsAsync(tags);

            var response = await _tagsController.Get();

            _photosService.Verify(m => m.GetTags(true),
                Times.Once);

            Assert.IsType<OkObjectResult>(response);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<List<Dto.Tag>>(value);

            var dtos = value as List<Dto.Tag>;

            Assert.Equal(tags.Count(), dtos.Count);
        }

        [Fact]
        public async Task SearchTags()
        {
            var tags = _fixture.CreateMany<Tag>(10);
            var keywords = "birthday party";

            _photosService.Setup(m => m.GetTagsByKeywords(keywords, 1, 200))
                .ReturnsAsync(tags);

            var response = await _tagsController.SearchTags(keywords, 1, 200);

            _photosService.Verify(m => m.GetTagsByKeywords(keywords, 1, 200),
                Times.Once);

            Assert.IsType<OkObjectResult>(response);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<List<Dto.Tag>>(value);

            var dtos = value as List<Dto.Tag>;

            Assert.Equal(tags.Count(), dtos.Count);
        }

        [Fact]
        public async Task MergeTags()
        {
            var tag = _fixture.Create<Tag>();
            var newName = tag.TagName;
            var mergeInfo = new TagMergeInfo { NewTagName = newName, SourceTagIds = new int[] { 1, 2, 3 } };

            _photosService.Setup(m => m.MergeTags(mergeInfo.NewTagName, mergeInfo.SourceTagIds))
                .ReturnsAsync(tag);

            var response = await _tagsController.MergeTags(mergeInfo);

            _photosService.Verify(m => m.MergeTags(mergeInfo.NewTagName, mergeInfo.SourceTagIds),
                Times.Once);

            Assert.IsType<OkObjectResult>(response);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<Dto.Tag>(value);

            var dto = value as Dto.Tag;

            Assert.Equal(tag.TagName, dto.TagName);
            Assert.Equal(tag.TagId, dto.TagId);
        }

        [Fact]
        public async Task MergeTagsInvalid()
        {
            var tag = _fixture.Create<Tag>();
            var newName = tag.TagName;
            var mergeInfo = new TagMergeInfo { NewTagName = newName, SourceTagIds = new int[] { 1, 2, 3 } };

            _photosService.Setup(m => m.MergeTags(mergeInfo.NewTagName, mergeInfo.SourceTagIds))
               .ReturnsAsync(tag);

            _tagsController.ModelState.AddModelError("key", "error");

            _photosService.Verify(m => m.MergeTags(mergeInfo.NewTagName, mergeInfo.SourceTagIds),
               Times.Never);

            var response = await _tagsController.MergeTags(mergeInfo);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task CopyTags()
        {
            var tag = _fixture.Create<Tag>();
            var newName = tag.TagName;
            var mergeInfo = new TagCopyInfo { NewTagName = newName, SourceTagId = 1 };

            _photosService.Setup(m => m.CopyTags(mergeInfo.NewTagName, mergeInfo.SourceTagId))
                .ReturnsAsync(tag);

            var response = await _tagsController.CopyTags(mergeInfo);

            _photosService.Verify(m => m.CopyTags(mergeInfo.NewTagName, mergeInfo.SourceTagId),
                Times.Once);

            Assert.IsType<OkObjectResult>(response);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<Dto.Tag>(value);

            var dto = value as Dto.Tag;

            Assert.Equal(tag.TagName, dto.TagName);
            Assert.Equal(tag.TagId, dto.TagId);
        }

        [Fact]
        public async Task CopyTagsInvalid()
        {
            var tag = _fixture.Create<Tag>();
            var newName = tag.TagName;
            var mergeInfo = new TagCopyInfo { NewTagName = newName, SourceTagId = 1 };

            _photosService.Setup(m => m.CopyTags(mergeInfo.NewTagName, mergeInfo.SourceTagId))
                .ReturnsAsync(tag);

            _tagsController.ModelState.AddModelError("key", "error");

            _photosService.Verify(m => m.CopyTags(mergeInfo.NewTagName, mergeInfo.SourceTagId),
                Times.Never);

            var response = await _tagsController.CopyTags(mergeInfo);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task GetPhotosToTag()
        {
            var photoIds = _fixture.CreateMany<int>(5).ToArray();
            var tags = _fixture.CreateMany<Tag>(5);

            _photosService.Setup(m => m.GetTagsAndPhotos(photoIds))
                .ReturnsAsync(tags);

            var response = await _tagsController.GetPhotosToTag(photoIds);

            _photosService.Verify(m => m.GetTagsAndPhotos(photoIds),
                Times.Once);

            Assert.IsType<OkObjectResult>(response);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<BatchSelectTags>(value);

            var dto = value as BatchSelectTags;

            Assert.Equal(tags.Count(), dto.Tags.Count);
        }

        [Fact]
        public async Task GetPhotosToTagInvalid()
        {
            var photoIds = _fixture.CreateMany<int>(5).ToArray();
            var tags = _fixture.CreateMany<Tag>(5);

            _photosService.Setup(m => m.GetTagsAndPhotos(photoIds))
                .ReturnsAsync(tags);

            _tagsController.ModelState.AddModelError("key", "error");

            var response = await _tagsController.GetPhotosToTag(photoIds);

            _photosService.Verify(m => m.GetTagsAndPhotos(photoIds),
                Times.Never);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task UpdatePhotoTags()
        {
            var photoIds = _fixture.CreateMany<int>(5);
            var tagStates = _fixture.CreateMany<TagState>(5);
            var batchUpdate = new BatchUpdateTags
            {
                PhotoIds = photoIds.ToList(),
                TagStates = tagStates.ToList()
            };
            
            _photosService.Setup(m => m.UpdatePhotoTags(It.IsAny<List<int>>(), It.IsAny<List<string>>(), It.IsAny<List<int>>()));

            var response = await _tagsController.TagPhotos(batchUpdate);

            _photosService.Verify(m => m.UpdatePhotoTags(It.IsAny<List<int>>(), It.IsAny<List<string>>(), It.IsAny<List<int>>()),
                Times.Once);

            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task UpdatePhotoTagsInvalid()
        {
            var photoIds = _fixture.CreateMany<int>(5);
            var tagStates = _fixture.CreateMany<TagState>(5);
            var batchUpdate = new BatchUpdateTags
            {
                PhotoIds = photoIds.ToList(),
                TagStates = tagStates.ToList()
            };

            _photosService.Setup(m => m.UpdatePhotoTags(It.IsAny<List<int>>(), It.IsAny<List<string>>(), It.IsAny<List<int>>()));

            _tagsController.ModelState.AddModelError("key", "message");

            var response = await _tagsController.TagPhotos(batchUpdate);

            _photosService.Verify(m => m.UpdatePhotoTags(It.IsAny<List<int>>(), It.IsAny<List<string>>(), It.IsAny<List<int>>()),
                Times.Never);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task AddTag()
        {
            var tag = _fixture.Create<Tag>();
            var dto = new Dto.Tag(tag);

            _photosService.Setup(m => m.SaveTag(It.IsAny<Tag>()))
                .ReturnsAsync(tag);

            var response = await _tagsController.AddTag(dto);

            _photosService.Verify(m => m.SaveTag(It.IsAny<Tag>()),
                Times.Once);

            Assert.IsType<OkObjectResult>(response);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<Dto.Tag>(value);

            var dto2 = value as Dto.Tag;

            Assert.Equal(dto.TagName, dto2.TagName);
            Assert.Equal(dto.TagId, dto2.TagId);
        }

        [Fact]
        public async Task AddTagInvalid()
        {
            var tag = _fixture.Create<Tag>();
            var dto = new Dto.Tag(tag);

            _photosService.Setup(m => m.SaveTag(It.IsAny<Tag>()))
                .ReturnsAsync(tag);

            _tagsController.ModelState.AddModelError("key", "message");

            var response = await _tagsController.AddTag(dto);

            _photosService.Verify(m => m.SaveTag(It.IsAny<Tag>()),
                Times.Never);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task AddTagInvalidHandlesEx()
        {
            var tag = _fixture.Create<Tag>();
            var dto = new Dto.Tag(tag);

            _photosService.Setup(m => m.SaveTag(It.IsAny<Tag>()))
                .Throws(new InvalidOperationException("Some message"));

            var response = await _tagsController.AddTag(dto);

            _photosService.Verify(m => m.SaveTag(It.IsAny<Tag>()),
                Times.Once);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task UpdateTag()
        {
            var tag = _fixture.Create<Tag>();
            var dto = new Dto.Tag(tag);

            _photosService.Setup(m => m.SaveTag(It.IsAny<Tag>()))
                .ReturnsAsync(tag);

            var response = await _tagsController.UpdateTag(dto);

            _photosService.Verify(m => m.SaveTag(It.IsAny<Tag>()),
                Times.Once);

            Assert.IsType<OkObjectResult>(response);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<Dto.Tag>(value);

            var dto2 = value as Dto.Tag;

            Assert.Equal(dto.TagName, dto2.TagName);
            Assert.Equal(dto.TagId, dto2.TagId);
        }

        [Fact]
        public async Task UpdateTagInvalid()
        {
            var tag = _fixture.Create<Tag>();
            var dto = new Dto.Tag(tag);

            _photosService.Setup(m => m.SaveTag(It.IsAny<Tag>()))
                .ReturnsAsync(tag);

            _tagsController.ModelState.AddModelError("key", "message");

            var response = await _tagsController.UpdateTag(dto);

            _photosService.Verify(m => m.SaveTag(It.IsAny<Tag>()),
                Times.Never);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task UpdateTagInvalidHandlesEx()
        {
            var tag = _fixture.Create<Tag>();
            var dto = new Dto.Tag(tag);

            _photosService.Setup(m => m.SaveTag(It.IsAny<Tag>()))
                .Throws(new InvalidOperationException("Some message"));

            var response = await _tagsController.UpdateTag(dto);

            _photosService.Verify(m => m.SaveTag(It.IsAny<Tag>()),
                Times.Once);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task DeleteTag()
        {
            var tagId = _fixture.Create<int>();

            _photosService.Setup(m => m.DeleteTag(tagId));

            var response = await _tagsController.DeleteTag(tagId);

            _photosService.Verify(m => m.DeleteTag(tagId),
                Times.Once);

            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task DeleteTagInvalid()
        {
            var tagId = _fixture.Create<int>();

            _photosService.Setup(m => m.DeleteTag(tagId));

            _tagsController.ModelState.AddModelError("key", "message");

            var response = await _tagsController.DeleteTag(tagId);

            _photosService.Verify(m => m.DeleteTag(tagId),
                Times.Never);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task DeleteTagInvalidHandlesEx()
        {
            var tagId = _fixture.Create<int>();

            _photosService.Setup(m => m.DeleteTag(tagId))
                .Throws(new InvalidOperationException("Some message"));

            var response = await _tagsController.DeleteTag(tagId);

            _photosService.Verify(m => m.DeleteTag(tagId),
                Times.Once);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        protected override void DisposeController()
        {
            _tagsController.Dispose();
        }
    }
}
