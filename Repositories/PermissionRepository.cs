using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using UsersManagementApi.Data;
using UsersManagementApi.DTOs.Permissions;
using UsersManagementApi.Interfaces.Repositories;
using UsersManagementApi.Models;

namespace UsersManagementApi.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly DapperContext _context;

        public PermissionRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Guid?> CreateAsync(CreatePermissionDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@PermissionId", dbType: DbType.Guid, direction: ParameterDirection.Output);
            parameters.Add("@PermissionName", dto.PermissionName);
            parameters.Add("@Module", dto.Module);
            parameters.Add("@Description", dto.Description);

            await connection.ExecuteAsync("SP_Permissions_Create", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<Guid?>("@PermissionId");
        }

        public async Task<Permission?> GetByIdAsync(Guid id)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Permission?>(
                "SP_Permissions_GetById",
                new { PermissionId = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<List<Permission>> GetAllAsync(string module)
        {
            using var connection = _context.CreateConnection();

            var permissions = await connection.QueryAsync<Permission>(
                "SP_Permissions_GetAll",
                new { Module = module },
                commandType: CommandType.StoredProcedure);

            return permissions.ToList();
        }

        public async Task<bool> UpdateAsync(Guid id, UpdatePermissionDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsUpdated", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@PermissionId", id);
            parameters.Add("@PermissionName", dto.PermissionName);
            parameters.Add("@Module", dto.Module);
            parameters.Add("@Description", dto.Description);

            await connection.ExecuteAsync("SP_Permissions_Update", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsUpdated");
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsDeleted", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@PermissionId", id);

            await connection.ExecuteAsync(
                "SP_Permissions_Delete",
                parameters,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsDeleted");
        }
    }
}