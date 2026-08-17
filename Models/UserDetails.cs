namespace TailorSoftAPI.Models
{
    public class UserDetails
    {
        // User Profile Properties
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        // Credentials Properties
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsAccountLocked { get; set; }

        
        // User Subscription Properties

        public bool HasActiveSubscription { get; set; }

        // Roles (from second result set)
        public List<string> Roles { get; set; } = new();
    }
}
