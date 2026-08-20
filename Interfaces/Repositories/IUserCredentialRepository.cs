using UsersManagementApi.DTOs.UserCredentials;
using UsersManagementApi.DTOs.Common;
using UsersManagementApi.Models;

namespace UsersManagementApi.Interfaces.Repositories;
public interface IUserCredentialRepository
{
    Task<Guid?> CreateAsync(CreateUserCredentialDto dto);
    Task<UserCredential?> GetByUserIdAsync(Guid userId);
    Task<bool> IncrementFailedLoginAttemptsAsync(Guid userId, FailedLoginRequestDto dto);
    Task<bool> IsAccountLocked(Guid userId);
    Task<bool> ResetFailedLoginAttempts(Guid userId);
    Task<bool> UpdatePasswordAsync(Guid userId, UpdateUserCredentialDto dto);

}


