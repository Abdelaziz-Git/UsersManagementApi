using TailorSoftAPI.DTOs.UserSessions;
using TailorSoftAPI.Models;

namespace TailorSoftAPI.Interfaces.Repositories
{
    public interface IUserSessionsRepository
    {
        Task<Guid?> CreateAsync(CreateUserSessionDto dto);
        Task<UserSession?> GetByIdAsync(Guid sessionId);
        Task<UserSession?> GetByRefreshTokenHashAsync(string refreshTokenHash);
        Task<List<UserSession>> GetByUserIdAsync(Guid userId, bool activeOnly);
        Task<List<UserSession>> GetAllAsync(Guid? userId, bool activeOnly);
        Task<bool> DeleteAsync(Guid sessionId);

        Task<bool> RevokeByIdAsync(Guid sessionId);
        Task<bool> RevokeByRefreshTokenHashAsync(string refreshTokenHash);
        Task<int> RevokeByUserIdAsync(Guid userId);
        Task<int> RevokeAllExceptCurrentAsync(Guid userId, Guid currentSessionId);
        Task<RotateTokenResultDto> RotateTokenAsync(RotateTokenRequestDto dto);
        Task<bool> IsValidAsync(string refreshTokenHash);

        Task<int> CleanupExpiredTokensAsync();
        Task<int> GetActiveSessionCountAsync(Guid userId);
        Task<List<SessionStatsDto>> GetStatsAsync();
    }
}