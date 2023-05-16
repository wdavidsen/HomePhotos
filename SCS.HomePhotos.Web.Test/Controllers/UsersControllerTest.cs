using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Web.Controllers;
using SCS.HomePhotos.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SCS.HomePhotos.Web.Test.Controllers
{
    public class UsersControllerTest : ControllerTestBase
    {
        private readonly Fixture _fixture;

        private readonly UsersController _usersController;
        private readonly Mock<IAccountService> _accountService;
        private readonly Mock<IStaticConfig> _staticConfig;
        private readonly Mock<ILogger<UsersController>> _logger;

        public UsersControllerTest()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _logger = new Mock<ILogger<UsersController>>();
            _accountService = new Mock<IAccountService>();
            _staticConfig = new Mock<IStaticConfig>();

            _usersController = new UsersController(_logger.Object, _accountService.Object, _staticConfig.Object);
        }

        [Fact]
        public async Task GetUsers()
        {
            var users = _fixture.CreateMany<User>(5);

            _accountService.Setup(m => m.GetUsers())
                .ReturnsAsync(users);

            var response = await _usersController.GetUsers();

            _accountService.Verify(m => m.GetUsers(),
                Times.Once);

            Assert.IsType<OkObjectResult>(response);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<List<Dto.User>>(value);

            var dtos = value as List<Dto.User>;

            Assert.Equal(users.Count(), dtos.Count);
        }

        [Fact]
        public async Task GetUser()
        {
            var user = _fixture.Create<User>();
            user.UserId = 1;

            _accountService.Setup(m => m.GetUser(user.UserId.Value))
                .ReturnsAsync(user);

            var response = await _usersController.GetUser(user.UserId.Value);

            _accountService.Verify(m => m.GetUser(user.UserId.Value),
                Times.Once);

            Assert.IsType<OkObjectResult>(response);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<Dto.User>(value);
        }

        [Fact]
        public async Task AddUser()
        {
            var user = _fixture.Create<User>();            
            var userDto = new Dto.PasswordUser(user);
            var registerResult = new RegisterResult();
            userDto.UserId = null;
            userDto.Password = "password1";

            _accountService.Setup(m => m.Register(It.IsAny<User>(), userDto.Password))
               .ReturnsAsync(registerResult);

            _accountService.Setup(m => m.GetUser(userDto.Username))
                .ReturnsAsync(user);

            var response = await _usersController.AddUser(userDto);

            _accountService.Verify(m => m.Register(It.IsAny<User>(), userDto.Password),
                Times.Once);

            _accountService.Verify(m => m.GetUser(userDto.Username),
                Times.Once);

            Assert.IsType<OkObjectResult>(response);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<Dto.User>(value);
            Assert.Equal(user.UserId, ((Dto.User)value).UserId);
        }

        [Fact]
        public async Task AddUserInvalid()
        {
            var user = _fixture.Create<User>();
            var userDto = new Dto.PasswordUser(user);
            userDto.PasswordCompare = null;

            _usersController.ModelState.AddModelError("PasswordCompare", "PasswordCompare cannot be null");

            _accountService.Setup(m => m.SaveUser(It.IsAny<User>(), userDto.Password))
                .ReturnsAsync(user);

            var response = await _usersController.AddUser(userDto);

            _accountService.Verify(m => m.SaveUser(It.IsAny<User>(), userDto.Password),
                Times.Never);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task UpdateUser()
        {
            var user = _fixture.Create<User>();
            var userDto = new Dto.TokenUser(user);
            
            _accountService.Setup(m => m.SaveUser(It.IsAny<User>(), null))
                .ReturnsAsync(user);

            var response = await _usersController.UpdateUser(userDto);

            _accountService.Verify(m => m.SaveUser(It.IsAny<User>(), null),
                Times.Once);

            Assert.IsType<OkObjectResult>(response);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<Dto.User>(value);
            Assert.Equal(user.UserId, ((Dto.User)value).UserId);
        }

        [Fact]
        public async Task UpdateUserInvalid()
        {
            var user = _fixture.Create<User>();
            var userDto = new Dto.TokenUser(user);
            userDto.Jwt = null;

            _usersController.ModelState.AddModelError("Token", "Token is required");

            _accountService.Setup(m => m.SaveUser(It.IsAny<User>(), null))
                .ReturnsAsync(user);

            var response = await _usersController.UpdateUser(userDto);

            _accountService.Verify(m => m.SaveUser(It.IsAny<User>(), null),
                Times.Never);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task DeleteUser()
        {
            var userId = 1;

            _accountService.Setup(m => m.DeleteUser(userId));

            var response = await _usersController.DeleteUser(userId);

            _accountService.Verify(m => m.DeleteUser(userId),
                Times.Once);

            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task ChangePassword()
        {
            var user = _fixture.Create<User>();
            var model = _fixture.Create<ResetPasswordModel>();

            model.UserName = user.UserName;            
            model.NewPasswordCompare = model.NewPassword;

            var result = new ChangePasswordResult(new AuthResult());

            _accountService.Setup(m => m.GetUser(user.UserId.Value))
                .ReturnsAsync(user);

            _accountService.Setup(m => m.ResetPassword(model.UserName, model.NewPassword))
                .ReturnsAsync(result);

            var response = await _usersController.ResetPassword(user.UserId.Value, model);

            _accountService.Verify(m => m.GetUser(user.UserId.Value),
                Times.Once);

            _accountService.Verify(m => m.ResetPassword(model.UserName, model.NewPassword),
                Times.Once);

            Assert.IsType<OkResult>(response);         
        }

        protected override void Dispose(bool disposing)
        {
            _usersController.Dispose();
        }
    }
}
