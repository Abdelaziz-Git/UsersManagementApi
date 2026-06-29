namespace TailorSoftAPI.DTOs.SubscriptionPlans
{
    public class SubscriptionPlanResponseDto
    {
        public Guid PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal MonthlyPrice { get; set; }

        public decimal? AnnualPrice { get; set; }

        public long? MaxStorage { get; set; }

        public string? Features { get; set; }

        public int DisplayOrder { get; set; }
    }
}
