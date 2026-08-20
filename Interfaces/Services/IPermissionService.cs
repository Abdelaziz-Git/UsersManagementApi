using UsersManagementApi.DTOs.Common;
using UsersManagementApi.DTOs.Permissions;

namespace UsersManagementApi.Interfaces.Services
{
    public interface IPermissionService
    {
        Task<ResultDto<Guid>> CreateAsync(CreatePermissionDto dto);
        Task<ResultDto<PermissionResponseDto>> GetByIdAsync(Guid permissionId);
        Task<ResultDto<List<PermissionResponseDto>>> GetAllAsync(string module);
        Task<ResultDto<bool>> UpdateAsync(Guid permissionId, UpdatePermissionDto dto);
        Task<ResultDto<bool>> DeleteAsync(Guid permissionId);
    }
}