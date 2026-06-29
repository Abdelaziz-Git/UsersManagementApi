using TailorSoftAPI.Models;
using TailorSoftAPI.DTOs.UserRoles;


namespace TailorSoftAPI.Interfaces.Repositories
{
    public interface IUserRolesRepository
    {
        Task<Guid?> AsignAsync(AssignUserRoleDto dto);
        Task<UserRole?> GetByIdAsync(Guid userRoleId);
        Task<List<UserRole>> GetByUserIdAsync(Guid userId);
        Task<List<UserRole>> GetByRoleIdAsync(Guid roleId);
        Task<List<UserRole>> GetAllAsync();
        Task<bool> DeleteAsync(Guid userRoleId);
        Task<bool> DeleteByUserAndRoleAsync(DeleteUserRoleDto dto);
        Task<bool> DeleteAllByUserId(Guid userId);
        Task<bool> ExistsAsync(CheckUserRoleDto dto);
    }
}