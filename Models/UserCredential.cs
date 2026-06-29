namespace TailorSoftAPI.Models
{
    public class UserCredential
    {
        public Guid CredentialId { get; set; }

        public Guid UserId { get; set; }

        public string PasswordHash { get; set; } = string.Empty;

        public DateTime LastPasswordChangeDate { get; set; }

        public int FailedLoginAttempts { get; set; }

        public DateTime? AccountLockedUntil { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
