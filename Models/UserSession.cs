namespace TailorSoftAPI.Models
{
    public class UserSession
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public string RefreshTokenHash { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? RevokedDate { get; set; }
    }
}
