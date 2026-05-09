using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkCollector.API.DTOs.Collections;
using MilkCollector.API.Services.Interfaces;

namespace MilkCollector.API.Controllers
{
    [ApiController]
    [Route("api/collections")]
    [Authorize]
    public class CollectionsController : ControllerBase
    {
        private readonly ICollectionService _collectionService;

        public CollectionsController(ICollectionService collectionService)
        {
            _collectionService = collectionService;
        }

        [HttpGet("shift-values")]
        public async Task<IActionResult> GetShiftValues([FromQuery] string date, [FromQuery] string shift)
        {
            var result = await _collectionService.GetShiftValuesAsync(date, shift);
            return Ok(result);
        }

        [HttpGet("rate-for-date")]
        public async Task<IActionResult> GetRateForDate([FromQuery] string date, [FromQuery] int farmerId)
        {
            var result = await _collectionService.GetRateForDateAsync(date, farmerId);
            return Ok(result);
        }

        [HttpPut("shift")]
        public async Task<IActionResult> UpsertShift([FromBody] ShiftUpsertDto dto)
        {
            var result = await _collectionService.UpsertShiftAsync(dto);
            return Ok(result);
        }

        [HttpGet("dashboard-today")]
        public async Task<IActionResult> GetDashboardToday([FromQuery] string? date)
        {
            var result = await _collectionService.GetDashboardTodayAsync(date);
            return Ok(result);
        }
    }
}
