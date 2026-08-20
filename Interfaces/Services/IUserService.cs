using UsersManagementApi.DTOs.Common;
using UsersManagementApi.DTOs.Users;

namespace UsersManagementApi.Interfaces.Services
{
    public interface IUserService
    {
        Task<ResultDto<Guid>> CreateAsync(CreateUserDto dto);
        Task<ResultDto<UserResponseDto>> GetByIdAsync(Guid id);
        Task<ResultDto<UserResponseDto>> GetByEmailAsync(string email);
        Task<ResultDto<List<UserResponseDto>>> GetAllAsync(PagedRequestDto dto);
        Task<ResultDto<bool>> UpdateAsync(Guid userId, UpdateUserDto dto);
        Task<ResultDto<bool>> UpdateLastLoginAsync(Guid userId);
        Task<ResultDto<bool>> DeleteAsync(Guid userId);
        Task<ResultDto<bool>> ExistsByEmailAsync(string email);
        Task<ResultDto<UserDetailsResponseDto>> GetUserDetailsByEmailAsync(string email);
    }
}
