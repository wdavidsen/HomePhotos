using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Web.Controllers;
using SCS.HomePhotos.Web.Test.Mocks;

using System.Collections.Generic;
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
        private readonly Mock<IUserData> _userData;
        private readonly Mock<IFileUploadService> _fileUploadService;
        private readonly Mock<IAdminLogService> _adminLogService;
        private readonly Mock<IUploadTracker> _uploadTracker;

        public UploadControllerTests()
        {
            _logger = new Mock<ILogger<UploadController>>();
            _imageService = new Mock<IImageService>();
            _userData = new Mock<IUserData>();
            _fileUploadService = new Mock<IFileUploadService>();
            _adminLogService = new Mock<IAdminLogService>();
            _uploadTracker = new Mock<IUploadTracker>();

            _uploadController = new UploadController(_logger.Object, _imageService.Object, _userData.Object, _fileUploadService.Object, _adminLogService.Object, _uploadTracker.Object);
        }

        [Fact]
        public async Task ImageUpload()
        {
            var fileName = "Whale Shark.jpg";

            var user = new Model.User { UserName = "wdavidsen", UserId = 1 };

            var formCollecton = new MockFormCollection(new MockFormFile(fileName), "tag1", "tag2");

            var tags = new List<Model.Tag>
            {
                new Model.Tag { TagName = "tag1" },
                new Model.Tag { TagName = "tag2" },
                new Model.Tag { TagName = "wdavidsen Upload" }
            };

            SetControllerContext(_uploadController, "POST", user.UserName, formCollecton);

            var cachePath = "c1/A2A44CAE-2EC8-4610-BA4D-6995878B1183.jpg";

            _fileUploadService.Setup(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create));
            _imageService.Setup(m => m.QueueMobileResize(user, It.IsAny<string>(), tags))
                .ReturnsAsync(cachePath)
                .Callback<string, string, string[]>((contextUserName, imageFilePath, tags) =>
                {
                    Assert.Equal(fileName, Path.GetFileName(imageFilePath));
                });

            _uploadTracker.Setup(m => m.AddUpload(user.UserName, It.IsAny<string>()))
                .Callback<string, string>((userName, filePath) =>
                {
                    Assert.Equal(fileName, Path.GetFileName(filePath));
                });

            var response = await _uploadController.ImageUpload(formCollecton).ConfigureAwait(true);

            Assert.IsType<OkResult>(response);

            _fileUploadService.Verify(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create),
                Times.Once);

            _imageService.Verify(m => m.QueueMobileResize(It.IsAny<Model.User>(), It.IsAny<string>(), tags),
                Times.Once);

            _uploadTracker.Verify(m => m.AddUpload(user.UserName, It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task ImageUploadInvalidFileName()
        {
            var formCollecton = new MockFormCollection(new MockFormFile("** Whale ?? Shark ::.jpg"));

            var user = new Model.User { UserName = "wdavidsen", UserId = 1 };

            var tags = new List<Model.Tag>
            {
                new Model.Tag { TagName = "tag1" },
                new Model.Tag { TagName = "tag2" },
                new Model.Tag { TagName = "wdavidsen Upload" }
            };

            SetControllerContext(_uploadController, "POST", user.UserName, formCollecton);

            _fileUploadService.Setup(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create));
            _imageService.Setup(m => m.QueueMobileResize(It.IsAny<Model.User>(), It.IsAny<string>(), tags));

            _uploadTracker.Setup(m => m.AddUpload(user.UserName, It.IsAny<string>()));

            var response = await _uploadController.ImageUpload(formCollecton).ConfigureAwait(true);

            Assert.IsType<BadRequestResult>(response);

            _fileUploadService.Verify(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create),
                Times.Never);

            _imageService.Verify(m => m.QueueMobileResize(It.IsAny<Model.User>(), It.IsAny<string>(), tags),
                Times.Never);

            _uploadTracker.Verify(m => m.AddUpload(user.UserName, It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ImageUploadInvalidExtension()
        {
            var formCollecton = new MockFormCollection(new MockFormFile("Whale Shark.webp"));
         
            var user = new Model.User { UserName = "wdavidsen", UserId = 1 };
            
            var tags = new List<Model.Tag>
            {
                new Model.Tag { TagName = "tag1" },
                new Model.Tag { TagName = "tag2" },
                new Model.Tag { TagName = "wdavidsen Upload" }
            };

            SetControllerContext(_uploadController, "POST", user.UserName, formCollecton);

            _fileUploadService.Setup(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create));
            _imageService.Setup(m => m.QueueMobileResize(It.IsAny<Model.User>(), It.IsAny<string>(), tags));

            _uploadTracker.Setup(m => m.AddUpload(user.UserName, It.IsAny<string>()));

            var response = await _uploadController.ImageUpload(formCollecton).ConfigureAwait(true);

            Assert.IsType<BadRequestResult>(response);

            _fileUploadService.Verify(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create),
                Times.Never);

            _imageService.Verify(m => m.QueueMobileResize(It.IsAny<Model.User>(), It.IsAny<string>(), tags),
                Times.Never);

            _uploadTracker.Verify(m => m.AddUpload(user.UserName, It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ImageUploadImageImposter()
        {
            var formCollecton = new MockFormCollection(new MockFormFile("Whale Shark BMP.jpg"));

            var user = new Model.User { UserName = "wdavidsen", UserId = 1 };

            var tags = new List<Model.Tag>
            {
                new Model.Tag { TagName = "tag1" },
                new Model.Tag { TagName = "tag2" },
                new Model.Tag { TagName = "wdavidsen Upload" }
            };

            SetControllerContext(_uploadController, "POST", user.UserName, formCollecton);

            _fileUploadService.Setup(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create));
            _imageService.Setup(m => m.QueueMobileResize(It.IsAny<Model.User>(), It.IsAny<string>(), tags));

            _uploadTracker.Setup(m => m.AddUpload(user.UserName, It.IsAny<string>()));

            var response = await _uploadController.ImageUpload(formCollecton).ConfigureAwait(true);

            Assert.IsType<BadRequestResult>(response);

            _fileUploadService.Verify(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create),
                Times.Never);

            _imageService.Verify(m => m.QueueMobileResize(It.IsAny<Model.User>(), It.IsAny<string>(), tags),
                Times.Never);

            _uploadTracker.Verify(m => m.AddUpload(user.UserName, It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ImageUploadFilePathTooLong()
        {
            var formCollecton = new MockFormCollection(new MockFormFile($"{new string('W', 256)}.jpg"));

            var user = new Model.User { UserName = "wdavidsen", UserId = 1 };

            var tags = new List<Model.Tag>
            {
                new Model.Tag { TagName = "tag1" },
                new Model.Tag { TagName = "tag2" },
                new Model.Tag { TagName = "wdavidsen Upload" }
            };

            SetControllerContext(_uploadController, "POST", user.UserName, formCollecton);

            _fileUploadService.Setup(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create));
            _imageService.Setup(m => m.QueueMobileResize(It.IsAny<Model.User>(), It.IsAny<string>(), tags));
            _uploadTracker.Setup(m => m.AddUpload(user.UserName, It.IsAny<string>()));

            var response = await _uploadController.ImageUpload(formCollecton).ConfigureAwait(true);

            Assert.IsType<BadRequestResult>(response);

            _fileUploadService.Verify(m => m.CopyFile(It.IsAny<IFormFile>(), It.IsAny<string>(), FileMode.Create),
                Times.Never);

            _imageService.Verify(m => m.QueueMobileResize(It.IsAny<Model.User>(), It.IsAny<string>(), tags),
                Times.Never);

            _uploadTracker.Verify(m => m.AddUpload(user.UserName, It.IsAny<string>()),
                Times.Never);
        }

        protected override void Dispose(bool disposing)
        {
            _uploadController.Dispose();
        }
    }
}
