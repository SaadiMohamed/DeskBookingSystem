using drivers.api.Configurations;
using drivers.api.Models.DTO_s;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace drivers.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthManagementController :ControllerBase
    {
        private readonly ILogger<AuthManagementController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AuthManagementController(ILogger<AuthManagementController> logger,
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<JwtConfig> _optionsMonitor,
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _jwtConfig = _optionsMonitor.CurrentValue;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO requestDTO)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(requestDTO.Email);

                if (existingUser == null)
                {
                    return BadRequest("Invalid authentication");
                }

                var passwordValid = await _userManager.CheckPasswordAsync(existingUser, requestDTO.Password);

                if(passwordValid)
                {
                    var token = GenerateJwtToken(existingUser);

                    return Ok(new LoginRequestResponse()
                    {
                        Token = token,
                        Result = true
                    });
                }
                return BadRequest("Invalid authentication");

            }
            return BadRequest("Invalid request payload");

        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var role =  _userManager.GetRolesAsync(user).Result[0];

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(JwtRegisteredClaimNames.Sub, value: user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, value: user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, value: Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDTO requestDTO)
        {
            if (ModelState.IsValid)
            {
                var emailExist = await _userManager.FindByEmailAsync(requestDTO.Email);

                if (emailExist != null)
                {
                    return BadRequest("Email already exist");
                }

                var newUser = new IdentityUser { Email = requestDTO.Email, UserName = requestDTO.Email };

                var isCreated = await _userManager.CreateAsync(newUser, requestDTO.Password);

                //if (requestDTO.Role == "Admin")
                //{
                //    await _userManager.AddToRoleAsync(newUser, "Admin");

                //}
                //else
                //{
                    await _userManager.AddToRoleAsync(newUser, "User");
                //}



                if (isCreated.Succeeded) {
                    // generate token

                    var token = GenerateJwtToken(newUser);

                    return Ok(new RegistrationRequestResponse()
                    {
                        Result = true,
                        Token = token
                    });
                }

                return BadRequest(isCreated.Errors.Select(x => x.Description).ToList());
            };

            return BadRequest("Invalid request payload");
        }

    }
}
