namespace UsersManagementApi.Models
{
    public class UserDetails
    {
        // User Information
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }

        // Credentials Information
        public string PasswordHash { get; set; } = null!;
        public DateTime? AccountLockedUntil { get; set; }
        public bool IsAccountLocked { get; set; }

        // Subscription Plan Information
        public string PlanName { get; set; } = null!;
        public bool IsActivePlan { get; set; }

        // Subscription Information
        public string SubscriptionStatus { get; set; } = null!;
        public DateTime SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public DateTime? TrialEndDate { get; set; }
        public bool HasActiveSubscription { get; set; }

        // User Roles
        public ICollection<string> Roles { get; set; } = new List<string>();
    }
}
