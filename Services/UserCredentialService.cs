using TailorSoftAPI.DTOs.UserCredentials;
using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.Interfaces.Repositories;
using TailorSoftAPI.Interfaces.Services;
using TailorSoftAPI.Models;

namespace TailorSoftAPI.Services
{
    public class UserCredentialService : IUserCredentialService
    {
        private readonly IUserCredentialRepository _userCredentialRepository;

        public UserCredentialService(IUserCredentialRepository userCredentialRepository)
        {
            _userCredentialRepository = userCredentialRepository ?? throw new ArgumentNullException(nameof(userCredentialRepository), "User credential repository cannot be null");
        }

        public async Task<ResultDto<Guid>> CreateAsync(CreateUserCredentialDto dto)
        {
            if (dto is null)
                return ResultDto<Guid>.Failure("Create user credential DTO cannot be null");

            ValidateCreateUserCredentialDto(dto, out var validationError);
            if (validationError != null)
                return ResultDto<Guid>.Failure(validationError);

            var result = await _userCredentialRepository.CreateAsync(dto);
            if (result is null || result == Guid.Empty)
                return ResultDto<Guid>.Failure("Failed to create user credential");

            return ResultDto<Guid>.Success(result.Value);
        }

        public async Task<ResultDto<UserCredentialResponseDto>> GetByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return ResultDto<UserCredentialResponseDto>.Failure("User ID cannot be empty");

            var credential = await _userCredentialRepository.GetByUserIdAsync(userId);

            if (credential == null)
                return ResultDto<UserCredentialResponseDto>.Failure($"User credential with user ID {userId} not found");

            return ResultDto<UserCredentialResponseDto>.Success(MapToUserCredentialResponseDto(credential));
        }

        public async Task<ResultDto<bool>> IncrementFailedLoginAttemptsAsync(Guid userId, FailedLoginRequestDto dto)
        {
            if (userId == Guid.Empty)
                return ResultDto<bool>.Failure("User ID cannot be empty");

            if (dto is null)
                return ResultDto<bool>.Failure("Failed login request DTO cannot be null");

            var result = await _userCredentialRepository.IncrementFailedLoginAttemptsAsync(userId, dto);

            if (result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to increment failed login attempts");
        }

        public async Task<ResultDto<bool>> IsAccountLockedAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return ResultDto<bool>.Failure("User ID cannot be empty");

            var result = await _userCredentialRepository.IsAccountLocked(userId);
            return ResultDto<bool>.Success(result);
        }

        public async Task<ResultDto<bool>> ResetFailedLoginAttemptsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return ResultDto<bool>.Failure("User ID cannot be empty");

            var result = await _userCredentialRepository.ResetFailedLoginAttempts(userId);

            if (result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to reset failed login attempts");
        }

        public async Task<ResultDto<bool>> UpdatePasswordAsync(Guid userId, UpdateUserCredentialDto dto)
        {
            if (userId == Guid.Empty)
                return ResultDto<bool>.Failure("User ID cannot be empty");

            if (dto is null)
                return ResultDto<bool>.Failure("Update user credential DTO cannot be null");
            ValidatePasswordHash(dto.PasswordHash, out var passwordHashError);
            if (passwordHashError != null)
                return ResultDto<bool>.Failure(passwordHashError);

            var result = await _userCredentialRepository.UpdatePasswordAsync(userId, dto);

            if (result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to update password");
        }

        #region Mapping Methods

        private UserCredentialResponseDto MapToUserCredentialResponseDto(UserCredential userCredential)
        {
            return new UserCredentialResponseDto
            {
                CredentialId = userCredential.CredentialId,
                UserId = userCredential.UserId,
                PasswordHash = userCredential.PasswordHash,
                LastPasswordChangeDate = userCredential.LastPasswordChangeDate,
                FailedLoginAttempts = userCredential.FailedLoginAttempts,
                AccountLockedUntil = userCredential.AccountLockedUntil,
                UpdatedDate = userCredential.UpdatedDate
            };
        }

        #endregion

        #region Validation Methods

        private void ValidateCreateUserCredentialDto(CreateUserCredentialDto dto, out string? error)
        {
            if (dto.UserId == Guid.Empty)
            {
                error = "User ID cannot be empty";
                return;
            }
            ValidatePasswordHash(dto.PasswordHash, out var passwordHashError);
            if(passwordHashError != null)
            {
                error = passwordHashError;
                return;
            }
            error = null;
        }
        private void ValidatePasswordHash(string passwordHash, out string? error)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                error = "Password hash cannot be null or empty";
                return;
            }
            error = null;
        }

        #endregion
    }
}