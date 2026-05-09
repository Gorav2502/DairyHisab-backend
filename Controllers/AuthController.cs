using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkCollector.API.DTOs.Auth;
using MilkCollector.API.DTOs.Common;
using MilkCollector.API.Services.Interfaces;

namespace MilkCollector.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _authService.UserLoginAsync(dto);
            if (!result.Success)
            {
                if (result.Message.Contains("pending approval") || result.Message.Contains("disapproved"))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, result);
                }
                return Unauthorized(result);
            }
            return Ok(result);
        }

        [HttpPost("manager-login")]
        [AllowAnonymous]
        public async Task<IActionResult> ManagerLogin([FromBody] ManagerLoginRequestDto dto)
        {
            var result = await _authService.ManagerLoginAsync(dto);
            if (!result.Success)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }
    }
}
