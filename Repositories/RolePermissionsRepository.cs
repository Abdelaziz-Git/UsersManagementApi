using Dapper;
using System.Data;
using TailorSoftAPI.Data;
using TailorSoftAPI.DTOs.RolePermissions;
using TailorSoftAPI.Interfaces.Repositories;
using TailorSoftAPI.Models;

namespace TailorSoftAPI.Repositories
{
    public class RolePermissionsRepository : IRolePermissionsRepository
    {
        private readonly DapperContext _context;

        public RolePermissionsRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Guid?> GrantAsync(GrantPermissionDto dto)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@RolePermissionId", dbType: DbType.Guid, direction: ParameterDirection.Output);
            parameters.Add("@RoleId", dto.RoleId);
            parameters.Add("@PermissionId", dto.PermissionId);

            await connection.ExecuteAsync("SP_RolePermissions_Grant", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<Guid?>("@RolePermissionId");
        }

        public async Task<List<RolePermission>> GetByRoleIdAsync(Guid roleId)
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryAsync<RolePermission>(
                "SP_RolePermissions_GetByRoleId",
                new { RoleId = roleId },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<List<RolePermission>> GetByPermissionIdAsync(Guid permissionId)
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryAsync<RolePermission>(
                "SP_RolePermissions_GetByPermissionId",
                new { PermissionId = permissionId },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<bool> RevokeAsync(Guid roleId, Guid permissionId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsRevoked", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@RoleId", roleId);
            parameters.Add("@PermissionId", permissionId);

            await connection.ExecuteAsync("SP_RolePermissions_Revoke", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsRevoked");
        }

        public async Task<bool> RevokeAllAsync(Guid roleId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add(name: "@IsRevoked", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@RoleId", roleId);

            await connection.ExecuteAsync("SP_RolePermissions_RevokeAll", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsRevoked");
        }
    }
}