using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Web.Models;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("SCS.HomePhotos.Web.Test")]

namespace SCS.HomePhotos.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AccountController : HomePhotosController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;
        private readonly IStaticConfig _staticConfig;
        private readonly ISecurityService _securityService;

        public AccountController(ILogger<AccountController> logger,
            IAccountService accountService,
            IStaticConfig staticConfig,
            ISecurityService securityService)
        {
            _logger = logger;
            _accountService = accountService;
            _staticConfig = staticConfig;
            _securityService = securityService;
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
            var claims = _securityService.GetUserClaims(model.UserName, RoleType.Reader);
            var newJwtToken = _securityService.GenerateToken(claims);
            var newRefreshToken = _securityService.GenerateRefreshToken();

            await _accountService.DeleteAgentRefreshTokens(model.UserName, agentId);
            await _accountService.SaveRefreshToken(model.UserName, newRefreshToken, agentId,
                _securityService.ValidIssuer, _securityService.ValidAudience, DateTime.UtcNow.AddDays(_staticConfig.RefreshTokenExpirationDays));

            return Ok(new TokenResultModel
            {
                Jwt = newJwtToken,
                RefreshToken = newRefreshToken
            });
        }

        [Authorize]
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

            var claims = _securityService.GetUserClaims(model.UserName, result.User.Role);
            var newJwtToken = _securityService.GenerateToken(claims);
            var newRefreshToken = _securityService.GenerateRefreshToken();

            await _accountService.DeleteAgentRefreshTokens(model.UserName, agentId);
            await _accountService.SaveRefreshToken(model.UserName, newRefreshToken, agentId,
                _securityService.ValidIssuer, _securityService.ValidAudience, DateTime.UtcNow.AddDays(_staticConfig.RefreshTokenExpirationDays));

            return Ok(new TokenResultModel
            {
                Jwt = newJwtToken,
                RefreshToken = newRefreshToken
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await _accountService.GetUser(User.Identity.Name);

            return Ok(new Dto.AccountInfo(user));
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]Dto.AccountInfo accountInfo)
        {
            if (accountInfo.Username != User.Identity.Name)
            {
                return Unauthorized();
            }

            var user = await _accountService.UpdateAccount(accountInfo.ToModel());

            return Ok(new Dto.AccountInfo(user));
        }
    }
}