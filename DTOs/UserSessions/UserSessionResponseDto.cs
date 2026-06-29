namespace TailorSoftAPI.DTOs.UserSessions
{
    public class UserSessionResponseDto
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
    }
}
