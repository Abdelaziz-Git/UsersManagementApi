using TailorSoftAPI.DTOs.UserCredentials;
using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.Models;

namespace TailorSoftAPI.Interfaces.Repositories;
public interface IUserCredentialRepository
{
    Task<Guid?> CreateAsync(CreateUserCredentialDto dto);
    Task<UserCredential?> GetByUserIdAsync(Guid userId);
    Task<bool> IncrementFailedLoginAttemptsAsync(Guid userId, FailedLoginRequestDto dto);
    Task<bool> IsAccountLocked(Guid userId);
    Task<bool> ResetFailedLoginAttempts(Guid userId);
    Task<bool> UpdatePasswordAsync(Guid userId, UpdateUserCredentialDto dto);

}


