using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using UsersManagementApi.Data;
using UsersManagementApi.DTOs.Roles;
using UsersManagementApi.Interfaces.Repositories;
using UsersManagementApi.Models;

namespace UsersManagementApi.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DapperContext _context;

        public RoleRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Guid?> CreateAsync(CreateRoleDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@RoleId", dbType: DbType.Guid, direction: ParameterDirection.Output);
            parameters.Add("@RoleName", dto.RoleName);
            parameters.Add("@Description", dto.Description);

            await connection.ExecuteAsync("SP_Roles_Create", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<Guid>("@RoleId");
        }

        public async Task<Role?> GetByIdAsync(Guid roleId)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Role>(
                "SP_Roles_GetById",
                new { RoleId = roleId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Role?> GetByNameAsync(string roleName)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Role>(
                "SP_Roles_GetByName",
                new { RoleName = roleName },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<List<Role>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();

            var roles = await connection.QueryAsync<Role>(
                "SP_Roles_GetAll",
                commandType: CommandType.StoredProcedure);

            return roles.ToList();
        }

        public async Task<bool> UpdateAsync(Guid roleId, UpdateRoleDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsUpdated", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@RoleId", roleId);
            parameters.Add("@RoleName", dto.RoleName);
            parameters.Add("@Description", dto.Description);

            await connection.ExecuteAsync("SP_Roles_Update", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsUpdated");
        }

        public async Task<bool> DeleteAsync(Guid roleId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsDeleted", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@RoleId", roleId);

            await connection.ExecuteAsync(
                "SP_Roles_Delete",
                parameters,
                commandType: CommandType.StoredProcedure);
            return parameters.Get<bool>("@IsDeleted");
        }
    }
}