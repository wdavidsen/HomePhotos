using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Web.Controllers;
using SCS.HomePhotos.Web.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SCS.HomePhotos.Web
{
    public class SecurityService : ISecurityService
    {
        private readonly JwtAuthentication _jwtAuthentication;
        private readonly IStaticConfig _staticConfig;

        public SecurityService(IOptions<JwtAuthentication> jwtAuthentication, IStaticConfig staticConfig)
        {
            _jwtAuthentication = jwtAuthentication?.Value ?? throw new ArgumentNullException(nameof(jwtAuthentication));
            _staticConfig = staticConfig;

            ValidIssuer = _jwtAuthentication.ValidIssuer;
            ValidAudience = _jwtAuthentication.ValidAudience;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                IssuerSigningKey = _jwtAuthentication.SymmetricSecurityKey,
                ValidAudience = _jwtAuthentication.ValidAudience,
                ValidIssuer = _jwtAuthentication.ValidIssuer
            };

            JwtSecurityToken jwtSecurityToken;
            ClaimsPrincipal principal;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;
                principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
                jwtSecurityToken = securityToken as JwtSecurityToken;
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException("Invalid token", ex);
            }
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public string GenerateToken(IEnumerable<Claim> claims)
        {
            var jwt = new JwtSecurityToken(
                issuer: _jwtAuthentication.ValidIssuer,
                audience: _jwtAuthentication.ValidAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_staticConfig.TokenExpirationMinutes),
                notBefore: DateTime.UtcNow,
                signingCredentials: _jwtAuthentication.SigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public List<Claim> GetUserClaims(string userName, RoleType role)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.UniqueName, userName),
                new Claim(JwtRegisteredClaimNames.Typ, Guid.NewGuid().ToString())
            };

            claims.Add(new Claim(ClaimTypes.Name, userName));
            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));

            return claims;
        }

        public string ValidIssuer { get; private set;}
        public string ValidAudience { get; private set; }

    }
}
