using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Web.Controllers;
using SCS.HomePhotos.Web.Models;
using SCS.HomePhotos.Web.Test.Mocks;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SCS.HomePhotos.Web.Test.Controllers
{
    public class UploadControllerTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly UploadController _uploadController;

        private readonly Mock<ILogger<UploadController>> _logger;
        private readonly Mock<IImageService> _imageService;
        private readonly Mock<IFileUploadService> _fileUploadService;

        public UploadControllerTests()
        {
            _logger = new Mock<ILogger<UploadController>>();
            _imageService = new Mock<IImageService>();
            _fileUploadService = new Mock<IFileUploadService>();

            _uploadController = new UploadController(_logger.Object, _imageService.Object, _fileUploadService.Object);
        }

        [Fact]
        public async Task ImageUpload()
        {
            var files = _fixture.CreateMany<MockFormFile>(1).Cast<IFormFile>().ToList();
            var cachePath = "c1/A2A44CAE-2EC8-4610-BA4D-6995878B1183.jpg";

            _fileUploadService.Setup(m => m.CopyFile(files[0], It.IsAny<string>(), FileMode.Create));
            _imageService.Setup(m => m.QueueMobileResize(It.IsAny<string>(), false))
                .ReturnsAsync(cachePath);

            var response = await _uploadController.ImageUpload(files).ConfigureAwait(true);

            Assert.IsType<OkObjectResult>(response);

            _fileUploadService.Verify(m => m.CopyFile(files[0], It.IsAny<string>(), FileMode.Create),
                Times.Once);

            _imageService.Verify(m => m.QueueMobileResize(It.IsAny<string>(), false),
                Times.Once);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<FileGroup<FileProcessed>>(value);

            var fileGroup = value as FileGroup<FileProcessed>;

            Assert.True(fileGroup.Files.Count == 1);
            Assert.Equal(files.Count, fileGroup.Files.Count);
            Assert.Equal(files[0].GetFileName(), fileGroup.Files[0].OriginalName);
            Assert.Equal(Path.GetFileName(cachePath), fileGroup.Files[0].Name);
            Assert.Equal($"{Constants.CacheRoute}/{cachePath}", fileGroup.Files[0].ThumbnailUrl);
        }
    }
}
