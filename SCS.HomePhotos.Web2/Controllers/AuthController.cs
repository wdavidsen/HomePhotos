using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Web.Models;
using SCS.HomePhotos.Web.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("SCS.HomePhotos.Web.Test")]

namespace SCS.HomePhotos.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly JwtAuthentication _jwtAuthentication;
        private readonly IAccountService _accountService;
        private readonly IStaticConfig _staticConfig;

        public AuthController(ILogger<AuthController> logger, IOptions<JwtAuthentication> jwtAuthentication, IAccountService accountService, IStaticConfig staticConfig)
        {
            _logger = logger;
            _jwtAuthentication = jwtAuthentication?.Value ?? throw new ArgumentNullException(nameof(jwtAuthentication));
            _accountService = accountService;
            _staticConfig = staticConfig;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agentId = GetAgentIdentifier();
            var result = await _accountService.Authenticate(model.UserName, model.Password);

            if (!result.Success)
            {
                if (result.MustChangePassword)
                {
                    return Conflict(new ProblemModel { Id = "PasswordChangeNeeded", Message = "Please change your password" });
                }
                else
                {
                    return Unauthorized(new ProblemModel { Id = "LoginFailed", Message = "Login failed" });
                }
            }

            var claims = GetUserClaims(model.UserName, result.User.Admin);
            var newJwtToken = GenerateToken(claims);
            var newRefreshToken = GenerateRefreshToken();

            await _accountService.DeleteAgentRefreshTokens(model.UserName, agentId);
            await _accountService.SaveRefreshToken(model.UserName, newRefreshToken, agentId, 
                _jwtAuthentication.ValidIssuer, _jwtAuthentication.ValidAudience, DateTime.UtcNow.AddDays(_staticConfig.RefreshTokenExpirationDays));

            return Ok(new Dto.User(result.User)
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userName = User?.Identity?.Name;

            if (userName == null)
            {
                return BadRequest(ModelState);
            }
            var agentId = GetAgentIdentifier();
            await _accountService.DeleteAgentRefreshTokens(userName, agentId);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody]RefreshModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agentId = GetAgentIdentifier();
            ClaimsPrincipal principal;

            try
            {
                principal = GetPrincipalFromExpiredToken(model.Token);
            }
            catch (SecurityTokenException)
            {
                return Unauthorized();
            }

            var userName = principal.Identity.Name;
            var savedRefreshTokens = new List<UserToken>();

            try
            {
                savedRefreshTokens = await _accountService.GetRefreshTokens(userName, _jwtAuthentication.ValidIssuer, _jwtAuthentication.ValidAudience, agentId);
            }
            catch (InvalidOperationException)
            {
                return Unauthorized();
            }

            if (!(savedRefreshTokens.Any(o => o.Token == model.RefreshToken && o.Expiration > DateTime.UtcNow)))
            {
                return Unauthorized();
            }

            var newJwtToken = GenerateToken(principal.Claims);
            var newRefreshToken = GenerateRefreshToken();

            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_staticConfig.RefreshTokenExpirationDays);
            await _accountService.DeleteRefreshToken(userName, model.RefreshToken);
            await _accountService.SaveRefreshToken(userName, newRefreshToken, agentId, 
                _jwtAuthentication.ValidIssuer, _jwtAuthentication.ValidAudience, refreshTokenExpiration);

            return Ok(new TokenResultModel
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _accountService.Register(model.ToUser(), model.Password);

            if (!result.Success)
            {
                if (result.UserNameTaken)
                {
                    return Conflict(new ProblemModel { Id = "UserNameTaken", Message = "User name is already taken." });
                }
                else if (result.PasswordNotStrong)
                {
                    return BadRequest(new ProblemModel { Id = "PasswordStrength", Message = "Password needs to be stronger." });
                }
                else
                {
                    return Unauthorized();
                }
            }

            var agentId = GetAgentIdentifier();
            var claims = GetUserClaims(model.UserName, false);
            var newJwtToken = GenerateToken(claims);
            var newRefreshToken = GenerateRefreshToken();

            await _accountService.DeleteAgentRefreshTokens(model.UserName, agentId);
            await _accountService.SaveRefreshToken(model.UserName, newRefreshToken, agentId,
                _jwtAuthentication.ValidIssuer, _jwtAuthentication.ValidAudience, DateTime.UtcNow.AddDays(_staticConfig.RefreshTokenExpirationDays));

            return Ok(new TokenResultModel
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agentId = GetAgentIdentifier();
            var result = await _accountService.ChangePassword(model.UserName, model.CurrentPassword, model.NewPassword);

            if (!result.Success)
            {
                return Unauthorized((new ProblemModel { Id = "CurrentPasswordFailed", Message = "Current password validation failed." }));
            }

            var claims = GetUserClaims(model.UserName, result.User.Admin);
            var newJwtToken = GenerateToken(claims);
            var newRefreshToken = GenerateRefreshToken();

            await _accountService.DeleteAgentRefreshTokens(model.UserName, agentId);
            await _accountService.SaveRefreshToken(model.UserName, newRefreshToken, agentId,
                _jwtAuthentication.ValidIssuer, _jwtAuthentication.ValidAudience, DateTime.UtcNow.AddDays(_staticConfig.RefreshTokenExpirationDays));

            return Ok(new TokenResultModel
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken
            });
        }

        internal ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
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

        internal string GenerateToken(IEnumerable<Claim> claims)
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

        internal string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        internal string GetAgentIdentifier()
        { 
            var ipAddress = HttpContext?.Connection?.RemoteIpAddress;

            if (ipAddress != null)
            {
                return ipAddress.ToString();
            }

            var ipHeader = GetHeader("X-Forwarded-For");

            return $"{ipHeader}:{Request.Headers["User-Agent"].ToString()}";
        }

        private string GetHeader(string header)
        {
            // https://stackoverflow.com/a/36316189
            if (HttpContext.Request.Headers.TryGetValue(header, out var value))
            {
                return value
                    .ToString()
                    .TrimEnd(',')
                    .Split(',')
                    .Select(s => s.Trim())
                    .FirstOrDefault();
            }
            return null;
        }

        private List<Claim> GetUserClaims(string userName, bool admin)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.UniqueName, userName),
                new Claim(JwtRegisteredClaimNames.Typ, Guid.NewGuid().ToString())
            };

            if (admin)
            {
                claims.Add(new Claim("Admin", "Admin"));
            }
            return claims;
        }
    }
}