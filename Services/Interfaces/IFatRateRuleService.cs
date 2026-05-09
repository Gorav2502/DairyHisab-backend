using MilkCollector.API.DTOs.Rates;
using MilkCollector.API.DTOs.Common;

namespace MilkCollector.API.Services.Interfaces
{
    public interface IFatRateRuleService
    {
        Task<ApiResponseDto<List<FatRateRuleDto>>> GetRulesAsync();
        Task<ApiResponseDto<FatRateRuleDto>> GetRuleByIdAsync(int id);
        Task<ApiResponseDto<int>> CreateRuleAsync(CreateFatRateRuleDto dto);
        Task<ApiResponseDto<string>> UpdateRuleAsync(int id, CreateFatRateRuleDto dto);
        Task<ApiResponseDto<string>> DeleteRuleAsync(int id);
    }
}
