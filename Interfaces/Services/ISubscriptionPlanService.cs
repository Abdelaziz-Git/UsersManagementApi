using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.SubscriptionPlans;

namespace TailorSoftAPI.Interfaces.Services
{
    public interface ISubscriptionPlanService
    {
        Task<ResultDto<Guid>> CreateAsync(CreateSubscriptionPlanDto dto);
        Task<ResultDto<SubscriptionPlanResponseDto>> GetByIdAsync(Guid planId);
        Task<ResultDto<SubscriptionPlanResponseDto>> GetByNameAsync(string planName);
        Task<ResultDto<List<SubscriptionPlanResponseDto>>> GetAllAsync(bool activeOnly = true);
        Task<ResultDto<bool>> UpdateAsync(Guid planId, UpdateSubscriptionPlanDto dto);
        Task<ResultDto<bool>> DeleteAsync(Guid planId);
    }
}