namespace TailorSoftAPI.Models
{
    public class PasswordResetToken
    {
        public Guid TokenId { get; set; }

        public Guid UserId { get; set; }

        public string ResetToken { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }

        public bool IsUsed { get; set; }

        public DateTime? UsedDate { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
