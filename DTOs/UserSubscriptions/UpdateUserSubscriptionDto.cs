namespace TailorSoftAPI.DTOs.UserSubscriptions
{
    public class UpdateUserSubscriptionDto
    {
        public Guid SubscriptionId { get; set; }
        public string SubscriptionStatus { get; set; } = string.Empty;
        public bool AutoRenew { get; set; }
    }
}
