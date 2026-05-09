using MilkCollector.API.DTOs.Settlements;
using MilkCollector.API.DTOs.Common;

namespace MilkCollector.API.Services.Interfaces
{
    public interface ISettlementService
    {
        Task<ApiResponseDto<SettlementPreviewDto>> GetPreviewAsync(string start, string end);
        Task<ApiResponseDto<int>> CreateSettlementAsync(CreateSettlementDto dto);
        Task<ApiResponseDto<List<BilledLineDto>>> GetBilledLinesAsync();
        Task<ApiResponseDto<string>> MarkPaidAsync(int settlementId, int farmerId, MarkPaidDto dto);
    }
}
