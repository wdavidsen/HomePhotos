using AutoFixture;
using Moq;
using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Service.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SCS.HomePhotos.Service.Test
{
    public class AccountServiceTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly AccountService _accountService;
        private readonly Mock<IUserData> _userData;
        private readonly Mock<IUserTokenData> _userTokenData;
        private readonly Mock<IStaticConfig> _staticConfig;
        private readonly Mock<IAdminLogService> _adminLogService;

        public AccountServiceTests()
        {
            _staticConfig = new Mock<IStaticConfig>();
            _userData = new Mock<IUserData>();
            _userTokenData = new Mock<IUserTokenData>();
            _adminLogService = new Mock<IAdminLogService>();

            _staticConfig.SetupGet(p => p.MaxFailedLogins).Returns(3);
            _staticConfig.SetupGet(p => p.PasswordRequirements)
                .Returns(new PasswordRequirements
                {
                    Digits = 1,
                    MinLength = 8,
                    SpecialCharacters = 1,
                    UppercaseCharacters = 1
                });

            _accountService = new AccountService(_staticConfig.Object, _userData.Object, _userTokenData.Object, _adminLogService.Object);
        }

        [Fact]
        public async Task Authenticate()
        {
            var passwordHash = "1000:PkYbm8bOlapPJezAPXkyPd0f+/+CM0Ht:1ovVFVOoE3xq7MM0Xom4zJm21NFY0TzJ";
            var password = "Pass$12345";
            var userName = "jdoe";

            var user = new User
            {
                PasswordHash = passwordHash,
                UserName = userName,
                Enabled = true
            };

            _userData.Setup(m => m.GetUser(userName)).ReturnsAsync(user);
            _userData.Setup(m => m.UpdateAsync(It.IsAny<User>()))
                .Callback<User>((u) =>
                {
                    Assert.True(u.LastLogin > DateTime.Now.AddMinutes(-1));
                    Assert.True(u.LastLogin < DateTime.Now.AddMinutes(1));
                    Assert.Equal(0, u.FailedLoginCount);
                    Assert.Equal(user.PasswordHash, u.PasswordHash);
                    Assert.Equal(user.Enabled, u.Enabled);
                });

            var result = await _accountService.Authenticate(userName, password);

            _userData.Verify(m => m.GetUser(userName),
                Times.Once);
            _userData.Verify(m => m.UpdateAsync(It.IsAny<User>()),
                Times.Once);

            Assert.True(result.Success);
            Assert.False(result.MustChangePassword);
            Assert.False(result.PasswordMismatch);
            Assert.False(result.UserDisabled);
            Assert.False(result.UserNotExists);
        }

        [Fact]
        public async Task AuthenticateUserNotExists()
        {
            var password = "Pass$12345";
            var userName = "jdoe";

            var user = default(User);

            _userData.Setup(m => m.GetUser(userName)).ReturnsAsync(user);

            var result = await _accountService.Authenticate(userName, password);

            _userData.Verify(m => m.GetUser(userName),
                Times.Once);
            _userData.Verify(m => m.UpdateAsync(It.IsAny<User>()),
                Times.Never);

            Assert.False(result.Success);
            Assert.False(result.MustChangePassword);
            Assert.False(result.PasswordMismatch);
            Assert.False(result.UserDisabled);
            Assert.True(result.UserNotExists);
        }

        [Fact]
        public async Task AuthenticateDisabledUser()
        {
            var passwordHash = "1000:PkYbm8bOlapPJezAPXkyPd0f+/+CM0Ht:1ovVFVOoE3xq7MM0Xom4zJm21NFY0TzJ";
            var password = "Pass$12345";
            var userName = "jdoe";

            var user = new User
            {
                PasswordHash = passwordHash,
                UserName = userName,
                Enabled = false
            };

            _userData.Setup(m => m.GetUser(userName)).ReturnsAsync(user);
            _userData.Setup(m => m.UpdateAsync(It.IsAny<User>()))
                .Callback<User>((u) =>
                {
                    Assert.True(u.LastLogin > DateTime.Now.AddMinutes(-1));
                    Assert.True(u.LastLogin < DateTime.Now.AddMinutes(1));
                    Assert.Equal(0, u.FailedLoginCount);
                    Assert.Equal(user.PasswordHash, u.PasswordHash);
                    Assert.False(u.Enabled);
                });

            var result = await _accountService.Authenticate(userName, password);

            _userData.Verify(m => m.GetUser(userName),
                Times.Once);
            _userData.Verify(m => m.UpdateAsync(It.IsAny<User>()),
                Times.Once);

            Assert.False(result.Success);
            Assert.False(result.MustChangePassword);
            Assert.False(result.PasswordMismatch);
            Assert.True(result.UserDisabled);
            Assert.False(result.UserNotExists);
        }

        [Fact]
        public async Task AuthenticateBadPassword()
        {
            var passwordHash = "1000:PkYbm8bOlapPJezAPXkyPd0f+/+CM0Ht:1ovVFVOoE3xq7MM0Xom4zJm21NFY0TzJ";
            var password = "bad_password";
            var userName = "jdoe";

            var user = new User
            {
                PasswordHash = passwordHash,
                UserName = userName,
                Enabled = true,
                FailedLoginCount = 0,
                LastLogin = DateTime.Today
            };

            _userData.Setup(m => m.GetUser(userName)).ReturnsAsync(user);
            _userData.Setup(m => m.UpdateAsync(It.IsAny<User>()))
                .Callback<User>((u) =>
                {
                    Assert.Equal(user.PasswordHash, u.PasswordHash);
                    Assert.True(u.Enabled);
                    Assert.Equal(1, u.FailedLoginCount);
                    Assert.Equal(user.LastLogin, u.LastLogin);
                });

            var result = await _accountService.Authenticate(userName, password);

            _userData.Verify(m => m.GetUser(userName),
                Times.Once);
            _userData.Verify(m => m.UpdateAsync(It.IsAny<User>()),
                Times.Once);

            Assert.False(result.Success);
            Assert.False(result.MustChangePassword);
            Assert.True(result.PasswordMismatch);
            Assert.False(result.UserDisabled);
            Assert.False(result.UserNotExists);
        }

        [Fact]
        public async Task AuthenticateBadPasswordWithLock()
        {
            var passwordHash = "1000:PkYbm8bOlapPJezAPXkyPd0f+/+CM0Ht:1ovVFVOoE3xq7MM0Xom4zJm21NFY0TzJ";
            var password = "bad_password";
            var userName = "jdoe";

            var user = new User
            {
                PasswordHash = passwordHash,
                UserName = userName,
                Enabled = true,
                FailedLoginCount = 0,
                LastLogin = DateTime.Today
            };

            _staticConfig.SetupGet(p => p.MaxFailedLogins).Returns(0);
            _userData.Setup(m => m.GetUser(userName)).ReturnsAsync(user);
            _userData.Setup(m => m.UpdateAsync(It.IsAny<User>()))
                .Callback<User>((u) =>
                {
                    Assert.Equal(user.PasswordHash, u.PasswordHash);
                    Assert.False(u.Enabled);
                    Assert.Equal(1, u.FailedLoginCount);
                    Assert.Equal(user.LastLogin, u.LastLogin);
                });

            var result = await _accountService.Authenticate(userName, password);

            _userData.Verify(m => m.GetUser(userName),
                Times.Once);
            _userData.Verify(m => m.UpdateAsync(It.IsAny<User>()),
                Times.Once);

            Assert.False(result.Success);
            Assert.False(result.MustChangePassword);
            Assert.True(result.PasswordMismatch);
            Assert.True(result.UserDisabled);
            Assert.True(result.AttemptsExceeded);
            Assert.False(result.UserNotExists);
        }

        [Fact]
        public async Task Register()
        {
            var password = "Pass$12345";
            var userName = "jdoe";
            var firstName = "John";
            var lastName = "Doe";
            var emailAddress = "jdoe@gmail.com";

            var user = default(User);

            _userData.Setup(m => m.GetUser(userName)).ReturnsAsync(user);
            _userData.Setup(m => m.InsertAsync(It.IsAny<User>()))
               .Callback<User>((u) =>
               {
                   Assert.NotNull(u.PasswordHash);
                   Assert.True(u.PasswordHash.Length > 30);
                   Assert.False(u.Enabled);
                   Assert.Null(u.LastLogin);
                   Assert.False(u.MustChangePassword);
                   Assert.Equal(userName, u.UserName);
                   Assert.Equal(firstName, u.FirstName);
                   Assert.Equal(lastName, u.LastName);
                   Assert.Equal(emailAddress, u.EmailAddress);
               });

            var userToCreate = new User
            {
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress
            };
            var result = await _accountService.Register(userToCreate, password);

            _userData.Verify(m => m.GetUser(userName),
                Times.Once);
            _userData.Verify(m => m.InsertAsync(It.IsAny<User>()),
                Times.Once);

            Assert.True(result.Success);
            Assert.False(result.UserNameTaken);
        }

        [Fact]
        public async Task RegisterExistingUser()
        {
            var password = "Pass$12345";
            var userName = "jdoe";
            var firstName = "John";
            var lastName = "Doe";

            var user = new User
            {
                UserName = userName,
                FirstName = firstName,
                LastName = lastName
            };

            _userData.Setup(m => m.GetUser(userName)).ReturnsAsync(user);

            var userToCreate = new User(user);
            var result = await _accountService.Register(userToCreate, password);

            _userData.Verify(m => m.GetUser(userName),
                Times.Once);
            _userData.Verify(m => m.InsertAsync(It.IsAny<User>()),
                Times.Never);

            Assert.False(result.Success);
            Assert.True(result.UserNameTaken);
        }

        [Theory]
        [InlineData("Pass$12")]
        [InlineData("pass$123")]
        [InlineData("Pass1234")]
        [InlineData("Pass$way")]
        public async Task RegisterPasswordNotMeetsRequirements(string password)
        {
            var userName = "jdoe";
            var firstName = "John";
            var lastName = "Doe";

            var user = new User
            {
                UserName = userName,
                FirstName = firstName,
                LastName = lastName
            };

            var userToCreate = new User(user);
            var result = await _accountService.Register(userToCreate, password);

            _userData.Verify(m => m.GetUser(userName),
                Times.Never);
            _userData.Verify(m => m.InsertAsync(It.IsAny<User>()),
                Times.Never);

            Assert.False(result.Success);
            Assert.True(result.PasswordNotStrong);
        }

        [Fact]
        public async Task ChangePassword()
        {
            var passwordHash = "1000:PkYbm8bOlapPJezAPXkyPd0f+/+CM0Ht:1ovVFVOoE3xq7MM0Xom4zJm21NFY0TzJ";
            var password = "Pass$12345";
            var newPassword = "1234$Pass";
            var userName = "jdoe";

            var user = new User
            {
                PasswordHash = passwordHash,
                UserName = userName,
                Enabled = true
            };

            var updateNumber = 1;

            _userData.Setup(m => m.GetUser(userName)).ReturnsAsync(user);
            _userData.Setup(m => m.UpdateAsync(It.IsAny<User>()))
                .Callback<User>((u) =>
                {
                    if (updateNumber == 2)
                    {
                        Assert.True(u.LastLogin > DateTime.Now.AddMinutes(-1));
                        Assert.True(u.LastLogin < DateTime.Now.AddMinutes(1));
                        Assert.Equal(0, u.FailedLoginCount);
                        Assert.NotEqual(passwordHash, u.PasswordHash);
                        Assert.Equal(user.Enabled, u.Enabled);
                    }
                    updateNumber++;
                });

            var result = await _accountService.ChangePassword(userName, password, newPassword);

            _userData.Verify(m => m.GetUser(userName),
                Times.Exactly(2));
            _userData.Verify(m => m.UpdateAsync(It.IsAny<User>()),
                Times.Exactly(2));

            Assert.True(result.Success);
            Assert.False(result.MustChangePassword);
            Assert.False(result.PasswordMismatch);
            Assert.False(result.UserDisabled);
            Assert.False(result.UserNotExists);
        }

        [Fact]
        public async Task ChangePasswordAuthFailed()
        {
            var passwordHash = "1000:PkYbm8bOlapPJezAPXkyPd0f+/+CM0Ht:1ovVFVOoE3xq7MM0Xom4zJm21NFY0TzJ";
            var password = "Pass$12345_wrong";
            var newPassword = "1234$Pass";
            var userName = "jdoe";

            var user = new User
            {
                PasswordHash = passwordHash,
                UserName = userName,
                Enabled = true
            };

            _userData.Setup(m => m.GetUser(userName)).ReturnsAsync(user);

            var result = await _accountService.ChangePassword(userName, password, newPassword);

            _userData.Verify(m => m.GetUser(userName),
                Times.Once);
            _userData.Verify(m => m.UpdateAsync(It.IsAny<User>()),
                Times.Once);

            Assert.False(result.Success);
            Assert.True(result.PasswordMismatch);
        }

        [Fact]
        public async Task ChangePasswordUsedPreviously()
        {
            var passwordHash = "1000:PkYbm8bOlapPJezAPXkyPd0f+/+CM0Ht:1ovVFVOoE3xq7MM0Xom4zJm21NFY0TzJ";
            var password = "Pass$12345";
            var newPassword = "Pass$12345";
            var userName = "jdoe";

            var user = new User
            {
                PasswordHash = passwordHash,
                UserName = userName,
                Enabled = true,
                PasswordHistory = $"something invalid\n1000:PkYbm8bOlapPJezAPXkyPd0f+/+CM0Ht:1ovVFVOoE3xq7MM0Xom4zJm21NFY0TzJ"
            };

            _userData.Setup(m => m.GetUser(userName)).ReturnsAsync(user);

            var result = await _accountService.ChangePassword(userName, password, newPassword);

            _userData.Verify(m => m.GetUser(userName),
                Times.Exactly(2));
            _userData.Verify(m => m.UpdateAsync(It.IsAny<User>()),
                Times.Exactly(2));

            Assert.False(result.Success);
            Assert.True(result.PasswordUsedPreviously);
        }

        [Fact]
        public async Task GetRefreshTokens()
        {
            var userName = "jdoe";
            var userId = 1;
            var token = _fixture.Create<string>();

            var user = new User
            {
                UserId = userId,
                UserName = userName
            };

            var tokens = new List<UserToken>
            {
                new UserToken { Token = token, AgentIdentifier = "id1", Refresh = true, Issuer = "iss", Audience = "aud" }
            };

            _userData.Setup(m => m.GetUser(userName)).ReturnsAsync(user);

            _userTokenData.Setup(m => m.GetUserTokens(userId, true)).ReturnsAsync(tokens);

            var refreshTokens = await _accountService.GetRefreshTokens(userName, "iss", "aud", "id1");

            _userData.Verify(m => m.GetUser(userName),
                Times.Once);

            _userTokenData.Verify(m => m.GetUserTokens(userId, true),
                Times.Once);

            Assert.True(refreshTokens.Count == 1);
            Assert.True(refreshTokens[0].Refresh);
            Assert.Equal(token, refreshTokens[0].Token);
        }

        [Theory]
        [InlineData("id2", true, "iss", "aud")]
        [InlineData("id1", false, "iss", "aud")]
        [InlineData("id1", true, "iss2", "aud")]
        [InlineData("id1", true, "iss", "aud2")]
        public async Task GetRefreshTokensNoneAvailable(string agentId, bool refresh, string issuer, string audience)
        {
            var userName = "jdoe";
            var userId = 1;
            var token = _fixture.Create<string>();

            var user = new User
            {
                UserId = userId,
                UserName = userName
            };

            var tokens = new List<UserToken>
            {
                new UserToken { Token = token, AgentIdentifier = agentId, Refresh = refresh, Issuer = issuer, Audience = audience }
            };

            _userData.Setup(m => m.GetUser(userName)).ReturnsAsync(user);

            _userTokenData.Setup(m => m.GetUserTokens(userId, true)).ReturnsAsync(tokens);

            var refreshTokens = await _accountService.GetRefreshTokens(userName, "iss", "aud", "id1");

            _userData.Verify(m => m.GetUser(userName),
                Times.Once);

            _userTokenData.Verify(m => m.GetUserTokens(userId, true),
                Times.Once);

            Assert.True(refreshTokens.Count == 0);
        }

        [Fact]
        public async Task DeleteRefreshToken()
        {
            var userName = "jdoe";
            var userId = 1;
            var refreshToken = _fixture.Create<string>();

            var user = new User
            {
                UserId = userId,
                UserName = userName
            };

            _userData.Setup(m => m.GetUser(userName)).ReturnsAsync(user);

            _userTokenData.Setup(m => m.DeleteRefreshToken(userId, refreshToken))
                .Callback<int, string>((uId, token) =>
                {
                    Assert.Equal(userId, uId);
                    Assert.Equal(refreshToken, token);
                });

            await _accountService.DeleteRefreshToken(userName, refreshToken);

            _userData.Verify(m => m.GetUser(userName),
                Times.Once);
            _userTokenData.Verify(m => m.DeleteRefreshToken(userId, refreshToken),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAgentRefreshTokens()
        {
            var userName = "jdoe";
            var userId = 1;
            var agentId = "id1";

            var user = new User
            {
                UserId = userId,
                UserName = userName
            };

            _userData.Setup(m => m.GetUser(userName)).ReturnsAsync(user);

            _userTokenData.Setup(m => m.DeleteAgentRefreshTokens(userId, agentId))
                .Callback<int, string>((uId, aId) =>
                {
                    Assert.Equal(userId, uId);
                    Assert.Equal(agentId, aId);
                });

            await _accountService.DeleteAgentRefreshTokens(userName, agentId);

            _userData.Verify(m => m.GetUser(userName),
                Times.Once);
            _userTokenData.Verify(m => m.DeleteAgentRefreshTokens(userId, agentId),
                Times.Once);
        }

        [Fact]
        public async Task SaveRefreshToken()
        {
            var userName = "jdoe";
            var userId = 1;
            var newRefreshToken = _fixture.Create<string>();
            var agentIdentifier = "id1";
            var issuer = "iss";
            var audience = "aud";
            var expirationUtc = DateTime.UtcNow.AddDays(1);

            var user = new User
            {
                UserId = userId,
                UserName = userName
            };

            _userData.Setup(m => m.GetUser(userName)).ReturnsAsync(user);

            _userTokenData.Setup(m => m.InsertAsync(It.IsAny<UserToken>()))
               .Callback<UserToken>((t) =>
               {
                   Assert.Equal(newRefreshToken, t.Token);
                   Assert.Equal(agentIdentifier, t.AgentIdentifier);
                   Assert.Equal(issuer, t.Issuer);
                   Assert.Equal(audience, t.Audience);
                   Assert.Equal(expirationUtc, t.Expiration);
               });

            await _accountService.SaveRefreshToken(userName, newRefreshToken, agentIdentifier, issuer, audience, expirationUtc);

            _userData.Verify(m => m.GetUser(userName),
                Times.Once);
            _userTokenData.Verify(m => m.InsertAsync(It.IsAny<UserToken>()),
                Times.Once);
        }
    }
}
