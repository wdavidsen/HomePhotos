using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Web.Models;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("SCS.HomePhotos.Web.Test")]

namespace SCS.HomePhotos.Web.Controllers
{
    [Authorize(Policy = "Admins")]
    [Route("api/[controller]")]
    public class UsersController : HomePhotosController
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IAccountService _accountService;
        private readonly IStaticConfig _staticConfig;

        public UsersController(ILogger<UsersController> logger, IAccountService accountService, IStaticConfig staticConfig)
        {
            _logger = logger;
            _accountService = accountService;
            _staticConfig = staticConfig;
        }

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

        [HttpGet("{userId}", Name = "GetUser")]
        public async Task<IActionResult> GetUser([FromRoute]int userId)
        {
            var user = await _accountService.GetUser(userId);

            return Ok(new Dto.User(user));
        }

        [HttpPost("", Name = "AddUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser([FromBody]Dto.PasswordUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            var updatedUser = await _accountService.SaveUser(user.ToModel(), user.Password);

            return Ok(new Dto.User(updatedUser));
        }

        [HttpPut("{userId}", Name = "UpdateUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser([FromBody]Dto.User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            var updatedUser = await _accountService.SaveUser(user.ToModel());

            return Ok(new Dto.User(updatedUser));
        }

        [HttpDelete("{userId}", Name = "DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser([FromRoute]int userId)
        {
            await _accountService.DeleteUser(userId);

            return Ok();
        }

        [HttpPost("{userId}/resetPassword", Name = "ResetPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword([FromRoute] int userId, [FromBody] ResetPasswordModel resetPasswordModel)
        {
            var user = await _accountService.GetUser(userId);

            if (user ==  null)
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