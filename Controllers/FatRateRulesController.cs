using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkCollector.API.DTOs.Rates;
using MilkCollector.API.Services.Interfaces;

namespace MilkCollector.API.Controllers
{
    [ApiController]
    [Route("api/fat-rate-rules")]
    [Authorize]
    public class FatRateRulesController : ControllerBase
    {
        private readonly IFatRateRuleService _ruleService;

        public FatRateRulesController(IFatRateRuleService ruleService)
        {
            _ruleService = ruleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _ruleService.GetRulesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _ruleService.GetRuleByIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFatRateRuleDto dto)
        {
            var result = await _ruleService.CreateRuleAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateFatRateRuleDto dto)
        {
            var result = await _ruleService.UpdateRuleAsync(id, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _ruleService.DeleteRuleAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
