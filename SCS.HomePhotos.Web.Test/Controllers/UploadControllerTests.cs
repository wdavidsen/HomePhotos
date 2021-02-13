using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Service.Contracts;
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
            var fileName = "Whale Shark.jpg";
            var formCollecton = new MockFormCollection(new MockFormFile(fileName), "tag1", "tag2");
            var tags = new string[] { "tag1", "tag2", "wdavidsen Upload" };

            SetControllerContext(_uploadController, "POST", userName, formCollecton);

            var cachePath = "c1/A2A44CAE-2EC8-4610-BA4D-6995878B1183.jpg";

            _fileUploadService.Setup(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create));
            _imageService.Setup(m => m.QueueMobileResize(userName, It.IsAny<string>(), tags))
                .ReturnsAsync(cachePath)
                .Callback<string, string, string[]>((contextUserName, imageFilePath, tags) =>
                {
                    Assert.Equal(fileName, Path.GetFileName(imageFilePath));
                });

            _uploadTracker.Setup(m => m.AddUpload(userName, It.IsAny<string>()))
                .Callback<string, string>((userName, filePath) =>
                {
                    Assert.Equal(fileName, Path.GetFileName(filePath));
                });

            var response = await _uploadController.ImageUpload(formCollecton).ConfigureAwait(true);

            Assert.IsType<OkResult>(response);

            _fileUploadService.Verify(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create),
                Times.Once);

            _imageService.Verify(m => m.QueueMobileResize(It.IsAny<string>(), It.IsAny<string>(), tags),
                Times.Once);

            _uploadTracker.Verify(m => m.AddUpload(userName, It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task ImageUploadInvalidFileName()
        {
            var userName = "wdavidsen";
            var formCollecton = new MockFormCollection(new MockFormFile("** Whale ?? Shark ::.jpg"));
            var tags = new string[] { "tag1", "tag2", "wdavidsen Upload" };

            SetControllerContext(_uploadController, "POST", userName, formCollecton);

            _fileUploadService.Setup(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create));
            _imageService.Setup(m => m.QueueMobileResize(It.IsAny<string>(), It.IsAny<string>(), tags));

            _uploadTracker.Setup(m => m.AddUpload(userName, It.IsAny<string>()));

            var response = await _uploadController.ImageUpload(formCollecton).ConfigureAwait(true);

            Assert.IsType<BadRequestResult>(response);

            _fileUploadService.Verify(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create),
                Times.Never);

            _imageService.Verify(m => m.QueueMobileResize(It.IsAny<string>(), It.IsAny<string>(), tags),
                Times.Never);

            _uploadTracker.Verify(m => m.AddUpload(userName, It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ImageUploadInvalidExtension()
        {
            var userName = "wdavidsen";
            var formCollecton = new MockFormCollection(new MockFormFile("Whale Shark.webp"));
            var tags = new string[] { "tag1", "tag2", "wdavidsen Upload" };

            SetControllerContext(_uploadController, "POST", userName, formCollecton);

            _fileUploadService.Setup(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create));
            _imageService.Setup(m => m.QueueMobileResize(It.IsAny<string>(), It.IsAny<string>(), tags));

            _uploadTracker.Setup(m => m.AddUpload(userName, It.IsAny<string>()));

            var response = await _uploadController.ImageUpload(formCollecton).ConfigureAwait(true);

            Assert.IsType<BadRequestResult>(response);

            _fileUploadService.Verify(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create),
                Times.Never);

            _imageService.Verify(m => m.QueueMobileResize(It.IsAny<string>(), It.IsAny<string>(), tags),
                Times.Never);

            _uploadTracker.Verify(m => m.AddUpload(userName, It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ImageUploadImageImposter()
        {
            var userName = "wdavidsen";
            var formCollecton = new MockFormCollection(new MockFormFile("Whale Shark BMP.jpg"));
            var tags = new string[] { "tag1", "tag2", "wdavidsen Upload" };

            SetControllerContext(_uploadController, "POST", userName, formCollecton);

            _fileUploadService.Setup(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create));
            _imageService.Setup(m => m.QueueMobileResize(It.IsAny<string>(), It.IsAny<string>(), tags));

            _uploadTracker.Setup(m => m.AddUpload(userName, It.IsAny<string>()));

            var response = await _uploadController.ImageUpload(formCollecton).ConfigureAwait(true);

            Assert.IsType<BadRequestResult>(response);

            _fileUploadService.Verify(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create),
                Times.Never);

            _imageService.Verify(m => m.QueueMobileResize(It.IsAny<string>(), It.IsAny<string>(), tags),
                Times.Never);

            _uploadTracker.Verify(m => m.AddUpload(userName, It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ImageUploadFilePathTooLong()
        {
            var userName = "wdavidsen";
            var formCollecton = new MockFormCollection(new MockFormFile($"{new string('W', 256)}.jpg"));
            var tags = new string[] { "tag1", "tag2", "wdavidsen Upload" };

            SetControllerContext(_uploadController, "POST", userName, formCollecton);

            _fileUploadService.Setup(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create));
            _imageService.Setup(m => m.QueueMobileResize(It.IsAny<string>(), It.IsAny<string>(), tags));
            _uploadTracker.Setup(m => m.AddUpload(userName, It.IsAny<string>()));

            var response = await _uploadController.ImageUpload(formCollecton).ConfigureAwait(true);

            Assert.IsType<BadRequestResult>(response);

            _fileUploadService.Verify(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create),
                Times.Never);

            _imageService.Verify(m => m.QueueMobileResize(It.IsAny<string>(), It.IsAny<string>(), tags),
                Times.Never);

            _uploadTracker.Verify(m => m.AddUpload(userName, It.IsAny<string>()),
                Times.Never);
        }

        protected override void DisposeController()
        {
            _uploadController.Dispose();
        }
    }
}
