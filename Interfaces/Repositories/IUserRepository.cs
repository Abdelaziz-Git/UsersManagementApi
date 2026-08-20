using UsersManagementApi.DTOs.Users;
using UsersManagementApi.DTOs.Common;
using UsersManagementApi.Models;

namespace UsersManagementApi.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<Guid?> CreateAsync(CreateUserDto dto);
        Task<User?> GetByIdAsync(Guid userId);
        Task<User?> GetByEmailAsync(string email);
        Task<List<User>> GetAllAsync(PagedRequestDto dto);
        Task<bool> UpdateAsync(Guid userId, UpdateUserDto dto);
        Task<bool> UpdateLastLogin(Guid userId);
        Task<bool> DeleteAsync(Guid userId);
        Task<bool> ExistsByEmailAsync(string email);
        Task<UserDetails?> GetUserDetailsByEmailAsync(string email);
    }
}
