namespace TailorSoftAPI.DTOs.UserSessions
{
    public class CreateUserSessionDto
    {
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
    }
    /// <summary>
    /// Full session projection. Contains the raw RefreshToken —
    /// only surface this through admin/internal-facing endpoints.
    /// </summary>
    public class UserSessionResponseDto
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? RevokedDate { get; set; }
    }
    public class RotateTokenRequestDto
    {
        public string OldRefreshToken { get; set; } = string.Empty;
        public string NewRefreshToken { get; set; } = string.Empty;
        public DateTime NewExpiryDate { get; set; }
    }
    public class RotateTokenResultDto
    {
        public Guid? UserId { get; set; }
        public bool IsRotated { get; set; }
        public bool IsReuseDetected { get; set; }
    }

    public class RevokeAllExceptCurrentDto
    {
        public Guid CurrentSessionId { get; set; }
    }
    /// <summary>Slim request DTO for token-scoped operations (validate, revoke-by-token, lookup).</summary>
    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
    /// <summary>Slim projection DTO for the aggregate stats query.</summary>
    public class SessionStatsDto
    {
        public string SessionState { get; set; } = string.Empty;
        public int TotalCount { get; set; }
    }
}
