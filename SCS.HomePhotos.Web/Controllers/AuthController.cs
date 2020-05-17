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
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("SCS.HomePhotos.Web.Test")]

namespace SCS.HomePhotos.Web.Controllers
{    
    [Route("api/[controller]")]
    public class AuthController : HomePhotosController
    {
        private readonly ILogger<AuthController> _logger;
        private readonly JwtAuthentication _jwtAuthentication;
        private readonly IAccountService _accountService;
        private readonly IStaticConfig _staticConfig;
        private readonly ISecurityService _securityService;

        public AuthController(ILogger<AuthController> logger, 
            IOptions<JwtAuthentication> jwtAuthentication, 
            IAccountService accountService, 
            IStaticConfig staticConfig,
            ISecurityService securityService)
        {
            _logger = logger;
            _jwtAuthentication = jwtAuthentication?.Value ?? throw new ArgumentNullException(nameof(jwtAuthentication));
            _accountService = accountService;
            _staticConfig = staticConfig;
            _securityService = securityService;
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

            var claims = _securityService.GetUserClaims(model.UserName, result.User.Role);
            var newJwtToken = _securityService.GenerateToken(claims);
            var newRefreshToken = _securityService.GenerateRefreshToken();

            await _accountService.DeleteAgentRefreshTokens(model.UserName, agentId);
            await _accountService.SaveRefreshToken(model.UserName, newRefreshToken, agentId,
                _jwtAuthentication.ValidIssuer, _jwtAuthentication.ValidAudience, DateTime.UtcNow.AddDays(_staticConfig.RefreshTokenExpirationDays));

            return Ok(new Dto.TokenUser(result.User)
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userName = User?.Identity?.Name;

            if (userName == null)
            {
                return new ForbidResult();
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
                principal = _securityService.GetPrincipalFromExpiredToken(model.Token);
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

            var newJwtToken = _securityService.GenerateToken(principal.Claims);
            var newRefreshToken = _securityService.GenerateRefreshToken();

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
    }
}