using TailorSoftAPI.DTOs.Roles;
using TailorSoftAPI.Models;
namespace TailorSoftAPI.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<Guid?> CreateAsync(CreateRoleDto dto);
        Task<Role?> GetByIdAsync(Guid roleId);
        Task<Role?> GetByNameAsync(string roleName);
        Task<List<Role>> GetAllAsync();
        Task<bool> UpdateAsync(Guid roleId, UpdateRoleDto dto);
        Task<bool> DeleteAsync(Guid roleId);
    }
}
