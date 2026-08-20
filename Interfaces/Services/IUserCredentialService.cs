using UsersManagementApi.DTOs.Common;
using UsersManagementApi.DTOs.UserCredentials;

namespace UsersManagementApi.Interfaces.Services
{
    public interface IUserCredentialService
    {
        Task<ResultDto<Guid>> CreateAsync(CreateUserCredentialDto dto);
        Task<ResultDto<UserCredentialResponseDto>> GetByUserIdAsync(Guid userId);
        Task<ResultDto<bool>> IncrementFailedLoginAttemptsAsync(Guid userId, FailedLoginRequestDto dto);
        Task<ResultDto<bool>> IsAccountLockedAsync(Guid userId);
        Task<ResultDto<bool>> ResetFailedLoginAttemptsAsync(Guid userId);
        Task<ResultDto<bool>> UpdatePasswordAsync(Guid userId, UpdateUserCredentialDto dto);
    }
}