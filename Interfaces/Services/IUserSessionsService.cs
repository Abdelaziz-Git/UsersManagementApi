// Interfaces/Services/IUserSessionsService.cs
using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.UserSessions;

namespace TailorSoftAPI.Interfaces.Services
{
    public interface IUserSessionsService
    {
        Task<ResultDto<Guid>> CreateAsync(CreateUserSessionDto dto);
        Task<ResultDto<UserSessionResponseDto>> GetByIdAsync(Guid sessionId);
        Task<ResultDto<UserSessionResponseDto>> GetByRefreshTokenHashAsync(string refreshTokenHash);
        Task<ResultDto<List<UserSessionResponseDto>>> GetByUserIdAsync(Guid userId, bool activeOnly);
        Task<ResultDto<List<UserSessionResponseDto>>> GetAllAsync(Guid? userId, bool activeOnly);
        Task<ResultDto<bool>> DeleteAsync(Guid sessionId);

        Task<ResultDto<bool>> RevokeByIdAsync(Guid sessionId);
        Task<ResultDto<bool>> RevokeByRefreshTokenHashAsync(string refreshTokenHash);
        Task<ResultDto<int>> RevokeByUserIdAsync(Guid userId);
        Task<ResultDto<int>> RevokeAllExceptCurrentAsync(Guid userId, Guid currentSessionId);
        Task<ResultDto<RotateTokenResultDto>> RotateTokenAsync(RotateTokenRequestDto dto);
        Task<ResultDto<bool>> IsValidAsync(string refreshTokenHash);

        Task<ResultDto<int>> CleanupExpiredTokensAsync();
        Task<ResultDto<int>> GetActiveSessionCountAsync(Guid userId);
        Task<ResultDto<List<SessionStatsDto>>> GetStatsAsync();
    }
}