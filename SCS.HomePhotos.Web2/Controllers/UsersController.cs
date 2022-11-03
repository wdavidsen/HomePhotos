using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Web.Models;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("SCS.HomePhotos.Web.Test")]

namespace SCS.HomePhotos.Web.Controllers
{
    /// <summary>User services.</summary>
    [Authorize(Policy = "Admins")]
    [Route("api/[controller]")]
    public class UsersController : HomePhotosController
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IAccountService _accountService;
        private readonly IStaticConfig _staticConfig;

        /// <summary>Initializes a new instance of the <see cref="UsersController" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="accountService">The account service.</param>
        /// <param name="staticConfig">The static configuration.</param>
        public UsersController(ILogger<UsersController> logger, IAccountService accountService, IStaticConfig staticConfig)
        {
            _logger = logger;
            _accountService = accountService;
            _staticConfig = staticConfig;
        }

        /// <summary>Gets all users.</summary>
        /// <returns>A user list.</returns>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dto.User))]
        [HttpGet("", Name = "GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var userDtoList = new List<Dto.User>();

            foreach (var user in await _accountService.GetUsers())
            {
                userDtoList.Add(new Dto.User(user));
            }

            return Ok(userDtoList);
        }

        /// <summary>Gets a user.</summary>
        /// <param name="userId">The user id.</param>
        /// <returns>A user.</returns>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dto.User))]
        [HttpGet("{userId}", Name = "GetUser")]
        public async Task<IActionResult> GetUser([FromRoute] int userId)
        {
            var user = await _accountService.GetUser(userId);

            return Ok(new Dto.User(user));
        }

        /// <summary>Adds a user.</summary>
        /// <param name="user">The user to add.</param>
        /// <returns>The new user.</returns>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dto.User))]
        [HttpPost("", Name = "AddUser")]        
        public async Task<IActionResult> AddUser([FromBody] Dto.PasswordUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            var updatedUser = await _accountService.SaveUser(user.ToModel(), user.Password);

            return Ok(new Dto.User(updatedUser));
        }

        /// <summary>Updates a user.</summary>
        /// <param name="user">The user to update.</param>
        /// <returns>The updated user.</returns>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dto.User))]
        [HttpPut("{userId}", Name = "UpdateUser")]        
        public async Task<IActionResult> UpdateUser([FromBody] Dto.User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            var updatedUser = await _accountService.SaveUser(user.ToModel());

            return Ok(new Dto.User(updatedUser));
        }

        /// <summary>Deletes a user.</summary>
        /// <param name="userId">The user id.</param>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("{userId}", Name = "DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromRoute] int userId)
        {
            await _accountService.DeleteUser(userId);

            return Ok();
        }

        /// <summary>Resets a user's password.</summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resetPasswordModel">The reset password model.</param>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("{userId}/resetPassword", Name = "ResetPassword")]        
        public async Task<IActionResult> ResetPassword([FromRoute] int userId, [FromBody] ResetPasswordModel resetPasswordModel)
        {
            var user = await _accountService.GetUser(userId);

            if (user == null)
            {
                return NotFound();
            }

            if (user.UserName != resetPasswordModel.UserName)
            {
                return BadRequest();
            }

            var result = await _accountService.ResetPassword(resetPasswordModel.UserName, resetPasswordModel.NewPassword);

            if (!result.Success)
            {
                if (result.PasswordNotStrong)
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
    }
}