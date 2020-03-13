using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Web.Controllers;
using SCS.HomePhotos.Web.Models;
using SCS.HomePhotos.Web.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace SCS.HomePhotos.Web.Test.Controllers
{
    public class AuthControllerTests : ControllerTestBase
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly AuthController _authController;
        private readonly Mock<ILogger<AuthController>> _logger;
        private readonly Mock<IOptions<JwtAuthentication>> _jwtAuthentication;
        private readonly Mock<IAccountService> _accountService;
        private readonly Mock<IStaticConfig> _staticConfig;

        public AuthControllerTests()
        {
            _logger = new Mock<ILogger<AuthController>>();
            _jwtAuthentication = new Mock<IOptions<JwtAuthentication>>();
            _accountService = new Mock<IAccountService>();
            _staticConfig = new Mock<IStaticConfig>();

            _jwtAuthentication.SetupGet(p => p.Value).Returns(new JwtAuthentication
            {
                SecurityKey = "3loMQZr/i467EV5pPUlHJBdFgyOkiPERM+1xm8F8H3U=",
                ValidAudience = "aud",
                ValidIssuer = "iss"
            });

            _staticConfig.SetupGet(p => p.RefreshTokenExpirationDays).Returns(1);
            _staticConfig.SetupGet(p => p.TokenExpirationMinutes).Returns(15);

            _authController = new AuthController(_logger.Object, _jwtAuthentication.Object, _accountService.Object, _staticConfig.Object);
        }

        [Fact]
        public async Task Login()
        {
            var userName = "wdavidsen";
            var password = "Pass$1234";
            var user = new Model.User
            {
                UserName = userName,
                FirstName = "Bill",
                LastName = "Davis"
            };

            SetControllerContext(_authController, "POST");

            _accountService.Setup(m => m.Authenticate(userName, password))
                .ReturnsAsync(new AuthResult
                {
                    User = user
                });

            var response = await _authController.Login(
                new LoginModel
                {
                    UserName = userName,
                    Password = password
                });

            _accountService.Verify(m => m.Authenticate(userName, password),
                Times.Once);
            _accountService.Verify(m => m.DeleteAgentRefreshTokens(userName, It.IsAny<string>()),
                Times.Once);
            _accountService.Verify(m => m.SaveRefreshToken(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Once);

            Assert.IsType<OkObjectResult>(response);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<Dto.User>(value);

            var loginResult = value as Dto.User;

            Assert.NotNull(loginResult.Token);
            Assert.NotNull(loginResult.RefreshToken);
            Assert.True(loginResult.Token.Length > 50);
            Assert.True(loginResult.RefreshToken.Length > 30);
            Assert.Equal(user.UserName, loginResult.Username);
            Assert.Equal(user.FirstName, loginResult.FirstName);
            Assert.Equal(user.LastName, loginResult.LastName);
        }

        [Fact]
        public async Task LoginFailed()
        {
            var userName = "wdavidsen";
            var password = "Pass$1234";

            SetControllerContext(_authController, "POST");

            _accountService.Setup(m => m.Authenticate(userName, password))
                .ReturnsAsync(new AuthResult
                {
                    PasswordMismatch = true
                });

            var response = await _authController.Login(
                new LoginModel
                {
                    UserName = userName,
                    Password = password
                });

            _accountService.Verify(m => m.Authenticate(userName, password),
                Times.Once);
            _accountService.Verify(m => m.DeleteAgentRefreshTokens(userName, It.IsAny<string>()),
                Times.Never);
            _accountService.Verify(m => m.SaveRefreshToken(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Never);

            Assert.IsType<UnauthorizedObjectResult>(response);
        }

        [Fact]
        public async Task LoginMustChangePassword()
        {
            var userName = "wdavidsen";
            var password = "Pass$1234";

            SetControllerContext(_authController, "POST");

            _accountService.Setup(m => m.Authenticate(userName, password))
                .ReturnsAsync(new AuthResult
                {
                    MustChangePassword = true
                });

            var response = await _authController.Login(
                new LoginModel
                {
                    UserName = userName,
                    Password = password
                });

            _accountService.Verify(m => m.Authenticate(userName, password),
                Times.Once);
            _accountService.Verify(m => m.DeleteAgentRefreshTokens(userName, It.IsAny<string>()),
                Times.Never);
            _accountService.Verify(m => m.SaveRefreshToken(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Never);

            Assert.IsType<ConflictObjectResult>(response);
        }

        [Fact]
        public async Task Logout()
        {
            SetControllerContext(_authController, "POST", "wdavidsen");

            var response = await _authController.Logout();

            Assert.IsType<OkResult>(response);

            _accountService.Verify(m => m.DeleteAgentRefreshTokens("wdavidsen", It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task LogoutNotLoggedIn()
        {
            var response = await _authController.Logout();

            Assert.IsType<BadRequestObjectResult>(response);

            _accountService.Verify(m => m.DeleteAgentRefreshTokens(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Refresh()
        {
            var userName = "wdavidsen";
            SetControllerContext(_authController, "POST", userName);

            var refreshToken = _authController.GenerateRefreshToken();
            var token = _authController.GenerateToken(new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userName),
                    new Claim(JwtRegisteredClaimNames.UniqueName, userName)
                });

            _accountService.Setup(m => m.GetRefreshTokens(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Model.UserToken>
                {
                    { new Model.UserToken { Token = refreshToken, Expiration = DateTime.UtcNow.AddDays(1)} }
                });

            var response = await _authController.Refresh(
                new RefreshModel
                {
                    RefreshToken = refreshToken,
                    Token = token
                });

            _accountService.Verify(m => m.GetRefreshTokens(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
            _accountService.Verify(m => m.DeleteRefreshToken(userName, refreshToken),
                Times.Once);
            _accountService.Verify(m => m.SaveRefreshToken(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Once);

            Assert.IsType<OkObjectResult>(response);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<TokenResultModel>(value);

            var loginResult = value as TokenResultModel;

            Assert.NotNull(loginResult.Token);
            Assert.NotNull(loginResult.RefreshToken);
            Assert.True(loginResult.Token.Length > 50);
            Assert.True(loginResult.RefreshToken.Length > 30);
        }

        [Fact]
        public async Task RefreshInvalidToken()
        {
            var userName = "wdavidsen";
            SetControllerContext(_authController, "POST", userName);

            var refreshToken = _authController.GenerateRefreshToken();
            var token = _fixture.Create<string>();

            var response = await _authController.Refresh(
                new RefreshModel
                {
                    RefreshToken = refreshToken,
                    Token = token
                });

            _accountService.Verify(m => m.GetRefreshTokens(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
            _accountService.Verify(m => m.DeleteRefreshToken(userName, refreshToken),
                Times.Never);
            _accountService.Verify(m => m.SaveRefreshToken(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Never);

            Assert.IsType<UnauthorizedResult>(response);
        }

        [Fact]
        public async Task RefreshNoRefreshTokens()
        {
            var userName = "wdavidsen";
            SetControllerContext(_authController, "POST", userName);

            var refreshToken = _authController.GenerateRefreshToken();
            var token = _authController.GenerateToken(new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userName),
                    new Claim(JwtRegisteredClaimNames.UniqueName, userName)
                });

            _accountService.Setup(m => m.GetRefreshTokens(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Model.UserToken>
                {
                });

            var response = await _authController.Refresh(
                new RefreshModel
                {
                    RefreshToken = refreshToken,
                    Token = token
                });

            _accountService.Verify(m => m.GetRefreshTokens(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
            _accountService.Verify(m => m.DeleteRefreshToken(userName, refreshToken),
                Times.Never);
            _accountService.Verify(m => m.SaveRefreshToken(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Never);

            Assert.IsType<UnauthorizedResult>(response);
        }

        [Fact]
        public async Task RefreshOldRefreshTokens()
        {
            var userName = "wdavidsen";
            SetControllerContext(_authController, "POST", userName);

            var refreshToken = _authController.GenerateRefreshToken();
            var token = _authController.GenerateToken(new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userName),
                    new Claim(JwtRegisteredClaimNames.UniqueName, userName)
                });

            _accountService.Setup(m => m.GetRefreshTokens(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Model.UserToken>
                {
                    { new Model.UserToken { Token = refreshToken, Expiration = DateTime.UtcNow.AddDays(-1)} }
                });

            var response = await _authController.Refresh(
                new RefreshModel
                {
                    RefreshToken = refreshToken,
                    Token = token
                });

            _accountService.Verify(m => m.GetRefreshTokens(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
            _accountService.Verify(m => m.DeleteRefreshToken(userName, refreshToken),
                Times.Never);
            _accountService.Verify(m => m.SaveRefreshToken(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Never);

            Assert.IsType<UnauthorizedResult>(response);
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
                PasswordCompare = "654321"
            };

            _accountService.Setup(m => m.Register(It.IsAny<Model.User>(), registerModel.Password))
                .ReturnsAsync(new RegisterResult())
                .Callback<Model.User, string>((model, password) =>
                {
                    Assert.Equal(registerModel.UserName, model.UserName);
                    Assert.Equal(registerModel.FirstName, model.FirstName);
                    Assert.Equal(registerModel.LastName, model.LastName);
                    Assert.Equal(registerModel.Password, password);
                    Assert.Equal(registerModel.PasswordCompare, password);
                });

            SetControllerContext(_authController, "POST");
            var response = await _authController.Register(registerModel);

            _accountService.Verify(m => m.Register(It.IsAny<Model.User>(), registerModel.Password),
                Times.Once);
            _accountService.Verify(m => m.DeleteAgentRefreshTokens(registerModel.UserName, It.IsAny<string>()),
               Times.Once);
            _accountService.Verify(m => m.SaveRefreshToken(registerModel.UserName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Once);

            Assert.IsType<OkObjectResult>(response);
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

            var response = await _authController.Register(registerModel);

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

            var response = await _authController.Register(registerModel);

            _accountService.Verify(m => m.Register(It.IsAny<Model.User>(), registerModel.Password), Times.Once);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task ChangePassword()
        {
            var userName = "wdavidsen";
            var password = _fixture.Create<string>();
            var newPassword = _fixture.Create<string>();

            SetControllerContext(_authController, "POST");

            _accountService.Setup(m => m.ChangePassword(userName, password, newPassword))
                .ReturnsAsync(new ChangePasswordResult
                {
                    User = new Model.User
                    {
                        UserName = userName
                    }
                });

            var response = await _authController.ChangePassword(
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

            Assert.NotNull(loginResult.Token);
            Assert.NotNull(loginResult.RefreshToken);
            Assert.True(loginResult.Token.Length > 50);
            Assert.True(loginResult.RefreshToken.Length > 30);
        }

        [Fact]
        public async Task ChangePasswordFailed()
        {
            var userName = "wdavidsen";
            var password = _fixture.Create<string>();
            var newPassword = _fixture.Create<string>();

            SetControllerContext(_authController, "POST");

            _accountService.Setup(m => m.ChangePassword(userName, password, newPassword))
                .ReturnsAsync(new ChangePasswordResult
                {
                    UserDisabled = true
                });

            var response = await _authController.ChangePassword(
                new ChangePasswordModel
                {
                    UserName = userName,
                    CurrentPassword = password,
                    NewPassword = newPassword
                });

            _accountService.Verify(m => m.ChangePassword(userName, password, newPassword),
                Times.Once);
            _accountService.Verify(m => m.DeleteAgentRefreshTokens(userName, It.IsAny<string>()),
                Times.Never);
            _accountService.Verify(m => m.SaveRefreshToken(userName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Never);

            Assert.IsType<UnauthorizedObjectResult>(response);

            var value = ((UnauthorizedObjectResult)response).Value;

            Assert.IsType<ProblemModel>(value);
        }

        [Fact]
        public async Task ChangePasswordPasswordMismatch()
        {
            var userName = "wdavidsen";
            var password = _fixture.Create<string>();
            var newPassword = _fixture.Create<string>();

            SetControllerContext(_authController, "POST");
            _authController.ModelState.AddModelError("mismatch", "mismatch");

            _accountService.Setup(m => m.ChangePassword(userName, password, newPassword))
                .ReturnsAsync(new ChangePasswordResult
                {
                    PasswordMismatch = true
                });

            var response = await _authController.ChangePassword(
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

            Assert.IsType<SerializableError>(value);
        }

        protected override void DisposeController()
        {
            _authController.Dispose();
        }
    }
}
