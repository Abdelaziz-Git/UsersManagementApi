
using Dapper;
using System.Data;
using UsersManagementApi.Data;
using UsersManagementApi.DTOs.UserSessions;
using UsersManagementApi.Interfaces.Repositories;
using UsersManagementApi.Models;

namespace UsersManagementApi.Repositories
{
    public class UserSessionsRepository : IUserSessionsRepository
    {
        private readonly DapperContext _context;

        public UserSessionsRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Guid?> CreateAsync(CreateUserSessionDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@SessionId", dbType: DbType.Guid, direction: ParameterDirection.Output);
            parameters.Add("@UserId", dto.UserId);
            parameters.Add("@RefreshTokenHash", dto.RefreshTokenHash);
            parameters.Add("@ExpiryDate", dto.ExpiryDate);

            await connection.ExecuteAsync("SP_UserSessions_Create", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<Guid?>("@SessionId");
        }

        public async Task<UserSession?> GetByIdAsync(Guid sessionId)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<UserSession>(
                "SP_UserSessions_GetById",
                new { SessionId = sessionId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<UserSession?> GetByRefreshTokenHashAsync(string refreshTokenHash)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<UserSession>(
                "SP_UserSessions_GetByRefreshTokenHash",
                new { RefreshTokenHash = refreshTokenHash },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<List<UserSession>> GetByUserIdAsync(Guid userId, bool activeOnly)
        {
            using var connection = _context.CreateConnection();

            var sessions = await connection.QueryAsync<UserSession>(
                "SP_UserSessions_GetByUserId",
                new { UserId = userId, ActiveOnly = activeOnly },
                commandType: CommandType.StoredProcedure);

            return sessions.ToList();
        }

        public async Task<List<UserSession>> GetAllAsync(Guid? userId, bool activeOnly)
        {
            using var connection = _context.CreateConnection();

            var sessions = await connection.QueryAsync<UserSession>(
                "SP_UserSessions_GetAll",
                new { UserId = userId, ActiveOnly = activeOnly },
                commandType: CommandType.StoredProcedure);

            return sessions.ToList();
        }

        public async Task<bool> DeleteAsync(Guid sessionId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsDeleted", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@SessionId", sessionId);

            await connection.ExecuteAsync("SP_UserSessions_Delete", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsDeleted");
        }

        public async Task<bool> RevokeByIdAsync(Guid sessionId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsRevoked", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@SessionId", sessionId);

            await connection.ExecuteAsync("SP_UserSessions_RevokeById", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsRevoked");
        }

        public async Task<bool> RevokeByRefreshTokenHashAsync(string refreshTokenHash)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsRevoked", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@RefreshTokenHash", refreshTokenHash);

            await connection.ExecuteAsync("SP_UserSessions_RevokeByRefreshTokenHash", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsRevoked");
        }

        public async Task<int> RevokeByUserIdAsync(Guid userId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@RevokedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@UserId", userId);

            await connection.ExecuteAsync("SP_UserSessions_RevokeByUserId", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@RevokedCount");
        }

        public async Task<int> RevokeAllExceptCurrentAsync(Guid userId, Guid currentSessionId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@RevokedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@UserId", userId);
            parameters.Add("@CurrentSessionId", currentSessionId);

            await connection.ExecuteAsync("SP_UserSessions_RevokeAllExceptCurrent", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@RevokedCount");
        }

        public async Task<RotateTokenResultDto> RotateTokenAsync(RotateTokenRequestDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@OldRefreshTokenHash", dto.OldRefreshTokenHash);
            parameters.Add("@NewRefreshTokenHash", dto.NewRefreshTokenHash);
            parameters.Add("@NewExpiryDate", dto.NewExpiryDate);
            parameters.Add(name: "@UserId", dbType: DbType.Guid, direction: ParameterDirection.Output);
            parameters.Add(name: "@IsRotated", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add(name: "@IsReuseDetected", dbType: DbType.Boolean, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_UserSessions_RotateToken", parameters, commandType: CommandType.StoredProcedure);

            return new RotateTokenResultDto
            {
                UserId = parameters.Get<Guid?>("@UserId"),
                IsRotated = parameters.Get<bool>("@IsRotated"),
                IsReuseDetected = parameters.Get<bool>("@IsReuseDetected")
            };
        }

        public async Task<bool> IsValidAsync(string refreshTokenHash)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsValid", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@RefreshTokenHash", refreshTokenHash);

            await connection.ExecuteAsync("SP_UserSessions_IsValid", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsValid");
        }

        public async Task<int> CleanupExpiredTokensAsync()
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@DeletedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_UserSessions_CleanupExpiredTokens", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@DeletedCount");
        }

        public async Task<int> GetActiveSessionCountAsync(Guid userId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@ActiveCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@UserId", userId);

            await connection.ExecuteAsync("SP_UserSessions_GetActiveSessionCount", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@ActiveCount");
        }

        public async Task<List<SessionStatsDto>> GetStatsAsync()
        {
            using var connection = _context.CreateConnection();

            var stats = await connection.QueryAsync<SessionStatsDto>(
                "SP_UserSessions_GetStats",
                commandType: CommandType.StoredProcedure);

            return stats.ToList();
        }
    }
}