namespace TailorSoftAPI.DTOs.UserSubscriptions
{
    public class UserSubscriptionResponseDto
    {
        public Guid SubscriptionId { get; set; }
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public string SubscriptionStatus { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
