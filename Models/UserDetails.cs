
namespace TailorSoftAPI.Models
{
    public class UserDetails
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

        // Credential Information
        public Guid? CredentialId { get; set; }

        public string? PasswordHash { get; set; }

        public DateTime? LastPasswordChangeDate { get; set; }

        public int? FailedLoginAttempts { get; set; }

        public DateTime? AccountLockedUntil { get; set; }

        // Role Information
        public Guid? UserRoleId { get; set; }

        public Guid? RoleId { get; set; }

        public string? RoleName { get; set; }

        public string? RoleDescription { get; set; }

        public DateTime? RoleAssignedDate { get; set; }

        public DateTime? RoleCreatedDate { get; set; }

        // Subscription Information
        public Guid? SubscriptionId { get; set; }

        public Guid? PlanId { get; set; }

        public string? SubscriptionStatus { get; set; }

        public DateTime? SubscriptionStartDate { get; set; }

        public DateTime? SubscriptionEndDate { get; set; }

        public string? BillingCycle { get; set; }

        public decimal? SubscriptionAmount { get; set; }

        public DateTime? NextBillingDate { get; set; }

        public DateTime? TrialEndDate { get; set; }

        public bool? AutoRenew { get; set; }

        public DateTime? SubscriptionCancelledDate { get; set; }

        // Session Information
        public Guid? SessionId { get; set; }

        public string? SessionToken { get; set; }

        public string? UserAgent { get; set; }

        public string? IpAddress { get; set; }

        public DateTime? SessionStartDate { get; set; }

        public DateTime? SessionExpiryDate { get; set; }

        public bool? IsSessionActive { get; set; }
    }
}
