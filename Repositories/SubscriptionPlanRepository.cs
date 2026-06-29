using Dapper;
using System.Data;
using TailorSoftAPI.Data;
using TailorSoftAPI.DTOs.SubscriptionPlans;
using TailorSoftAPI.Interfaces.Repositories;
using TailorSoftAPI.Models;

namespace TailorSoftAPI.Repositories
{
    public class SubscriptionPlanRepository : ISubscriptionPlanRepository
    {
        private readonly DapperContext _context;

        public SubscriptionPlanRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Guid?> CreateAsync(CreateSubscriptionPlanDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@PlanId", dbType: DbType.Guid, direction: ParameterDirection.Output);
            parameters.Add("@PlanName", dto.PlanName);
            parameters.Add("@Description", dto.Description);
            parameters.Add("@MonthlyPrice", dto.MonthlyPrice);
            parameters.Add("@AnnualPrice", dto.AnnualPrice);
            parameters.Add("@MaxStorage", dto.MaxStorage);
            parameters.Add("@Features", dto.Features);
            parameters.Add("@DisplayOrder", dto.DisplayOrder);

            await connection.ExecuteAsync("SP_SubscriptionPlans_Create", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<Guid?>("@PlanId");
        }

        public async Task<SubscriptionPlan?> GetByIdAsync(Guid planId)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<SubscriptionPlan>(
                "SP_SubscriptionPlans_GetById",
                new { PlanId = planId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<SubscriptionPlan?> GetByNameAsync(string planName)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<SubscriptionPlan>(
                "SP_SubscriptionPlans_GetByName",
                new { PlanName = planName },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<List<SubscriptionPlan>> GetAllAsync(bool activeOnly = true)
        {
            using var connection = _context.CreateConnection();

            var plans = await connection.QueryAsync<SubscriptionPlan>(
                "SP_SubscriptionPlans_GetAll",
                new { ActiveOnly = activeOnly },
                commandType: CommandType.StoredProcedure);

            return plans.ToList();
        }

        public async Task<bool> UpdateAsync(Guid planId, UpdateSubscriptionPlanDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsUpdated", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@PlanId", planId);
            parameters.Add("@PlanName", dto.PlanName);
            parameters.Add("@Description", dto.Description);
            parameters.Add("@MonthlyPrice", dto.MonthlyPrice);
            parameters.Add("@AnnualPrice", dto.AnnualPrice);
            parameters.Add("@MaxStorage", dto.MaxStorage);
            parameters.Add("@Features", dto.Features);
            parameters.Add("@DisplayOrder", dto.DisplayOrder);
            parameters.Add("@IsActive", dto.IsActive);

            await connection.ExecuteAsync("SP_SubscriptionPlans_Update", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsUpdated");
        }

        public async Task<bool> DeleteAsync(Guid planId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsDeleted", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@PlanId", planId);

            await connection.ExecuteAsync("SP_SubscriptionPlans_Delete", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsDeleted");
        }
    }
}