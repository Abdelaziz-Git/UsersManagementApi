using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using TailorSoftAPI.Data;
using TailorSoftAPI.Interfaces.Repositories;
using TailorSoftAPI.Models;
using TailorSoftAPI.DTOs.UserRoles;

namespace TailorSoftAPI.Repositories
{
    public class UserRolesRepository : IUserRolesRepository
    {
        private readonly DapperContext _context;

        public UserRolesRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Guid?> AsignAsync(AssignUserRoleDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@UserRoleId", dbType: DbType.Guid, direction: ParameterDirection.Output);
            parameters.Add("@UserId", dto.UserId);
            parameters.Add("@RoleId", dto.RoleId);

            await connection.ExecuteAsync("SP_UserRoles_Assign", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<Guid?>("@UserRoleId");
        }

        public async Task<UserRole?> GetByIdAsync(Guid userRoleId)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<UserRole>(
                "SP_UserRoles_GetById",
                new { UserRoleId = userRoleId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<List<UserRole>> GetByUserIdAsync(Guid userId)
        {
            using var connection = _context.CreateConnection();

            var userRoles = await connection.QueryAsync<UserRole>(
                "SP_UserRoles_GetByUserId",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);

            return userRoles.ToList();
        }

        public async Task<List<UserRole>> GetByRoleIdAsync(Guid roleId)
        {
            using var connection = _context.CreateConnection();

            var userRoles = await connection.QueryAsync<UserRole>(
                "SP_UserRoles_GetByRoleId",
                new { RoleId = roleId },
                commandType: CommandType.StoredProcedure);

            return userRoles.ToList();
        }

        public async Task<List<UserRole>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();

            var userRoles = await connection.QueryAsync<UserRole>(
                "SP_UserRoles_GetAll",
                commandType: CommandType.StoredProcedure);

            return userRoles.ToList();
        }

        public async Task<bool> DeleteAsync(Guid userRoleId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsDeleted", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@UserRoleId", userRoleId);

            await connection.ExecuteAsync(
                "SP_UserRoles_Delete",
                parameters,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsDeleted");
        }

        public async Task<bool> DeleteByUserAndRoleAsync(DeleteUserRoleDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsDeleted", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@UserId", dto.UserId);
            parameters.Add("@RoleId", dto.RoleId);

            await connection.ExecuteAsync(
                "SP_UserRoles_DeleteByUserIdAndRoleId",
                parameters,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsDeleted");
        }

        public async Task<bool> DeleteAllByUserId(Guid userId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsDeleted", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@UserId", userId);

            await connection.ExecuteAsync(
                "SP_UserRoles_DeleteAllByUserId",
                parameters,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsDeleted");
        }

        public async Task<bool> ExistsAsync(CheckUserRoleDto dto)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", dto.UserId);
            parameters.Add("@RoleId", dto.RoleId);
            parameters.Add(name: "@IsExists", dbType: DbType.Boolean, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "SP_UserRoles_ExistsByUserIdAndRoleId",
                parameters,
                commandType: CommandType.StoredProcedure);
            return parameters.Get<bool>("@IsExists");
        }
    }
}