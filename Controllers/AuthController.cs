using BookFlix.API.Models.DTO;
using BookFlix.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookFlix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<IdentityUser> userManager, ITokenService tokenService)
        {
            this._userManager = userManager;
            this._tokenService = tokenService;
        }

        //post: api/auth/register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.EmailAddress,
                Email = registerRequestDto.EmailAddress
            };

            var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDto.Password);
            if (identityResult.Succeeded)
            {
                //add roles to this user
                if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                {
                    identityResult =  await _userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);
                    if (identityResult.Succeeded)
                    {
                        return Ok("User has been created successfully! Please login now.");
                    }
                }

            }

            return BadRequest("Something went wrong. Please re-check and try again.");
        }

        //POST: api/auth/login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.UserName);

            if (user!=null)
            {
                var checkPasswordResult = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if(checkPasswordResult)
                {
                    //fetch roles of the user
                    var roles = await _userManager.GetRolesAsync(user);
                    if(roles!=null)
                    {
                        //create Token
                        var jwtToken = _tokenService.CreateJWTToken(user, roles.ToList());
                        var response = new LoginResponseDto
                        {
                            JWTToken = jwtToken
                        };
                        return Ok(response);
                    }
                }
            }

            return BadRequest("UserName or Password might be incorrect.");
        }
    }
}
