using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SCS.HomePhotos.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SCS.HomePhotos.Web.Test.Mocks
{
    public static class MockHelper
    {
        public static Mock<ISecurityService> GetMockSecurityService(string username, int userId, bool isAdmin)
        {
            var mock = new Mock<ISecurityService>();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Typ, Guid.NewGuid().ToString())
            };

            if (isAdmin)
            {
                claims.Add(new Claim("Admin", "Admin"));
            }

            var jwt = new JwtSecurityToken(
                issuer: "https://localhost",
                audience: "https://localhost",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Convert.FromBase64String("3loMQZr/i467EV5pPUlHJBdFgyOkiPERM+1xm8F8H3U=")), 
                    SecurityAlgorithms.HmacSha256));

            var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
            identity.AddClaims(new List<Claim>
                {
                    new Claim(identity.NameClaimType, username),
                    new Claim(JwtRegisteredClaimNames.Sub, username),
                    new Claim(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Typ, Guid.NewGuid().ToString())
                });

            var principal = new ClaimsPrincipal(identity);

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            mock.Setup(m => m.GenerateRefreshToken())
                .Returns("34q384571384571803457234513490809");

            mock.Setup(m => m.GenerateToken(It.IsAny<IEnumerable<Claim>>()))
                .Returns(token);

            mock.Setup(m => m.GetUserClaims(It.IsAny<Model.User>(), It.IsAny<RoleType>()))
                .Returns(claims);

            mock.Setup(m => m.GetPrincipalFromExpiredToken(It.IsAny<string>()))
                .Returns(principal);

            mock.SetupGet(p => p.ValidAudience)
                .Returns("https://localhost");

            mock.SetupGet(p => p.ValidIssuer)
                .Returns("https://localhost");

            return mock;
        }
    }
}
