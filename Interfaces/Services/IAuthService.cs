using TailorSoftAPI.DTOs.Authentication;
using TailorSoftAPI.DTOs.Common;

namespace TailorSoftAPI.Interfaces.Services
{
    public interface IAuthService
    {
        public Task<ResultDto<LoginResponseDto>> LoginAsync(LoginRequestDto loginDTO);
        public Task<ResultDto<RefreshResponseDto>> RefreshAsync(RefreshRequestDto refreshDTO);
        public Task<ResultDto<bool>> LogoutAsync(LogoutRequestDto logoutDTO);
    }
}
