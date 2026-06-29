using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.Roles;

namespace TailorSoftAPI.Interfaces.Services
{
    public interface IRoleService
    {
        Task<ResultDto<Guid>> CreateAsync(CreateRoleDto dto);
        Task<ResultDto<RoleResponseDto>> GetByIdAsync(Guid roleId);
        Task<ResultDto<RoleResponseDto>> GetByNameAsync(string roleName);
        Task<ResultDto<List<RoleResponseDto>>> GetAllAsync();
        Task<ResultDto<bool>> UpdateAsync(Guid roleId, UpdateRoleDto dto);
        Task<ResultDto<bool>> DeleteAsync(Guid roleId);
    }
}