using MilkCollector.API.DTOs.Farmers;
using MilkCollector.API.DTOs.Common;

namespace MilkCollector.API.Services.Interfaces
{
    public interface IFarmerService
    {
        Task<ApiResponseDto<PagedResultDto<FarmerDto>>> GetFarmersAsync(bool? activeOnly, string? search, int pageNumber, int pageSize);
        Task<ApiResponseDto<FarmerDto>> GetFarmerByIdAsync(int id);
        Task<ApiResponseDto<int>> CreateFarmerAsync(CreateFarmerDto dto);
        Task<ApiResponseDto<string>> UpdateFarmerAsync(int id, UpdateFarmerDto dto);
        Task<ApiResponseDto<string>> DeleteFarmerAsync(int id);
    }
}
