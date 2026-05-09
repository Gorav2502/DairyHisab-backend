using MilkCollector.API.DTOs.Common;
using MilkCollector.API.DTOs.PashuAahar;

namespace MilkCollector.API.Services.Interfaces
{
    public interface IPashuAaharService
    {
        Task<ApiResponseDto<List<PashuAaharDto>>> GetAllAsync();
        Task<ApiResponseDto<int>> CreateAsync(CreatePashuAaharDto dto);
        Task<ApiResponseDto<string>> UpdateAsync(int id, CreatePashuAaharDto dto);
        Task<ApiResponseDto<string>> DeleteAsync(int id);
    }
}
