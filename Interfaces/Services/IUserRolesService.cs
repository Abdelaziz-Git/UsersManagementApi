using UsersManagementApi.DTOs.Common;
using UsersManagementApi.DTOs.UserRoles;

namespace UsersManagementApi.Interfaces.Services
{
    public interface IUserRolesService
    {
        Task<ResultDto<Guid>> AssignAsync(AssignUserRoleDto dto);
        Task<ResultDto<UserRoleResponseDto>> GetByIdAsync(Guid userRoleId);
        Task<ResultDto<List<UserRoleResponseDto>>> GetByUserIdAsync(Guid userId);
        Task<ResultDto<List<UserRoleResponseDto>>> GetByRoleIdAsync(Guid roleId);
        Task<ResultDto<List<UserRoleResponseDto>>> GetAllAsync();
        Task<ResultDto<bool>> DeleteAsync(Guid userRoleId);
        Task<ResultDto<bool>> DeleteByUserAndRoleAsync(DeleteUserRoleDto dto);
        Task<ResultDto<bool>> DeleteAllByUserIdAsync(Guid userId);
        Task<ResultDto<bool>> ExistsAsync(CheckUserRoleDto dto);
    }
}