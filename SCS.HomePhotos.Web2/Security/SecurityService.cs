using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SCS.HomePhotos.Model;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SCS.HomePhotos.Web.Security
{
    /// <summary>
    /// Provides security services.
    /// </summary>
    /// <seealso cref="ISecurityService" />
    public class SecurityService : ISecurityService
    {
        private readonly JwtAuthentication _jwtAuthentication;
        private readonly IStaticConfig _staticConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityService"/> class.
        /// </summary>
        /// <param name="jwtAuthentication">The JWT authentication.</param>
        /// <param name="staticConfig">The static configuration.</param>
        /// <exception cref="ArgumentNullException">jwtAuthentication</exception>
        public SecurityService(IOptions<JwtAuthentication> jwtAuthentication, IStaticConfig staticConfig)
        {
            _jwtAuthentication = jwtAuthentication?.Value ?? throw new ArgumentNullException(nameof(jwtAuthentication));
            _staticConfig = staticConfig;

            ValidIssuer = _jwtAuthentication.ValidIssuer;
            ValidAudience = _jwtAuthentication.ValidAudience;
        }

        /// <summary>
        /// Gets the principal from expired token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException">
        /// Invalid token
        /// or
        /// Invalid token
        /// </exception>
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
                ValidIssuer = _jwtAuthentication.ValidIssuer,
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

        /// <summary>
        /// Generates a JWT security token.
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <returns>A JWT security token</returns>
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

        /// <summary>
        /// Generates a refresh token.
        /// </summary>
        /// <returns>The refresh token.</returns>
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <summary>
        /// Gets a user's claims.
        /// </summary>        
        /// <param name="user">The user.</param>
        /// <param name="role">The user role.</param>
        /// <returns>A list of claims.</returns>
        public List<Claim> GetUserClaims(User user, RoleType role)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Typ, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            return claims;
        }

        /// <summary>
        /// Gets the valid issuer.
        /// </summary>
        /// <value>
        /// The valid issuer.
        /// </value>
        public string ValidIssuer { get; private set; }

        /// <summary>
        /// Gets the valid audience.
        /// </summary>
        /// <value>
        /// The valid audience.
        /// </value>
        public string ValidAudience { get; private set; }
    }
}
