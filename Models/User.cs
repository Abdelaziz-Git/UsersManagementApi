namespace TailorSoftAPI.Models
{
    public class User
    {
        public Guid UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public bool EmailVerified { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}
