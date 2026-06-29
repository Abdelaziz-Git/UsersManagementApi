using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.SubscriptionPlans;
using TailorSoftAPI.Interfaces.Repositories;
using TailorSoftAPI.Interfaces.Services;
using TailorSoftAPI.Models;

namespace TailorSoftAPI.Services
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly ISubscriptionPlanRepository _repository;

        public SubscriptionPlanService(ISubscriptionPlanRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository), "Subscription plan repository cannot be null");
        }

        public async Task<ResultDto<Guid>> CreateAsync(CreateSubscriptionPlanDto dto)
        {
            if (dto is null)
                return ResultDto<Guid>.Failure("Create subscription plan DTO cannot be null");

            if (string.IsNullOrWhiteSpace(dto.PlanName))
                return ResultDto<Guid>.Failure("Plan name is required");

            if (dto.MonthlyPrice < 0)
                return ResultDto<Guid>.Failure("Monthly price cannot be negative");

            var guid = await _repository.CreateAsync(dto);
            if (guid is null || guid == Guid.Empty)
                return ResultDto<Guid>.Failure("Failed to create subscription plan");

            return ResultDto<Guid>.Success(guid.Value);
        }

        public async Task<ResultDto<SubscriptionPlanResponseDto>> GetByIdAsync(Guid planId)
        {
            if (planId == Guid.Empty)
                return ResultDto<SubscriptionPlanResponseDto>.Failure("Plan ID cannot be empty");

            var plan = await _repository.GetByIdAsync(planId);

            if (plan is null)
                return ResultDto<SubscriptionPlanResponseDto>.Failure("Subscription plan with ID:" + planId + " not found");

            return ResultDto<SubscriptionPlanResponseDto>.Success(MapToResponseDto(plan));
        }

        public async Task<ResultDto<SubscriptionPlanResponseDto>> GetByNameAsync(string planName)
        {
            if (string.IsNullOrWhiteSpace(planName))
                return ResultDto<SubscriptionPlanResponseDto>.Failure("Plan name cannot be empty");

            var plan = await _repository.GetByNameAsync(planName);

            if (plan is null)
                return ResultDto<SubscriptionPlanResponseDto>.Failure("Subscription plan with name:" + planName + " not found");

            return ResultDto<SubscriptionPlanResponseDto>.Success(MapToResponseDto(plan));
        }

        public async Task<ResultDto<List<SubscriptionPlanResponseDto>>> GetAllAsync(bool activeOnly = true)
        {
            var plans = await _repository.GetAllAsync(activeOnly);
            var planDtos = plans.Select(MapToResponseDto).ToList();

            if (planDtos.Count == 0)
                return ResultDto<List<SubscriptionPlanResponseDto>>.Failure("No subscription plans found");

            return ResultDto<List<SubscriptionPlanResponseDto>>.Success(planDtos);
        }

        public async Task<ResultDto<bool>> UpdateAsync(Guid planId, UpdateSubscriptionPlanDto dto)
        {
            if (planId == Guid.Empty)
                return ResultDto<bool>.Failure("Plan ID cannot be empty");

            if (dto is null)
                return ResultDto<bool>.Failure("Update subscription plan DTO cannot be null");

            if (string.IsNullOrWhiteSpace(dto.PlanName))
                return ResultDto<bool>.Failure("Plan name is required");

            if (dto.MonthlyPrice < 0)
                return ResultDto<bool>.Failure("Monthly price cannot be negative");

            var result = await _repository.UpdateAsync(planId, dto);
            if (result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to update subscription plan");
        }

        public async Task<ResultDto<bool>> DeleteAsync(Guid planId)
        {
            if (planId == Guid.Empty)
                return ResultDto<bool>.Failure("Plan ID cannot be empty");

            var result = await _repository.DeleteAsync(planId);
            if (result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to delete subscription plan");
        }

        #region Mapping Methods

        private SubscriptionPlanResponseDto MapToResponseDto(SubscriptionPlan plan)
        {
            return new SubscriptionPlanResponseDto
            {
                PlanId = plan.PlanId,
                PlanName = plan.PlanName,
                Description= plan.Description,
                MonthlyPrice = plan.MonthlyPrice,
                AnnualPrice = plan.AnnualPrice,
                MaxStorage= plan.MaxStorage,
                Features = plan.Features,
                DisplayOrder= plan.DisplayOrder,
            };
        }

        #endregion
    }
}