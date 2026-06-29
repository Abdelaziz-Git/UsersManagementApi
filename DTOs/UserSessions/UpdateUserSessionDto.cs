namespace TailorSoftAPI.DTOs.UserSessions
{
    public class UpdateUserSessionDto
    {
        public Guid SessionId { get; set; }
        public bool IsRevoked { get; set; }
    }
}
