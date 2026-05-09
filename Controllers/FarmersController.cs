using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkCollector.API.DTOs.Farmers;
using MilkCollector.API.Services.Interfaces;

namespace MilkCollector.API.Controllers
{
    [ApiController]
    [Route("api/farmers")]
    [Authorize]
    public class FarmersController : ControllerBase
    {
        private readonly IFarmerService _farmerService;

        public FarmersController(IFarmerService farmerService)
        {
            _farmerService = farmerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] bool? activeOnly, 
            [FromQuery] string? search, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            var result = await _farmerService.GetFarmersAsync(activeOnly, search, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _farmerService.GetFarmerByIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFarmerDto dto)
        {
            var result = await _farmerService.CreateFarmerAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFarmerDto dto)
        {
            var result = await _farmerService.UpdateFarmerAsync(id, dto);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(new { ok = true, message = result.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _farmerService.DeleteFarmerAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(new { ok = true, message = result.Message });
        }
    }
}
