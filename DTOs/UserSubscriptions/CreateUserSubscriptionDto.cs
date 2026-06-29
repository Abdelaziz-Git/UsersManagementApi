namespace TailorSoftAPI.DTOs.UserSubscriptions
{
    public class CreateUserSubscriptionDto
    {
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public string BillingCycle { get; set; } = string.Empty;
    }
}
