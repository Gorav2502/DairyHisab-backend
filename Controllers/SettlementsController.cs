using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkCollector.API.DTOs.Settlements;
using MilkCollector.API.Services.Interfaces;

namespace MilkCollector.API.Controllers
{
    [ApiController]
    [Route("api/settlements")]
    [Authorize]
    public class SettlementsController : ControllerBase
    {
        private readonly ISettlementService _settlementService;

        public SettlementsController(ISettlementService settlementService)
        {
            _settlementService = settlementService;
        }

        [HttpGet("preview")]
        public async Task<IActionResult> GetPreview([FromQuery] string start, [FromQuery] string end)
        {
            var result = await _settlementService.GetPreviewAsync(start, end);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSettlementDto dto)
        {
            var result = await _settlementService.CreateSettlementAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("billed-lines")]
        public async Task<IActionResult> GetBilledLines()
        {
            var result = await _settlementService.GetBilledLinesAsync();
            return Ok(result);
        }

        [HttpPatch("{settlementId}/lines/{farmerId}/paid")]
        public async Task<IActionResult> MarkPaid(int settlementId, int farmerId, [FromBody] MarkPaidDto dto)
        {
            var result = await _settlementService.MarkPaidAsync(settlementId, farmerId, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
