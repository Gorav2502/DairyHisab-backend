using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkCollector.API.DTOs.PashuAahar;
using MilkCollector.API.Services.Interfaces;

namespace MilkCollector.API.Controllers
{
    [ApiController]
    [Route("api/pashu-aahar")]
    [Authorize]
    public class PashuAaharController : ControllerBase
    {
        private readonly IPashuAaharService _pashuAaharService;

        public PashuAaharController(IPashuAaharService pashuAaharService)
        {
            _pashuAaharService = pashuAaharService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _pashuAaharService.GetAllAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePashuAaharDto dto)
        {
            var result = await _pashuAaharService.CreateAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreatePashuAaharDto dto)
        {
            var result = await _pashuAaharService.UpdateAsync(id, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _pashuAaharService.DeleteAsync(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
