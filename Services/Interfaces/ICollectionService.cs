using MilkCollector.API.DTOs.Collections;
using MilkCollector.API.DTOs.Common;

namespace MilkCollector.API.Services.Interfaces
{
    public interface ICollectionService
    {
        Task<ApiResponseDto<ShiftValuesResponseDto>> GetShiftValuesAsync(string date, string shift);
        Task<ApiResponseDto<RateForDateResponseDto>> GetRateForDateAsync(string date, int farmerId);
        Task<ApiResponseDto<string>> UpsertShiftAsync(ShiftUpsertDto dto);
        Task<ApiResponseDto<DashboardTodayDto>> GetDashboardTodayAsync(string? date);
    }
}
