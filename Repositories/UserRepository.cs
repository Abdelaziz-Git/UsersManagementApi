using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TailorSoftAPI.Data;
using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.Users;
using TailorSoftAPI.Interfaces.Repositories;
using TailorSoftAPI.Models;

namespace TailorSoftAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Guid?> CreateAsync(CreateUserDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@UserId", dbType: DbType.Guid, direction: ParameterDirection.Output);
            parameters.Add("@FirstName", dto.FirstName ?? (object)DBNull.Value);
            parameters.Add("@LastName", dto.LastName ?? (object)DBNull.Value);
            parameters.Add("@Email", dto.Email);
            parameters.Add("@PhoneNumber", dto.PhoneNumber);

            await connection.ExecuteAsync("SP_Users_Create", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<Guid>("@UserId");
        }

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<User>(
                "SP_Users_GetById",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<User>(
                "SP_Users_GetByEmail",
                new { Email = email },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<List<User>> GetAllAsync(PagedRequestDto dto)
        {
            using var connection = _context.CreateConnection();

            var users = await connection.QueryAsync<User>(
                "SP_Users_GetAll",
                new
                {
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize,
                    SearchTerm = dto.SearchTerm ?? (object)DBNull.Value
                },
                commandType: CommandType.StoredProcedure);

            return users.ToList();
        }

        public async Task<bool> UpdateAsync(Guid userId, UpdateUserDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsUpdated", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@UserId", userId);
            parameters.Add("@FirstName", dto.FirstName);
            parameters.Add("@LastName", dto.LastName);
            parameters.Add("@Email", dto.Email);
            parameters.Add("@PhoneNumber", dto.PhoneNumber);

            await connection.ExecuteAsync("SP_Users_Update", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsUpdated");
        }

        public async Task<bool> UpdateLastLogin(Guid userId)
        {
            using var connection = _context.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                "SP_Users_UpdateLastLogin",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid userId)
        {
            using var connection = _context.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                "SP_Users_Delete",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsExists", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@Email", email);
            connection.Execute("SP_Users_ExistsByEmail", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<bool>("@IsExists");
        }
    }
}