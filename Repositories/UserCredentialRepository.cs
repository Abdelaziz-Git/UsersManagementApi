using Dapper;
using System.Data;
using UsersManagementApi.Data;
using UsersManagementApi.DTOs.Common;
using UsersManagementApi.DTOs.UserCredentials;
using UsersManagementApi.Interfaces.Repositories;
using UsersManagementApi.Models;

namespace UsersManagementApi.Repositories
{
    public class UserCredentialRepository : IUserCredentialRepository
    {
        private readonly DapperContext _context;

        public UserCredentialRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Guid?> CreateAsync(CreateUserCredentialDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@CredentialId", dbType: DbType.Guid, direction: ParameterDirection.Output);
            parameters.Add("@UserId", dto.UserId);
            parameters.Add("@PasswordHash", dto.PasswordHash);

            await connection.ExecuteAsync("SP_UserCredentials_Create", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<Guid>("@CredentialId");
        }

        public async Task<UserCredential?> GetByUserIdAsync(Guid userId)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<UserCredential>(
                "SP_UserCredentials_GetByUserId",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> IncrementFailedLoginAttemptsAsync(Guid userId, FailedLoginRequestDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsIncremented", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@UserId", userId);
            parameters.Add("@MaxAttempts", dto.MaxAttempts);
            parameters.Add("@LockoutDurationMinutes", dto.LockoutDurationMinutes);

            await connection.ExecuteAsync("SP_UserCredentials_IncrementFailedLoginAttempts", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<bool>("@IsIncremented");
        }

        public async Task<bool> IsAccountLocked(Guid userId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsLocked", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@UserId", userId);

            await connection.ExecuteAsync(
                "SP_UserCredentials_IsAccountLocked",
                parameters,
                commandType: CommandType.StoredProcedure);
            return parameters.Get<bool>("@IsLocked");
        }

        public async Task<bool> ResetFailedLoginAttempts(Guid userId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsReseted", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@UserId", userId);

            await connection.ExecuteAsync("SP_UserCredentials_ResetFailedLoginAttempts", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<bool>("@IsReseted");
        }

        public async Task<bool> UpdatePasswordAsync(Guid userId, UpdateUserCredentialDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsUpdated", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@UserId", userId);
            parameters.Add("@PasswordHash", dto.PasswordHash);

            await connection.ExecuteAsync("SP_UserCredentials_UpdatePassword", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<bool>("@IsUpdated");
        }
    }
}