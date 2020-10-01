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
        private readonly Fixture _fixture;

        private readonly PhotosController _photosController;
        private readonly Mock<ILogger<PhotosController>> _logger;
        private readonly Mock<IPhotoService> _photosService;

        public PhotoControllerTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _logger = new Mock<ILogger<PhotosController>>();
            _photosService = new Mock<IPhotoService>();

            _photosController = new PhotosController(_logger.Object, _photosService.Object);
        }

        [Fact]
        public async Task GetLatestDefault()
        {
            const int photoCount = 400;
            var photos = _fixture.CreateMany<Model.Photo>(photoCount);

            _photosService.Setup(m => m.GetLatestPhotos(1, photoCount))
                .ReturnsAsync(photos);

            var response = await _photosController.GetLatestPhotos().ConfigureAwait(true);

            Assert.IsType<OkObjectResult>(response);

            _photosService.Verify(m => m.GetLatestPhotos(1, photoCount),
                Times.Once);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<List<Dto.Photo>>(value);

            var dtos = value as List<Dto.Photo>;

            Assert.Equal(photos.Count(), dtos.Count);
        }
    }
}
