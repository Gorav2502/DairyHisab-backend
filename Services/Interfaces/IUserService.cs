using MilkCollector.API.DTOs.Users;
using MilkCollector.API.DTOs.Common;

namespace MilkCollector.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponseDto<List<UserDto>>> GetUsersAsync(string? search, string? status);
        Task<ApiResponseDto<string>> ApproveUserAsync(int id);
        Task<ApiResponseDto<string>> DisapproveUserAsync(int id);
        Task<ApiResponseDto<ManagerStatsDto>> GetManagerStatsAsync();
    }
}
