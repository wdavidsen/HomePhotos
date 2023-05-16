using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Web.Models;
using SCS.HomePhotos.Web.Security;

using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;

[assembly: InternalsVisibleTo("SCS.HomePhotos.Web.Test")]

namespace SCS.HomePhotos.Web.Controllers
{
    /// <summary>Authentication services.</summary>
    [Route("api/[controller]")]
    public class AuthController : HomePhotosController
    {
        private readonly ILogger<AuthController> _logger;
        private readonly JwtAuthentication _jwtAuthentication;
        private readonly IAccountService _accountService;
        private readonly IStaticConfig _staticConfig;
        private readonly ISecurityService _securityService;

        /// <summary>Initializes a new instance of the <see cref="AuthController" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="jwtAuthentication">The JWT authentication.</param>
        /// <param name="accountService">The account service.</param>
        /// <param name="staticConfig">The static configuration.</param>
        /// <param name="securityService">The security service.</param>
        /// <exception cref="ArgumentNullException">jwtAuthentication</exception>
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

        /// <summary>User log-in.</summary>
        /// <param name="model">The login model.</param>
        /// <returns>A JWT and refresh token.</returns>
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dto.TokenUser))]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
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
                    return BadRequest(new ProblemModel { Id = "LoginFailed", Message = "Incorrect Username/Password" });
                }
            }

            var claims = _securityService.GetUserClaims(result.User, result.User.Role);
            var newJwtToken = _securityService.GenerateToken(claims);
            var newRefreshToken = _securityService.GenerateRefreshToken();

            await _accountService.DeleteAgentRefreshTokens(model.UserName, agentId);
            await _accountService.SaveRefreshToken(model.UserName, newRefreshToken, agentId,
                _jwtAuthentication.ValidIssuer, _jwtAuthentication.ValidAudience, DateTime.UtcNow.AddDays(_staticConfig.RefreshTokenExpirationDays));

            return Ok(new Dto.TokenUser(result.User)
            {
                Jwt = newJwtToken,
                RefreshToken = newRefreshToken
            });
        }

        /// <summary>User log-in with password change.</summary>
        /// <param name="model">The user/change password model.</param>
        /// <returns>A JWT and refresh token.</returns>
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dto.TokenUser))]
        [HttpPost("loginWithPasswordChange")]
        public async Task<IActionResult> LoginWithPasswordChange([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            var agentId = GetAgentIdentifier();
            var loginResult = await _accountService.Authenticate(model.UserName, model.CurrentPassword);

            if (!(loginResult.Success || loginResult.MustChangePassword))
            {
                return BadRequest(new ProblemModel { Id = "LoginFailed", Message = "Incorrect Username/Password" });
            }

            var changePasswordResult = await _accountService.ChangePassword(model.UserName, model.CurrentPassword, model.NewPassword);

            if (!changePasswordResult.Success)
            {
                return BadRequest((new ProblemModel { Id = "CurrentPasswordFailed", Message = "Current password validation failed." }));
            }

            var claims = _securityService.GetUserClaims(loginResult.User, loginResult.User.Role);
            var newJwtToken = _securityService.GenerateToken(claims);
            var newRefreshToken = _securityService.GenerateRefreshToken();

            await _accountService.DeleteAgentRefreshTokens(model.UserName, agentId);
            await _accountService.SaveRefreshToken(model.UserName, newRefreshToken, agentId,
                _jwtAuthentication.ValidIssuer, _jwtAuthentication.ValidAudience, DateTime.UtcNow.AddDays(_staticConfig.RefreshTokenExpirationDays));

            return Ok(new Dto.TokenUser(loginResult.User)
            {
                Jwt = newJwtToken,
                RefreshToken = newRefreshToken
            });
        }

        /// <summary>Logs-out current user.</summary>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("logout")]
        [SuppressMessage("Security", "SCS0016:Controller method is vulnerable to CSRF", Justification = "Anti-forgery detection deemed unnecessary for logging out.")]
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

        /// <summary>Refreshes a user's auth token.</summary>
        /// <param name="model">The refresh model.</param>
        /// <returns>A JWT and refresh token.</returns>
        [AllowAnonymous]        
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResultModel))]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            var agentId = GetAgentIdentifier();
            ClaimsPrincipal principal;

            try
            {
                principal = _securityService.GetPrincipalFromExpiredToken(model.Jwt);
            }
            catch (SecurityTokenException)
            {
                return Unauthorized();
            }

            var claim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == JwtRegisteredClaimNames.Sub);

            if (claim == null)
            {
                return Unauthorized();
            }

            var username = claim.Value;
            var savedRefreshTokens = new List<UserToken>();

            try
            {
                savedRefreshTokens = await _accountService.GetRefreshTokens(username, _jwtAuthentication.ValidIssuer, _jwtAuthentication.ValidAudience, agentId);
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
            await _accountService.DeleteRefreshToken(username, model.RefreshToken);
            await _accountService.SaveRefreshToken(username, newRefreshToken, agentId,
                _jwtAuthentication.ValidIssuer, _jwtAuthentication.ValidAudience, refreshTokenExpiration);

            return Ok(new TokenResultModel
            {
                Jwt = newJwtToken,
                RefreshToken = newRefreshToken
            });
        }

        /// <summary>
        /// Returns the password requirements for new users.
        /// </summary>
        /// <returns>Password requiements</returns>
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("pass-requirements")]
        public IActionResult PasswordRequirements()
        {
            return Ok(_staticConfig.PasswordRequirements);
        }
    }
}