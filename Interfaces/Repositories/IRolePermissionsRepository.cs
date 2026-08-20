using UsersManagementApi.DTOs.RolePermissions;
using UsersManagementApi.Models;

namespace UsersManagementApi.Interfaces.Repositories
{
    public interface IRolePermissionsRepository
    {
        Task<Guid?> GrantAsync(GrantPermissionDto dto);
        Task<List<RolePermission>> GetByRoleIdAsync(Guid roleId);
        Task<List<RolePermission>> GetByPermissionIdAsync(Guid permissionId);
        Task<bool> RevokeAsync(Guid roleId, Guid permissionId);
        Task<bool> RevokeAllAsync(Guid roleId);
    }
}