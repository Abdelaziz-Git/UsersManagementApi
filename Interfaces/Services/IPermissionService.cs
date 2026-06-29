using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.Permissions;

namespace TailorSoftAPI.Interfaces.Services
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