using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Web.Models;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("SCS.HomePhotos.Web.Test")]

namespace SCS.HomePhotos.Web.Controllers
{
    /// <summary>Account services.</summary>
    [Authorize]
    [Route("api/[controller]")]
    public class AccountController : HomePhotosController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;
        private readonly IStaticConfig _staticConfig;
        private readonly ISecurityService _securityService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IDynamicConfig _dynamicConfig;

        /// <summary>Initializes a new instance of the <see cref="AccountController" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="accountService">The account service.</param>
        /// <param name="staticConfig">The static configuration.</param>
        /// <param name="securityService">The security service.</param>
        /// <param name="fileUploadService">The file upload service.</param>
        /// <param name="dynamicConfig">The dynamic configuration.</param>
        public AccountController(ILogger<AccountController> logger,
            IAccountService accountService,
            IStaticConfig staticConfig,
            ISecurityService securityService,
            IFileUploadService fileUploadService,
            IDynamicConfig dynamicConfig)
        {
            _logger = logger;
            _accountService = accountService;
            _staticConfig = staticConfig;
            _securityService = securityService;
            _fileUploadService = fileUploadService;
            _dynamicConfig = dynamicConfig;
        }

        /// <summary>Registers a user.</summary>
        /// <param name="model">The register model.</param>
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
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
                    return BadRequest();
                }
            }

            return Ok();
        }

        /// <summary>Changes a user's password.</summary>
        /// <param name="model">The change passeord model.</param>
        /// <returns>A new JWT and refresh token.</returns>
        [Authorize]
        [ValidateAntiForgeryToken]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResultModel))]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            var agentId = GetAgentIdentifier();
            var result = await _accountService.ChangePassword(model.UserName, model.CurrentPassword, model.NewPassword);

            if (!result.Success)
            {
                if (result.PasswordMismatch)
                {
                    return BadRequest(new ProblemModel { Id = "CurrentPasswordFailed", Message = "Current password validation failed." });
                }
                else if (result.PasswordNotStrong)
                {
                    return BadRequest(new ProblemModel { Id = "PasswordStrength", Message = "Password needs to be stronger." });
                }
                else
                {
                    return BadRequest();
                }
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

        /// <summary>Updates the avatar image.</summary>
        /// <param name="model">The avatar model.</param>
        /// <returns>The avatar image name.</returns>
        [Authorize]
        [ValidateAntiForgeryToken]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut("updateAvatar")]
        public async Task<IActionResult> UpdateAvatar([FromForm] AvatarModel model)
        {
            string fileName = "";
            string filePath = "";
            string newFileName = "";

            try
            {
                fileName = model.Image.GetFileName();

                var extension = Path.GetExtension(fileName);

                #region Validations

                // check for invalid characters
                if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    return BadRequest();
                }

                // file name should not exceed 255 characters
                if (fileName.Length > 255)
                {
                    return BadRequest();
                }

                // check for valid extensions
                if (!Constants.AcceptedExtensions.Any(e => e == extension.ToUpper()))
                {
                    return BadRequest();
                }

                // check image header bytes
                using (var sourceStream = model.Image.OpenReadStream())
                {
                    if (!ImageValidationHelper.ValidateImageHeaders(sourceStream))
                    {
                        return BadRequest();
                    }
                }
                #endregion

                newFileName = Guid.NewGuid().ToString() + extension;
                filePath = Path.Combine(_dynamicConfig.CacheFolder, Constants.AvatarFolder, newFileName);

                _fileUploadService.CreateDirectory(filePath);
                await _fileUploadService.CopyFile(model.Image, filePath, FileMode.Create);

                var user = await _accountService.GetUser(User.Identity.Name);
                user.AvatarImage = newFileName;
                await _accountService.SaveUser(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to save avatar profile image: {fileName}");
                throw;
            }

            return Ok(new { avatarImage = newFileName });
        }

        /// <summary>Gets current user account info.</summary>
        /// <returns>User's account info.</returns>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dto.AccountInfo))]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await _accountService.GetUser(User.Identity.Name);

            return Ok(new Dto.AccountInfo(user));
        }

        /// <summary>Updates current user's account information.</summary>
        /// <param name="accountInfo">The account information.</param>
        /// <returns>User's account info.</returns>
        [Authorize]
        [ValidateAntiForgeryToken]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dto.AccountInfo))]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Dto.AccountInfo accountInfo)
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