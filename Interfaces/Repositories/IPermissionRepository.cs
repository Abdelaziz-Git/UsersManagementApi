using UsersManagementApi.DTOs.Permissions;
using UsersManagementApi.Models;
namespace UsersManagementApi.Interfaces.Repositories
{
    public interface IPermissionRepository
    {
        public Task<Guid?> CreateAsync(CreatePermissionDto dto);
        public Task<Permission?> GetByIdAsync(Guid id);
        public Task<List<Permission>> GetAllAsync(string module);
        public Task<bool> UpdateAsync(Guid id, UpdatePermissionDto dto);
        public Task<bool> DeleteAsync(Guid id);

    }
}
