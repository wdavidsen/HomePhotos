using Microsoft.IdentityModel.Tokens;

using SCS.HomePhotos.Model;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace SCS.HomePhotos.Service.Test
{
    public static class MockHelper
    {
        public static IPrincipal GetPrincipal(string username, int userId, RoleType role)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Typ, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var jwt = new JwtSecurityToken(
                issuer: "https://localhost",
                audience: "https://localhost",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Convert.FromBase64String("3loMQZr/i467EV5pPUlHJBdFgyOkiPERM+1xm8F8H3U=")),
                    SecurityAlgorithms.HmacSha256));

            var identity = new ClaimsIdentity("Bearer");
            identity.AddClaims(new List<Claim>
                {
                    new Claim(identity.NameClaimType, username),
                    new Claim(JwtRegisteredClaimNames.Sub, username),
                    new Claim(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Typ, Guid.NewGuid().ToString())
                });

            return new ClaimsPrincipal(identity);
        }
    }
}
