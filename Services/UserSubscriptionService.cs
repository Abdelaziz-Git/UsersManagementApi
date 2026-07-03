using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.UserSubscriptions;
using TailorSoftAPI.Interfaces.Repositories;
using TailorSoftAPI.Interfaces.Services;
using TailorSoftAPI.Models;

namespace TailorSoftAPI.Services
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IUserSubscriptionRepository _repository;

        public UserSubscriptionService(IUserSubscriptionRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository), "User subscription repository cannot be null");
        }

        public async Task<ResultDto<Guid>> CreateAsync(CreateUserSubscriptionDto dto)
        {
            if (dto is null)
                return ResultDto<Guid>.Failure("Create user subscription DTO cannot be null");

            if (dto.UserId == Guid.Empty)
                return ResultDto<Guid>.Failure("User ID is required");

            if (dto.PlanId == Guid.Empty)
                return ResultDto<Guid>.Failure("Plan ID is required");

            if (dto.Amount < 0)
                return ResultDto<Guid>.Failure("Amount cannot be negative");

            var guid = await _repository.CreateAsync(dto);
            if (guid is null || guid == Guid.Empty)
                return ResultDto<Guid>.Failure("Failed to create user subscription");

            return ResultDto<Guid>.Success(guid.Value);
        }

        public async Task<ResultDto<UserSubscriptionResponseDto>> GetByIdAsync(Guid subscriptionId)
        {
            if (subscriptionId == Guid.Empty)
                return ResultDto<UserSubscriptionResponseDto>.Failure("Subscription ID cannot be empty");

            var subscription = await _repository.GetByIdAsync(subscriptionId);
            if (subscription is null)
                return ResultDto<UserSubscriptionResponseDto>.Failure("Subscription with ID:" + subscriptionId + " not found");

            return ResultDto<UserSubscriptionResponseDto>.Success(MapToResponseDto(subscription));
        }

        public async Task<ResultDto<List<UserSubscriptionResponseDto>>> GetByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return ResultDto<List<UserSubscriptionResponseDto>>.Failure("User ID cannot be empty");

            var subscriptions = await _repository.GetByUserIdAsync(userId);
            if (subscriptions.Count == 0)
                return ResultDto<List<UserSubscriptionResponseDto>>.Failure("No subscriptions found for user with ID:" + userId);

            return ResultDto<List<UserSubscriptionResponseDto>>.Success(subscriptions.Select(MapToResponseDto).ToList());
        }

        public async Task<ResultDto<List<UserSubscriptionResponseDto>>> GetByPlanIdAsync(Guid planId, string? statusFilter)
        {
            if (planId == Guid.Empty)
                return ResultDto<List<UserSubscriptionResponseDto>>.Failure("Plan ID cannot be empty");

            var subscriptions = await _repository.GetByPlanIdAsync(planId, statusFilter);
            if (subscriptions.Count == 0)
                return ResultDto<List<UserSubscriptionResponseDto>>.Failure("No subscriptions found for plan with ID:" + planId);

            return ResultDto<List<UserSubscriptionResponseDto>>.Success(subscriptions.Select(MapToResponseDto).ToList());
        }

        public async Task<ResultDto<List<UserSubscriptionResponseDto>>> GetAllAsync(string? statusFilter)
        {
            var subscriptions = await _repository.GetAllAsync(statusFilter);
            if (subscriptions.Count == 0)
                return ResultDto<List<UserSubscriptionResponseDto>>.Failure("No subscriptions found");

            return ResultDto<List<UserSubscriptionResponseDto>>.Success(subscriptions.Select(MapToResponseDto).ToList());
        }

        public async Task<ResultDto<bool>> UpdateAsync(Guid subscriptionId, UpdateUserSubscriptionDto dto)
        {
            if (subscriptionId == Guid.Empty)
                return ResultDto<bool>.Failure("Subscription ID cannot be empty");

            if (dto is null)
                return ResultDto<bool>.Failure("Update user subscription DTO cannot be null");

            if (dto.Amount is not null && dto.Amount < 0)
                return ResultDto<bool>.Failure("Amount cannot be negative");

            var result = await _repository.UpdateAsync(subscriptionId, dto);
            if (result)
                return ResultDto<bool>.Success(result);

            return ResultDto<bool>.Failure("Failed to update subscription");
        }

        public async Task<ResultDto<bool>> ActivateAsync(Guid subscriptionId, ActivateUserSubscriptionDto dto)
        {
            if (subscriptionId == Guid.Empty)
                return ResultDto<bool>.Failure("Subscription ID cannot be empty");

            if (dto is null)
                return ResultDto<bool>.Failure("Activate subscription DTO cannot be null");

            if (dto.NextBillingDate == default)
                return ResultDto<bool>.Failure("NextBillingDate is required to activate a subscription");

            var result = await _repository.ActivateAsync(subscriptionId, dto);
            if (result)
                return ResultDto<bool>.Success(result);

            return ResultDto<bool>.Failure("Failed to activate subscription. It may already be active or not eligible for activation");
        }

        public async Task<ResultDto<bool>> CancelAsync(Guid subscriptionId)
        {
            if (subscriptionId == Guid.Empty)
                return ResultDto<bool>.Failure("Subscription ID cannot be empty");

            var result = await _repository.CancelAsync(subscriptionId);
            if (result)
                return ResultDto<bool>.Success(result);

            return ResultDto<bool>.Failure("Failed to cancel subscription. It may already be cancelled");
        }

        public async Task<ResultDto<bool>> ChangePlanAsync(Guid subscriptionId, ChangePlanDto dto)
        {
            if (subscriptionId == Guid.Empty)
                return ResultDto<bool>.Failure("Subscription ID cannot be empty");

            if (dto is null)
                return ResultDto<bool>.Failure("Change plan DTO cannot be null");

            if (dto.NewPlanId == Guid.Empty)
                return ResultDto<bool>.Failure("New plan ID is required");

            if (dto.NewAmount < 0)
                return ResultDto<bool>.Failure("New amount cannot be negative");

            var result = await _repository.ChangePlanAsync(subscriptionId, dto);
            if (result)
                return ResultDto<bool>.Success(result);

            return ResultDto<bool>.Failure("Failed to change plan. The subscription may be cancelled");
        }

        public async Task<ResultDto<bool>> RenewAsync(Guid subscriptionId, RenewUserSubscriptionDto dto)
        {
            if (subscriptionId == Guid.Empty)
                return ResultDto<bool>.Failure("Subscription ID cannot be empty");

            if (dto is null)
                return ResultDto<bool>.Failure("Renew subscription DTO cannot be null");

            if (dto.NextBillingDate == default)
                return ResultDto<bool>.Failure("NextBillingDate is required to renew a subscription");

            if (dto.NewEndDate == default)
                return ResultDto<bool>.Failure("NewEndDate is required to renew a subscription");

            if (dto.Amount is not null && dto.Amount < 0)
                return ResultDto<bool>.Failure("Amount cannot be negative");

            var result = await _repository.RenewAsync(subscriptionId, dto);
            if (result)
                return ResultDto<bool>.Success(result);

            return ResultDto<bool>.Failure("Failed to renew subscription");
        }

        public async Task<ResultDto<bool>> ToggleAutoRenewAsync(Guid subscriptionId, ToggleAutoRenewDto dto)
        {
            if (subscriptionId == Guid.Empty)
                return ResultDto<bool>.Failure("Subscription ID cannot be empty");

            if (dto is null)
                return ResultDto<bool>.Failure("Toggle auto-renew DTO cannot be null");

            var result = await _repository.ToggleAutoRenewAsync(subscriptionId, dto.AutoRenew);
            if (result)
                return ResultDto<bool>.Success(result);

            return ResultDto<bool>.Failure("Failed to toggle auto-renew. The subscription may be cancelled");
        }

        public async Task<ResultDto<bool>> MarkExpiredAsync(Guid subscriptionId)
        {
            if (subscriptionId == Guid.Empty)
                return ResultDto<bool>.Failure("Subscription ID cannot be empty");

            var result = await _repository.MarkExpiredAsync(subscriptionId);
            if (result)
                return ResultDto<bool>.Success(result);

            return ResultDto<bool>.Failure("Failed to mark subscription as expired. It may already be expired or not in an eligible status");
        }

        public async Task<ResultDto<bool>> MarkPastDueAsync(Guid subscriptionId)
        {
            if (subscriptionId == Guid.Empty)
                return ResultDto<bool>.Failure("Subscription ID cannot be empty");

            var result = await _repository.MarkPastDueAsync(subscriptionId);
            if (result)
                return ResultDto<bool>.Success(result);

            return ResultDto<bool>.Failure("Failed to mark subscription as past due. It must be in Active status");
        }

        public async Task<ResultDto<bool>> DeleteAsync(Guid subscriptionId)
        {
            if (subscriptionId == Guid.Empty)
                return ResultDto<bool>.Failure("Subscription ID cannot be empty");

            var result = await _repository.DeleteAsync(subscriptionId);
            if (result)
                return ResultDto<bool>.Success(result);

            return ResultDto<bool>.Failure("Failed to delete subscription");
        }

        public async Task<ResultDto<bool>> IsActiveAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return ResultDto<bool>.Failure("User ID cannot be empty");

            var isActive = await _repository.IsActiveAsync(userId);
            return ResultDto<bool>.Success(isActive);
        }

        public async Task<ResultDto<List<DueBillingResponseDto>>> GetDueBillingAsync(DateTime? asOfDate)
        {
            var result = await _repository.GetDueBillingAsync(asOfDate);
            if (result.Count == 0)
                return ResultDto<List<DueBillingResponseDto>>.Failure("No subscriptions due for billing");

            return ResultDto<List<DueBillingResponseDto>>.Success(result);
        }

        public async Task<ResultDto<List<ExpiredSubscriptionResponseDto>>> GetExpiredAsync(DateTime? asOfDate)
        {
            var result = await _repository.GetExpiredAsync(asOfDate);
            if (result.Count == 0)
                return ResultDto<List<ExpiredSubscriptionResponseDto>>.Failure("No expired subscriptions found");

            return ResultDto<List<ExpiredSubscriptionResponseDto>>.Success(result);
        }

        public async Task<ResultDto<List<ExpiredTrialResponseDto>>> GetExpiredTrialsAsync(DateTime? asOfDate)
        {
            var result = await _repository.GetExpiredTrialsAsync(asOfDate);
            if (result.Count == 0)
                return ResultDto<List<ExpiredTrialResponseDto>>.Failure("No expired trials found");

            return ResultDto<List<ExpiredTrialResponseDto>>.Success(result);
        }

        public async Task<ResultDto<List<SubscriptionStatResponseDto>>> GetStatsAsync()
        {
            var result = await _repository.GetStatsAsync();
            if (result.Count == 0)
                return ResultDto<List<SubscriptionStatResponseDto>>.Failure("No subscription statistics available");

            return ResultDto<List<SubscriptionStatResponseDto>>.Success(result);
        }

        public async Task<ResultDto<List<UpcomingBillingResponseDto>>> GetUpcomingBillingsAsync(int daysAhead)
        {
            if (daysAhead < 1)
                return ResultDto<List<UpcomingBillingResponseDto>>.Failure("DaysAhead must be at least 1");

            var result = await _repository.GetUpcomingBillingsAsync(daysAhead);
            if (result.Count == 0)
                return ResultDto<List<UpcomingBillingResponseDto>>.Failure("No upcoming billings found in the next " + daysAhead + " days");

            return ResultDto<List<UpcomingBillingResponseDto>>.Success(result);
        }

        #region Mapping Methods

        private UserSubscriptionResponseDto MapToResponseDto(UserSubscription subscription)
        {
            return new UserSubscriptionResponseDto
            {
                SubscriptionId = subscription.SubscriptionId,
                UserId = subscription.UserId,
                PlanId = subscription.PlanId,
                SubscriptionStatus = subscription.SubscriptionStatus,
                BillingCycle = subscription.BillingCycle,
                Amount = subscription.Amount,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                NextBillingDate = subscription.NextBillingDate,
                TrialEndDate = subscription.TrialEndDate,
                AutoRenew = subscription.AutoRenew,
                CancelledDate = subscription.CancelledDate,
                CreatedDate = subscription.CreatedDate,
                UpdatedDate = subscription.UpdatedDate
            };
        }

        #endregion
    }
}