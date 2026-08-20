// Services/UserSessionsService.cs
using UsersManagementApi.DTOs.Common;
using UsersManagementApi.DTOs.UserSessions;
using UsersManagementApi.Interfaces.Repositories;
using UsersManagementApi.Interfaces.Services;
using UsersManagementApi.Models;

namespace UsersManagementApi.Services
{
    public class UserSessionsService : IUserSessionsService
    {
        private readonly IUserSessionsRepository _repository;

        public UserSessionsService(IUserSessionsRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository), "User sessions repository cannot be null");
        }

        public async Task<ResultDto<Guid>> CreateAsync(CreateUserSessionDto dto)
        {
            if (dto is null)
                return ResultDto<Guid>.Failure("Create session DTO cannot be null");

            if (dto.UserId == Guid.Empty)
                return ResultDto<Guid>.Failure("User ID is required");

            if (string.IsNullOrWhiteSpace(dto.RefreshTokenHash))
                return ResultDto<Guid>.Failure("Refresh token hash is required");

            if (dto.ExpiryDate <= DateTime.UtcNow)
                return ResultDto<Guid>.Failure("Expiry date must be in the future");

            var sessionId = await _repository.CreateAsync(dto);
            if (sessionId is null || sessionId == Guid.Empty)
                return ResultDto<Guid>.Failure("Failed to create session");

            return ResultDto<Guid>.Success(sessionId.Value);
        }

        public async Task<ResultDto<UserSessionResponseDto>> GetByIdAsync(Guid sessionId)
        {
            if (sessionId == Guid.Empty)
                return ResultDto<UserSessionResponseDto>.Failure("Session ID cannot be empty");

            var session = await _repository.GetByIdAsync(sessionId);
            if (session is null)
                return ResultDto<UserSessionResponseDto>.Failure("Session with ID:" + sessionId + " not found");

            return ResultDto<UserSessionResponseDto>.Success(MapToResponseDto(session));
        }

        public async Task<ResultDto<UserSessionResponseDto>> GetByRefreshTokenHashAsync(string refreshTokenHash)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenHash))
                return ResultDto<UserSessionResponseDto>.Failure("Refresh token hash cannot be empty");

            var session = await _repository.GetByRefreshTokenHashAsync(refreshTokenHash);
            if (session is null)
                return ResultDto<UserSessionResponseDto>.Failure("No active session found for the provided refresh token hash");

            return ResultDto<UserSessionResponseDto>.Success(MapToResponseDto(session));
        }

        public async Task<ResultDto<List<UserSessionResponseDto>>> GetByUserIdAsync(Guid userId, bool activeOnly)
        {
            if (userId == Guid.Empty)
                return ResultDto<List<UserSessionResponseDto>>.Failure("User ID cannot be empty");

            var sessions = await _repository.GetByUserIdAsync(userId, activeOnly);
            var dtos = sessions.Select(MapToResponseDto).ToList();

            if (dtos.Count == 0)
                return ResultDto<List<UserSessionResponseDto>>.Failure("No sessions found for this user");

            return ResultDto<List<UserSessionResponseDto>>.Success(dtos);
        }

        public async Task<ResultDto<List<UserSessionResponseDto>>> GetAllAsync(Guid? userId, bool activeOnly)
        {
            var sessions = await _repository.GetAllAsync(userId, activeOnly);
            var dtos = sessions.Select(MapToResponseDto).ToList();

            if (dtos.Count == 0)
                return ResultDto<List<UserSessionResponseDto>>.Failure("No sessions found");

            return ResultDto<List<UserSessionResponseDto>>.Success(dtos);
        }

        public async Task<ResultDto<bool>> DeleteAsync(Guid sessionId)
        {
            if (sessionId == Guid.Empty)
                return ResultDto<bool>.Failure("Session ID cannot be empty");

            var result = await _repository.DeleteAsync(sessionId);
            if (result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to delete session");
        }

        public async Task<ResultDto<bool>> RevokeByIdAsync(Guid sessionId)
        {
            if (sessionId == Guid.Empty)
                return ResultDto<bool>.Failure("Session ID cannot be empty");

            var result = await _repository.RevokeByIdAsync(sessionId);
            if (result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Session not found or already revoked");
        }

        public async Task<ResultDto<bool>> RevokeByRefreshTokenHashAsync(string refreshTokenHash)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenHash))
                return ResultDto<bool>.Failure("Refresh token hash cannot be empty");

            var result = await _repository.RevokeByRefreshTokenHashAsync(refreshTokenHash);
            if (result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Session not found or already revoked");
        }

        public async Task<ResultDto<int>> RevokeByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return ResultDto<int>.Failure("User ID cannot be empty");

            var count = await _repository.RevokeByUserIdAsync(userId);
            return ResultDto<int>.Success(count);
        }

        public async Task<ResultDto<int>> RevokeAllExceptCurrentAsync(Guid userId, Guid currentSessionId)
        {
            if (userId == Guid.Empty)
                return ResultDto<int>.Failure("User ID cannot be empty");

            if (currentSessionId == Guid.Empty)
                return ResultDto<int>.Failure("Current session ID cannot be empty");

            var count = await _repository.RevokeAllExceptCurrentAsync(userId, currentSessionId);
            return ResultDto<int>.Success(count);
        }

        public async Task<ResultDto<RotateTokenResultDto>> RotateTokenAsync(RotateTokenRequestDto dto)
        {
            if (dto is null)
                return ResultDto<RotateTokenResultDto>.Failure("Rotate token DTO cannot be null");

            if (string.IsNullOrWhiteSpace(dto.OldRefreshTokenHash))
                return ResultDto<RotateTokenResultDto>.Failure("Old refresh token hash is required");

            if (string.IsNullOrWhiteSpace(dto.NewRefreshTokenHash))
                return ResultDto<RotateTokenResultDto>.Failure("New refresh token hash is required");

            if (dto.NewExpiryDate <= DateTime.UtcNow)
                return ResultDto<RotateTokenResultDto>.Failure("New expiry date must be in the future");

            var result = await _repository.RotateTokenAsync(dto);

            // Reuse detection is a valid, meaningful outcome — not a failure to surface as an error.
            if (result.IsReuseDetected)
                return ResultDto<RotateTokenResultDto>.Success(result);

            if (!result.IsRotated)
                return ResultDto<RotateTokenResultDto>.Failure("Token not found or expired");

            return ResultDto<RotateTokenResultDto>.Success(result);
        }

        public async Task<ResultDto<bool>> IsValidAsync(string refreshTokenHash)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenHash))
                return ResultDto<bool>.Failure("Refresh token hash cannot be empty");

            var isValid = await _repository.IsValidAsync(refreshTokenHash);

            // Resolves successfully either way — validity is the answer, not an error state.
            return ResultDto<bool>.Success(isValid);
        }

        public async Task<ResultDto<int>> CleanupExpiredTokensAsync()
        {
            var count = await _repository.CleanupExpiredTokensAsync();
            return ResultDto<int>.Success(count);
        }

        public async Task<ResultDto<int>> GetActiveSessionCountAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return ResultDto<int>.Failure("User ID cannot be empty");

            var count = await _repository.GetActiveSessionCountAsync(userId);
            return ResultDto<int>.Success(count);
        }

        public async Task<ResultDto<List<SessionStatsDto>>> GetStatsAsync()
        {
            var stats = await _repository.GetStatsAsync();
            return ResultDto<List<SessionStatsDto>>.Success(stats);
        }

        #region Mapping Methods

        private UserSessionResponseDto MapToResponseDto(UserSession session)
        {
            return new UserSessionResponseDto
            {
                SessionId = session.SessionId,
                UserId = session.UserId,
                RefreshTokenHash = session.RefreshTokenHash,
                CreatedDate = session.CreatedDate,
                ExpiryDate = session.ExpiryDate,
                IsRevoked = session.IsRevoked,
                RevokedDate = session.RevokedDate
            };
        }

        #endregion
    }
}