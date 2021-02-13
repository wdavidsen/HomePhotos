using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Web.Controllers;
using SCS.HomePhotos.Web.Models;
using SCS.HomePhotos.Web.Security;
using SCS.HomePhotos.Web.Test.Mocks;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SCS.HomePhotos.Web.Test.Controllers
{
    public class AccountControllerTests : ControllerTestBase
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly AccountController _accountController;
        private readonly Mock<ILogger<AccountController>> _logger;
        private readonly Mock<IOptions<JwtAuthentication>> _jwtAuthentication;
        private readonly Mock<IAccountService> _accountService;
        private readonly Mock<IStaticConfig> _staticConfig;
        private readonly Mock<ISecurityService> _securityService;
        private readonly Mock<IFileUploadService> _fileUploadService;        
        private readonly Mock<IDynamicConfig> _dynamicConfig;

        public AccountControllerTests()
        {
            _logger = new Mock<ILogger<AccountController>>();
            _jwtAuthentication = new Mock<IOptions<JwtAuthentication>>();
            _accountService = new Mock<IAccountService>();
            _staticConfig = new Mock<IStaticConfig>();
            _fileUploadService = new Mock<IFileUploadService>();
            _dynamicConfig = new Mock<IDynamicConfig>();
            _securityService = MockHelper.GetMockSecurityService("jdoe", false);

            _jwtAuthentication.SetupGet(p => p.Value).Returns(new JwtAuthentication
            {
                SecurityKey = "3loMQZr/i467EV5pPUlHJBdFgyOkiPERM+1xm8F8H3U=",
                ValidAudience = "aud",
                ValidIssuer = "iss"
            });

            _staticConfig.SetupGet(p => p.RefreshTokenExpirationDays).Returns(1);
            _staticConfig.SetupGet(p => p.TokenExpirationMinutes).Returns(15);

            _accountController = new AccountController(_logger.Object, _accountService.Object, _staticConfig.Object, 
                _securityService.Object, _fileUploadService.Object, _dynamicConfig.Object);
        }

        [Fact]
        public async Task Register()
        {
            var registerModel = new RegisterModel
            {
                UserName = "wdavidsen",
                FirstName = "Bill",
                LastName = "Davis",
                Password = "654321",
                PasswordCompare = "654321",
                EmailAddress = "wdavidsen@gmail.com"
            };

            _accountService.Setup(m => m.Register(It.IsAny<Model.User>(), registerModel.Password))
                .ReturnsAsync(new RegisterResult())
                .Callback<Model.User, string>((model, password) =>
                {
                    Assert.Equal(registerModel.UserName, model.UserName);
                    Assert.Equal(registerModel.FirstName, model.FirstName);
                    Assert.Equal(registerModel.LastName, model.LastName);
                    Assert.Equal(registerModel.EmailAddress, model.EmailAddress);
                    Assert.Equal(registerModel.Password, password);
                    Assert.Equal(registerModel.PasswordCompare, password);
                });

            SetControllerContext(_accountController, "POST");
            var response = await _accountController.Register(registerModel);

            _accountService.Verify(m => m.Register(It.IsAny<Model.User>(), registerModel.Password),
                Times.Once);
            _accountService.Verify(m => m.DeleteAgentRefreshTokens(registerModel.UserName, It.IsAny<string>()),
               Times.Never);
            _accountService.Verify(m => m.SaveRefreshToken(registerModel.UserName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Never);

            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task RegisterUserNameTaken()
        {
            var registerModel = new RegisterModel
            {
                UserName = "wdavidsen",
                FirstName = "Bill",
                LastName = "Davis",
                Password = "654321",
                PasswordCompare = "654321"
            };

            _accountService.Setup(m => m.Register(It.IsAny<Model.User>(), registerModel.Password))
                .ReturnsAsync(new RegisterResult
                {
                    UserNameTaken = true
                });

            var response = await _accountController.Register(registerModel);

            _accountService.Verify(m => m.Register(It.IsAny<Model.User>(), registerModel.Password), Times.Once);

            Assert.IsType<ConflictObjectResult>(response);
        }

        [Fact]
        public async Task RegisterPasswordNotStrong()
        {
            var registerModel = new RegisterModel
            {
                UserName = "wdavidsen",
                FirstName = "Bill",
                LastName = "Davis",
                Password = "654321",
                PasswordCompare = "654321"
            };

            _accountService.Setup(m => m.Register(It.IsAny<Model.User>(), registerModel.Password))
                .ReturnsAsync(new RegisterResult
                {
                    PasswordNotStrong = true
                });

            var response = await _accountController.Register(registerModel);

            _accountService.Verify(m => m.Register(It.IsAny<Model.User>(), registerModel.Password), Times.Once);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task ChangePassword()
        {
            var userName = "wdavidsen";
            var password = _fixture.Create<string>();
            var newPassword = _fixture.Create<string>();

            SetControllerContext(_accountController, "POST");

            _accountService.Setup(m => m.ChangePassword(userName, password, newPassword))
                .ReturnsAsync(new ChangePasswordResult
                {
                    User = new Model.User
                    {
                        UserName = userName
                    }
                });

            var response = await _accountController.ChangePassword(
                new ChangePasswordModel
                {
                    UserName = userName,
                    CurrentPassword = password,
                    NewPassword = newPassword
                });

            _accountService.Verify(m => m.ChangePassword(userName, password, newPassword),
                Times.Once);
            _accountService.Verify(m => m.DeleteAgentRefreshTokens(userName, It.IsAny<string>()),
                Times.Once);
            _accountService.Verify(m => m.SaveRefreshToken(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Once);

            Assert.IsType<OkObjectResult>(response);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<TokenResultModel>(value);

            var loginResult = value as TokenResultModel;

            Assert.NotNull(loginResult.Jwt);
            Assert.NotNull(loginResult.RefreshToken);
            Assert.True(loginResult.Jwt.Length > 50);
            Assert.True(loginResult.RefreshToken.Length > 30);
        }

        [Fact]
        public async Task ChangePasswordPasswordNotStrong()
        {
            var userName = "wdavidsen";
            var password = _fixture.Create<string>();
            var newPassword = "password1";

            var expectedErrorCode = "PasswordStrength";

            SetControllerContext(_accountController, "POST");
            
            _accountService.Setup(m => m.ChangePassword(userName, password, newPassword))
                .ReturnsAsync(new ChangePasswordResult
                {
                   PasswordNotStrong = true
                });

            var response = await _accountController.ChangePassword(
                new ChangePasswordModel
                {
                    UserName = userName,
                    CurrentPassword = password,
                    NewPassword = newPassword,
                    NewPasswordCompare = newPassword
                });

            _accountService.Verify(m => m.ChangePassword(userName, password, newPassword),
                Times.Once);
            _accountService.Verify(m => m.DeleteAgentRefreshTokens(userName, It.IsAny<string>()),
                Times.Never);
            _accountService.Verify(m => m.SaveRefreshToken(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Never);

            Assert.IsType<BadRequestObjectResult>(response);

            var value = ((BadRequestObjectResult)response).Value;

            Assert.IsType<ProblemModel>(value);
            Assert.Equal(((ProblemModel)value).Id, expectedErrorCode);
        }

        [Fact]
        public async Task ChangePasswordModelPasswordMismatch()
        {
            var userName = "wdavidsen";
            var password = _fixture.Create<string>();
            var newPassword = _fixture.Create<string>();

            var expectedErrorCode = "InvalidRequestPayload";

            SetControllerContext(_accountController, "POST");
            _accountController.ModelState.AddModelError("mismatch", "mismatch");

            _accountService.Setup(m => m.ChangePassword(userName, password, newPassword))
                .ReturnsAsync(new ChangePasswordResult
                {
                    PasswordMismatch = true
                });

            var response = await _accountController.ChangePassword(
                new ChangePasswordModel
                {
                    UserName = userName,
                    CurrentPassword = password,
                    NewPassword = newPassword,
                    NewPasswordCompare = newPassword + "!"
                });

            _accountService.Verify(m => m.ChangePassword(userName, password, newPassword),
                Times.Never);
            _accountService.Verify(m => m.DeleteAgentRefreshTokens(userName, It.IsAny<string>()),
                Times.Never);
            _accountService.Verify(m => m.SaveRefreshToken(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Never);

            Assert.IsType<BadRequestObjectResult>(response);

            var value = ((BadRequestObjectResult)response).Value;

            Assert.IsType<ProblemModel>(value);
            Assert.Equal(((ProblemModel)value).Id, expectedErrorCode);
        }

        protected override void DisposeController()
        {
            _accountController.Dispose();
        }
    }
}
