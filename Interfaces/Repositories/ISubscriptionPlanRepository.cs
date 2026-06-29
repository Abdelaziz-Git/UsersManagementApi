using TailorSoftAPI.DTOs.SubscriptionPlans;
using TailorSoftAPI.Models;

namespace TailorSoftAPI.Interfaces.Repositories
{
    public interface ISubscriptionPlanRepository
    {
        Task<Guid?> CreateAsync(CreateSubscriptionPlanDto dto);
        Task<SubscriptionPlan?> GetByIdAsync(Guid planId);
        Task<SubscriptionPlan?> GetByNameAsync(string planName);
        Task<List<SubscriptionPlan>> GetAllAsync(bool activeOnly = true);
        Task<bool> UpdateAsync(Guid planId, UpdateSubscriptionPlanDto dto);
        Task<bool> DeleteAsync(Guid planId);
    }
}