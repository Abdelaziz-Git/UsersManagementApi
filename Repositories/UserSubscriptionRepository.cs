using Dapper;
using System.Data;
using UsersManagementApi.Data;
using UsersManagementApi.DTOs.UserSubscriptions;
using UsersManagementApi.Interfaces.Repositories;
using UsersManagementApi.Models;

namespace UsersManagementApi.Repositories
{
    public class UserSubscriptionRepository : IUserSubscriptionRepository
    {
        private readonly DapperContext _context;

        public UserSubscriptionRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Guid?> CreateAsync(CreateUserSubscriptionDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@SubscriptionId", dbType: DbType.Guid, direction: ParameterDirection.Output);
            parameters.Add("@UserId", dto.UserId);
            parameters.Add("@PlanId", dto.PlanId);
            parameters.Add("@Amount", dto.Amount);
            parameters.Add("@SubscriptionStatus", dto.SubscriptionStatus);
            parameters.Add("@BillingCycle", dto.BillingCycle);
            parameters.Add("@StartDate", dto.StartDate);
            parameters.Add("@EndDate", dto.EndDate);
            parameters.Add("@NextBillingDate", dto.NextBillingDate);
            parameters.Add("@TrialEndDate", dto.TrialEndDate);
            parameters.Add("@AutoRenew", dto.AutoRenew);

            await connection.ExecuteAsync("SP_UserSubscriptions_Create", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<Guid?>("@SubscriptionId");
        }

        public async Task<UserSubscription?> GetByIdAsync(Guid subscriptionId)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<UserSubscription>(
                "SP_UserSubscriptions_GetById",
                new { SubscriptionId = subscriptionId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<List<UserSubscription>> GetByUserIdAsync(Guid userId)
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryAsync<UserSubscription>(
                "SP_UserSubscriptions_GetByUserId",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<List<UserSubscription>> GetByPlanIdAsync(Guid planId, string? statusFilter)
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryAsync<UserSubscription>(
                "SP_UserSubscriptions_GetByPlanId",
                new { PlanId = planId, StatusFilter = statusFilter },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<List<UserSubscription>> GetAllAsync(string? statusFilter)
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryAsync<UserSubscription>(
                "SP_UserSubscriptions_GetAll",
                new { StatusFilter = statusFilter },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<bool> UpdateAsync(Guid subscriptionId, UpdateUserSubscriptionDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsUpdated", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@SubscriptionId", subscriptionId);
            parameters.Add("@PlanId", dto.PlanId);
            parameters.Add("@SubscriptionStatus", dto.SubscriptionStatus);
            parameters.Add("@BillingCycle", dto.BillingCycle);
            parameters.Add("@Amount", dto.Amount);
            parameters.Add("@EndDate", dto.EndDate);
            parameters.Add("@NextBillingDate", dto.NextBillingDate);
            parameters.Add("@TrialEndDate", dto.TrialEndDate);
            parameters.Add("@AutoRenew", dto.AutoRenew);

            await connection.ExecuteAsync("SP_UserSubscriptions_Update", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsUpdated");
        }

        public async Task<bool> ActivateAsync(Guid subscriptionId, ActivateUserSubscriptionDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsActivated", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@SubscriptionId", subscriptionId);
            parameters.Add("@NextBillingDate", dto.NextBillingDate);
            parameters.Add("@EndDate", dto.EndDate);

            await connection.ExecuteAsync("SP_UserSubscriptions_Activate", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsActivated");
        }

        public async Task<bool> CancelAsync(Guid subscriptionId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsCancelled", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@SubscriptionId", subscriptionId);

            await connection.ExecuteAsync("SP_UserSubscriptions_Cancel", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsCancelled");
        }

        public async Task<bool> ChangePlanAsync(Guid subscriptionId, ChangePlanDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsUpdated", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@SubscriptionId", subscriptionId);
            parameters.Add("@NewPlanId", dto.NewPlanId);
            parameters.Add("@NewAmount", dto.NewAmount);
            parameters.Add("@NewBillingCycle", dto.NewBillingCycle);
            parameters.Add("@NewEndDate", dto.NewEndDate);
            parameters.Add("@NextBillingDate", dto.NextBillingDate);

            await connection.ExecuteAsync("SP_UserSubscriptions_ChangePlan", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsUpdated");
        }

        public async Task<bool> RenewAsync(Guid subscriptionId, RenewUserSubscriptionDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsRenewed", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@SubscriptionId", subscriptionId);
            parameters.Add("@NextBillingDate", dto.NextBillingDate);
            parameters.Add("@NewEndDate", dto.NewEndDate);
            parameters.Add("@Amount", dto.Amount);

            await connection.ExecuteAsync("SP_UserSubscriptions_Renew", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsRenewed");
        }

        public async Task<bool> ToggleAutoRenewAsync(Guid subscriptionId, bool autoRenew)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsUpdated", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@SubscriptionId", subscriptionId);
            parameters.Add("@AutoRenew", autoRenew);

            await connection.ExecuteAsync("SP_UserSubscriptions_ToggleAutoRenew", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsUpdated");
        }

        public async Task<bool> MarkExpiredAsync(Guid subscriptionId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsUpdated", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@SubscriptionId", subscriptionId);

            await connection.ExecuteAsync("SP_UserSubscriptions_MarkExpired", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsUpdated");
        }

        public async Task<bool> MarkPastDueAsync(Guid subscriptionId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsUpdated", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@SubscriptionId", subscriptionId);

            await connection.ExecuteAsync("SP_UserSubscriptions_MarkPastDue", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsUpdated");
        }

        public async Task<bool> DeleteAsync(Guid subscriptionId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsDeleted", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@SubscriptionId", subscriptionId);

            await connection.ExecuteAsync("SP_UserSubscriptions_Delete", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsDeleted");
        }

        public async Task<bool> IsActiveAsync(Guid userId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsActive", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@UserId", userId);

            await connection.ExecuteAsync("SP_UserSubscriptions_IsActive", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsActive");
        }

        public async Task<List<DueBillingResponseDto>> GetDueBillingAsync(DateTime? asOfDate)
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryAsync<DueBillingResponseDto>(
                "SP_UserSubscriptions_GetDueBilling",
                new { AsOfDate = asOfDate },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<List<ExpiredSubscriptionResponseDto>> GetExpiredAsync(DateTime? asOfDate)
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryAsync<ExpiredSubscriptionResponseDto>(
                "SP_UserSubscriptions_GetExpired",
                new { AsOfDate = asOfDate },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<List<ExpiredTrialResponseDto>> GetExpiredTrialsAsync(DateTime? asOfDate)
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryAsync<ExpiredTrialResponseDto>(
                "SP_UserSubscriptions_GetExpiredTrials",
                new { AsOfDate = asOfDate },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<List<SubscriptionStatResponseDto>> GetStatsAsync()
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryAsync<SubscriptionStatResponseDto>(
                "SP_UserSubscriptions_GetStats",
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<List<UpcomingBillingResponseDto>> GetUpcomingBillingsAsync(int daysAhead)
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryAsync<UpcomingBillingResponseDto>(
                "SP_UserSubscriptions_GetUpcomingBillings",
                new { DaysAhead = daysAhead },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }
    }
}