using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Web.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SCS.HomePhotos.Web.Test.Controllers
{
    public class TagControllerTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly TagsController _tagsController;
        private readonly Mock<ILogger<TagsController>> _logger;
        private readonly Mock<IPhotoService> _photosService;

        public TagControllerTests()
        {
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
    }
}
