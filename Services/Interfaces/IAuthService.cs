using MilkCollector.API.DTOs.Auth;
using MilkCollector.API.DTOs.Common;

namespace MilkCollector.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponseDto<string>> RegisterAsync(RegisterRequestDto dto);
        Task<ApiResponseDto<AuthResponseDto>> UserLoginAsync(LoginRequestDto dto);
        Task<ApiResponseDto<AuthResponseDto>> ManagerLoginAsync(ManagerLoginRequestDto dto);
    }
}
