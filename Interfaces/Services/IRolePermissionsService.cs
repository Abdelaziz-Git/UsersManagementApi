using UsersManagementApi.DTOs.Common;
using UsersManagementApi.DTOs.RolePermissions;

namespace UsersManagementApi.Interfaces.Services
{
    public interface IRolePermissionsService
    {
        Task<ResultDto<Guid>> GrantAsync(GrantPermissionDto dto);
        Task<ResultDto<List<RolePermissionResponseDto>>> GetByRoleIdAsync(Guid roleId);
        Task<ResultDto<List<RolePermissionResponseDto>>> GetByPermissionIdAsync(Guid permissionId);
        Task<ResultDto<bool>> RevokeAsync(Guid roleId, Guid permissionId);
        Task<ResultDto<bool>> RevokeAllAsync(Guid roleId);
    }
}