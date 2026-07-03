using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.UserSubscriptions;

namespace TailorSoftAPI.Interfaces.Services
{
    public interface IUserSubscriptionService
    {
        Task<ResultDto<Guid>> CreateAsync(CreateUserSubscriptionDto dto);
        Task<ResultDto<UserSubscriptionResponseDto>> GetByIdAsync(Guid subscriptionId);
        Task<ResultDto<List<UserSubscriptionResponseDto>>> GetByUserIdAsync(Guid userId);
        Task<ResultDto<List<UserSubscriptionResponseDto>>> GetByPlanIdAsync(Guid planId, string? statusFilter);
        Task<ResultDto<List<UserSubscriptionResponseDto>>> GetAllAsync(string? statusFilter);
        Task<ResultDto<bool>> UpdateAsync(Guid subscriptionId, UpdateUserSubscriptionDto dto);
        Task<ResultDto<bool>> ActivateAsync(Guid subscriptionId, ActivateUserSubscriptionDto dto);
        Task<ResultDto<bool>> CancelAsync(Guid subscriptionId);
        Task<ResultDto<bool>> ChangePlanAsync(Guid subscriptionId, ChangePlanDto dto);
        Task<ResultDto<bool>> RenewAsync(Guid subscriptionId, RenewUserSubscriptionDto dto);
        Task<ResultDto<bool>> ToggleAutoRenewAsync(Guid subscriptionId, ToggleAutoRenewDto dto);
        Task<ResultDto<bool>> MarkExpiredAsync(Guid subscriptionId);
        Task<ResultDto<bool>> MarkPastDueAsync(Guid subscriptionId);
        Task<ResultDto<bool>> DeleteAsync(Guid subscriptionId);
        Task<ResultDto<bool>> IsActiveAsync(Guid userId);
        Task<ResultDto<List<DueBillingResponseDto>>> GetDueBillingAsync(DateTime? asOfDate);
        Task<ResultDto<List<ExpiredSubscriptionResponseDto>>> GetExpiredAsync(DateTime? asOfDate);
        Task<ResultDto<List<ExpiredTrialResponseDto>>> GetExpiredTrialsAsync(DateTime? asOfDate);
        Task<ResultDto<List<SubscriptionStatResponseDto>>> GetStatsAsync();
        Task<ResultDto<List<UpcomingBillingResponseDto>>> GetUpcomingBillingsAsync(int daysAhead);
    }
}