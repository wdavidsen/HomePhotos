using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Web.Controllers;
using SCS.HomePhotos.Web.Test.Mocks;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace SCS.HomePhotos.Web.Test.Controllers
{
    public class UploadControllerTests : ControllerTestBase
    {
        private readonly UploadController _uploadController;

        private readonly Mock<ILogger<UploadController>> _logger;
        private readonly Mock<IImageService> _imageService;
        private readonly Mock<IFileUploadService> _fileUploadService;
        private readonly Mock<IAdminLogService> _adminLogService;
        private readonly Mock<IUploadTracker> _uploadTracker;

        public UploadControllerTests()
        {
            _logger = new Mock<ILogger<UploadController>>();
            _imageService = new Mock<IImageService>();
            _fileUploadService = new Mock<IFileUploadService>();
            _adminLogService = new Mock<IAdminLogService>();
            _uploadTracker = new Mock<IUploadTracker>();

            _uploadController = new UploadController(_logger.Object, _imageService.Object, _fileUploadService.Object, _adminLogService.Object, _uploadTracker.Object);
        }

        [Fact]
        public async Task ImageUpload()
        {
            var userName = "wdavidsen";
            var formCollecton = new MockFormCollection();
            var tags = new string[] { "tag1", "tag2", "wdavidsen Upload" };

            SetControllerContext(_uploadController, "POST", userName, formCollecton);

            var cachePath = "c1/A2A44CAE-2EC8-4610-BA4D-6995878B1183.jpg";

            _fileUploadService.Setup(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create));
            _imageService.Setup(m => m.QueueMobileResize(It.IsAny<string>(), It.IsAny<string>(), false, tags))
                .ReturnsAsync(cachePath);

            _uploadTracker.Setup(m => m.AddUpload(userName, It.IsAny<string>()));

            var response = await _uploadController.ImageUpload(formCollecton).ConfigureAwait(true);

            Assert.IsType<OkResult>(response);

            _fileUploadService.Verify(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create),
                Times.Once);

            _imageService.Verify(m => m.QueueMobileResize(It.IsAny<string>(), It.IsAny<string>(), false, tags),
                Times.Once);

            _uploadTracker.Verify(m => m.AddUpload(userName, It.IsAny<string>()),
                Times.Once);
        }

        protected override void DisposeController()
        {
            _uploadController.Dispose();
        }
    }
}
