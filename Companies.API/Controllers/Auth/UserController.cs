using Companies.Domain.Abstraction.Services;
using Companies.Domain.Abstraction.Services.Auth;
using Companies.Domain.Services;
using Companies.Domain.Services.Auth;
using Companies.Infrastructure.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Companies.API.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IClaimService _claimService;
        private readonly ITokenService _tokenService;

        public UserController(ITokenService tokenService, IUserService userService, IClaimService claimService)
        {
            _userService = userService;
            _claimService = claimService;
            _tokenService = tokenService;
        }

        [HttpGet]
        [Route("login")]
        public async Task<IActionResult> Login([FromQuery] UserLogin userLogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.Get(userLogin);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var claims = _claimService.GetClaims(user);
                var tokenString = _tokenService.GenerateToken(claims);

                return Ok(tokenString);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to generate token. Error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromQuery] Register newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _userService.AddUser(newUser);

                return Ok("User registered successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to register user. Error: {ex.Message}");
            }
        }

    }
}
