namespace TailorSoftAPI.DTOs.UserSessions
{
    public class CreateUserSessionDto
    {
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}
