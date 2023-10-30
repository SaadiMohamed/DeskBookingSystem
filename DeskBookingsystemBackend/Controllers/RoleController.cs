using drivers.api.Configurations;
using drivers.api.Models.DTO_s;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace drivers.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public RoleController(ILogger<RoleController> logger,
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<JwtConfig> _optionsMonitor)
        {
            _logger = logger;
            _userManager = userManager;
            _jwtConfig = _optionsMonitor.CurrentValue;
        }



        [HttpPut]
        [Route("ChangeRole")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> ChangeRole([FromBody] string email)
        {
            if (email == null)
            {
                return BadRequest("Invalid request (no email)");
            }

            var emailExist = await _userManager.FindByEmailAsync(email);

            if (emailExist == null)
            {
                return BadRequest("Email does not exist");
            }

            //check if the role is user because if he is an admin already deny it

            var role = _userManager.GetRolesAsync(emailExist).Result[0];

            if (role == "Admin") {
                return BadRequest("User is already an admin");
            }
            else if (role == "User")
            {
                await _userManager.RemoveFromRoleAsync(emailExist, role);
                await _userManager.AddToRoleAsync(emailExist, "Admin");
            }
            return Ok("Role changed successfully to admin!");
        }

    }
}
