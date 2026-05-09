using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkCollector.API.Services.Interfaces;

namespace MilkCollector.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "manager")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? status)
        {
            var result = await _userService.GetUsersAsync(search, status);
            return Ok(result);
        }

        [HttpPatch("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var result = await _userService.ApproveUserAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPatch("{id}/disapprove")]
        public async Task<IActionResult> Disapprove(int id)
        {
            var result = await _userService.DisapproveUserAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var result = await _userService.GetManagerStatsAsync();
            return Ok(result);
        }
    }
}
