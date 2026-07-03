namespace TailorSoftAPI.DTOs.UserSubscriptions
{
    /// <summary>
    /// DTO for creating a new user subscription
    /// </summary>
    public class CreateUserSubscriptionDto
    {
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public decimal Amount { get; set; }
        public string SubscriptionStatus { get; set; } = "Trial";
        public string BillingCycle { get; set; } = "Monthly";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? NextBillingDate { get; set; }
        public DateTime? TrialEndDate { get; set; }
        public bool AutoRenew { get; set; } = true;
    }

    /// <summary>
    /// DTO for general-purpose partial update of a subscription
    /// </summary>
    public class UpdateUserSubscriptionDto
    {
        public Guid? PlanId { get; set; }
        public string? SubscriptionStatus { get; set; }
        public string? BillingCycle { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? NextBillingDate { get; set; }
        public DateTime? TrialEndDate { get; set; }
        public bool? AutoRenew { get; set; }
    }

    /// <summary>
    /// DTO for activating a subscription (Trial/Expired -> Active)
    /// </summary>
    public class ActivateUserSubscriptionDto
    {
        public DateTime NextBillingDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// DTO for changing a subscription's plan (upgrade / downgrade)
    /// </summary>
    public class ChangePlanDto
    {
        public Guid NewPlanId { get; set; }
        public decimal NewAmount { get; set; }
        public string? NewBillingCycle { get; set; }
        public DateTime? NewEndDate { get; set; }
        public DateTime? NextBillingDate { get; set; }
    }

    /// <summary>
    /// DTO for renewing a subscription after a successful payment
    /// </summary>
    public class RenewUserSubscriptionDto
    {
        public DateTime NextBillingDate { get; set; }
        public DateTime NewEndDate { get; set; }
        public decimal? Amount { get; set; }
    }

    /// <summary>
    /// DTO for toggling the AutoRenew flag
    /// </summary>
    public class ToggleAutoRenewDto
    {
        public bool AutoRenew { get; set; }
    }

    /// <summary>
    /// Standard response DTO returned to API consumers
    /// </summary>
    public class UserSubscriptionResponseDto
    {
        public Guid SubscriptionId { get; set; }
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public string SubscriptionStatus { get; set; } = string.Empty;
        public string BillingCycle { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? NextBillingDate { get; set; }
        public DateTime? TrialEndDate { get; set; }
        public bool AutoRenew { get; set; }
        public DateTime? CancelledDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    /// <summary>
    /// Lightweight response DTO used for billing-job queries (GetDueBilling)
    /// </summary>
    public class DueBillingResponseDto
    {
        public Guid SubscriptionId { get; set; }
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public string BillingCycle { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime? NextBillingDate { get; set; }
    }

    /// <summary>
    /// Lightweight response DTO used for expiry-job queries (GetExpired)
    /// </summary>
    public class ExpiredSubscriptionResponseDto
    {
        public Guid SubscriptionId { get; set; }
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public string SubscriptionStatus { get; set; } = string.Empty;
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// Lightweight response DTO for expired-trial queries (GetExpiredTrials)
    /// </summary>
    public class ExpiredTrialResponseDto
    {
        public Guid SubscriptionId { get; set; }
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public DateTime? TrialEndDate { get; set; }
        public decimal Amount { get; set; }
        public bool AutoRenew { get; set; }
    }

    /// <summary>
    /// Aggregate row returned by GetStats; one row per status
    /// </summary>
    public class SubscriptionStatResponseDto
    {
        public string SubscriptionStatus { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    /// <summary>
    /// Rich response DTO for upcoming-billing queries (includes user + plan info)
    /// </summary>
    public class UpcomingBillingResponseDto
    {
        public Guid SubscriptionId { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Guid PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime? NextBillingDate { get; set; }
        public bool AutoRenew { get; set; }
    }
}