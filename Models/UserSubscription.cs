using System;
using System.Collections.Generic;

namespace UsersManagementApi.Models;

public partial class UserSubscription
{
    public Guid SubscriptionId { get; set; }

    public Guid UserId { get; set; }

    public Guid PlanId { get; set; }

    /// <summary>
    /// Trial, Active, Expired, Cancelled, PastDue
    /// </summary>
    public string SubscriptionStatus { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string BillingCycle { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime? NextBillingDate { get; set; }

    public DateTime? TrialEndDate { get; set; }

    public bool AutoRenew { get; set; }

    public DateTime? CancelledDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public virtual SubscriptionPlan Plan { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
