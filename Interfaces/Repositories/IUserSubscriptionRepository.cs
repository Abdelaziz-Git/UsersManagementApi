using TailorSoftAPI.DTOs.UserSubscriptions;
using TailorSoftAPI.Models;

namespace TailorSoftAPI.Interfaces.Repositories
{
    public interface IUserSubscriptionRepository
    {
        Task<Guid?> CreateAsync(CreateUserSubscriptionDto dto);
        Task<UserSubscription?> GetByIdAsync(Guid subscriptionId);
        Task<List<UserSubscription>> GetByUserIdAsync(Guid userId);
        Task<List<UserSubscription>> GetByPlanIdAsync(Guid planId, string? statusFilter);
        Task<List<UserSubscription>> GetAllAsync(string? statusFilter);
        Task<bool> UpdateAsync(Guid subscriptionId, UpdateUserSubscriptionDto dto);
        Task<bool> ActivateAsync(Guid subscriptionId, ActivateUserSubscriptionDto dto);
        Task<bool> CancelAsync(Guid subscriptionId);
        Task<bool> ChangePlanAsync(Guid subscriptionId, ChangePlanDto dto);
        Task<bool> RenewAsync(Guid subscriptionId, RenewUserSubscriptionDto dto);
        Task<bool> ToggleAutoRenewAsync(Guid subscriptionId, bool autoRenew);
        Task<bool> MarkExpiredAsync(Guid subscriptionId);
        Task<bool> MarkPastDueAsync(Guid subscriptionId);
        Task<bool> DeleteAsync(Guid subscriptionId);
        Task<bool> IsActiveAsync(Guid userId);
        Task<List<DueBillingResponseDto>> GetDueBillingAsync(DateTime? asOfDate);
        Task<List<ExpiredSubscriptionResponseDto>> GetExpiredAsync(DateTime? asOfDate);
        Task<List<ExpiredTrialResponseDto>> GetExpiredTrialsAsync(DateTime? asOfDate);
        Task<List<SubscriptionStatResponseDto>> GetStatsAsync();
        Task<List<UpcomingBillingResponseDto>> GetUpcomingBillingsAsync(int daysAhead);
    }
}