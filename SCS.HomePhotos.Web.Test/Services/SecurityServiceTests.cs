using Microsoft.Extensions.Options;
using Moq;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Web.Security;
using System;
using System.Security.Claims;
using Xunit;

namespace SCS.HomePhotos.Web.Test.Services
{
    public class SecurityServiceTests
    {
        private readonly SecurityService _securityService;
        private IOptions<JwtAuthentication> _options;
        private readonly Mock<IStaticConfig> _staticConfig;

        public SecurityServiceTests()
        {
            _staticConfig = new Mock<IStaticConfig>();
            _options = Options.Create(new JwtAuthentication
            {
                SecurityKey = "3loMQZr/i467EV5pPUlHJBdFgyOkiPERM+1xm8F8H3U=",
                ValidAudience = "https://localhost:44375/",
                ValidIssuer = "https://localhost:44375/"
            });

            _securityService = new SecurityService(_options, _staticConfig.Object);
        }

        [Fact]
        public void GenerateToken()
        {
            var userName = "wdavidsen";
            var claim = new Claim(ClaimTypes.Name, userName);

            _staticConfig.SetupGet(p => p.TokenExpirationMinutes).Returns(20);

            var token = _securityService.GenerateToken(new Claim[] { claim });

            Assert.NotNull(token);
            Assert.True(token.Length > 200);
        }

        [Fact]
        public void GetPrincipalFromExpiredToken()
        {
            var userName = "wdavidsen";
            var claim = new Claim(ClaimTypes.Name, userName);
            var claim2 = new Claim(ClaimTypes.Role, RoleType.Admin.ToString());

            _staticConfig.SetupGet(p => p.TokenExpirationMinutes).Returns(20);

            var token = _securityService.GenerateToken(new Claim[] { claim, claim2 });
            var principal = _securityService.GetPrincipalFromExpiredToken(token);

            Assert.NotNull(principal);
            Assert.True(principal.IsInRole(RoleType.Admin.ToString()));
            Assert.Equal(userName, principal.Identity.Name);
        }

        [Fact]
        public void GenerateRefreshToken()
        {
            var token = _securityService.GenerateRefreshToken();

            Assert.NotNull(token);
            Assert.True(token.Length > 30);
        }

        [Fact]
        public void GetUserClaims()
        {
            var userName = "wdavidsen";
            var role = RoleType.Admin;

            var claims = _securityService.GetUserClaims(userName, role);

            Assert.True(claims.Count == 5);
            Assert.Equal(userName, claims[0].Value);
            Assert.Equal(userName, claims[1].Value);
            Assert.Equal(Guid.NewGuid().ToString().Length, Guid.Parse(claims[2].Value).ToString().Length);
            Assert.Equal(userName, claims[3].Value);
            Assert.Equal(role.ToString(), claims[4].Value);

        }
    }
}
