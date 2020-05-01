using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Web.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SCS.HomePhotos.Web.Test.Controllers
{
    public class PhotoControllerTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly PhotosController _photosController;
        private readonly Mock<ILogger<PhotosController>> _logger;
        private readonly Mock<IPhotoService> _photosService;

        public PhotoControllerTests()
        {
            _logger = new Mock<ILogger<PhotosController>>();
            _photosService = new Mock<IPhotoService>();

            _photosController = new PhotosController(_logger.Object, _photosService.Object);
        }

        [Fact]
        public async Task GetLatestDefault()
        {
            var photos = _fixture.CreateMany<Model.Photo>(200);

            _photosService.Setup(m => m.GetLatestPhotos(1, 200))
                .ReturnsAsync(photos);

            var response = await _photosController.GetLatestPhotos().ConfigureAwait(true);

            Assert.IsType<OkObjectResult>(response);

            _photosService.Verify(m => m.GetLatestPhotos(1, 200),
                Times.Once);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<List<Dto.Photo>>(value);

            var dtos = value as List<Dto.Photo>;

            Assert.Equal(photos.Count(), dtos.Count);
        }
    }
}
