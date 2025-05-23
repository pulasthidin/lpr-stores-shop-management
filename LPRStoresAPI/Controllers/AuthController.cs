using LPRStoresAPI.DTOs;
using LPRStoresAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LPRStoresAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterAsync(registerDto);
            if (!result.IsSuccess)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new { result.Message }); 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.LoginAsync(loginDto);
            if (!result.IsSuccess || string.IsNullOrEmpty(result.Token))
            {
                return Unauthorized(new { result.Message });
            }
            return Ok(result); 
        }
    }
}
