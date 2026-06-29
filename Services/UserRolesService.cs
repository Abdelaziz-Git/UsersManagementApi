using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.UserRoles;
using TailorSoftAPI.Interfaces.Repositories;
using TailorSoftAPI.Interfaces.Services;
using TailorSoftAPI.Models;

namespace TailorSoftAPI.Services
{
    public class UserRolesService : IUserRolesService
    {
        private readonly IUserRolesRepository _repository;

        public UserRolesService(IUserRolesRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository), "User roles repository cannot be null");
        }

        public async Task<ResultDto<Guid>> AssignAsync(AssignUserRoleDto dto)
        {
            if (dto is null)
                return ResultDto<Guid>.Failure("Assign user role DTO cannot be null");

            if (dto.UserId == Guid.Empty)
                return ResultDto<Guid>.Failure("User ID cannot be empty");

            if (dto.RoleId == Guid.Empty)
                return ResultDto<Guid>.Failure("Role ID cannot be empty");

            var guid = await _repository.AsignAsync(dto);
            if (guid is null || guid == Guid.Empty)
                return ResultDto<Guid>.Failure("Failed to assign role to user");

            return ResultDto<Guid>.Success(guid.Value);
        }

        public async Task<ResultDto<UserRoleResponseDto>> GetByIdAsync(Guid userRoleId)
        {
            if (userRoleId == Guid.Empty)
                return ResultDto<UserRoleResponseDto>.Failure("User role ID cannot be empty");

            var userRole = await _repository.GetByIdAsync(userRoleId);

            if (userRole is null)
                return ResultDto<UserRoleResponseDto>.Failure($"User role with ID: {userRoleId} not found");

            return ResultDto<UserRoleResponseDto>.Success(MapToUserRoleResponseDto(userRole));
        }

        public async Task<ResultDto<List<UserRoleResponseDto>>> GetByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return ResultDto<List<UserRoleResponseDto>>.Failure("User ID cannot be empty");

            var userRoles = await _repository.GetByUserIdAsync(userId);
            var userRoleDtos = userRoles.Select(MapToUserRoleResponseDto).ToList();

            if (userRoleDtos.Count == 0)
                return ResultDto<List<UserRoleResponseDto>>.Failure($"No roles found for user with ID: {userId}");

            return ResultDto<List<UserRoleResponseDto>>.Success(userRoleDtos);
        }

        public async Task<ResultDto<List<UserRoleResponseDto>>> GetByRoleIdAsync(Guid roleId)
        {
            if (roleId == Guid.Empty)
                return ResultDto<List<UserRoleResponseDto>>.Failure("Role ID cannot be empty");

            var userRoles = await _repository.GetByRoleIdAsync(roleId);
            var userRoleDtos = userRoles.Select(MapToUserRoleResponseDto).ToList();

            if (userRoleDtos.Count == 0)
                return ResultDto<List<UserRoleResponseDto>>.Failure($"No users found for role with ID: {roleId}");

            return ResultDto<List<UserRoleResponseDto>>.Success(userRoleDtos);
        }

        public async Task<ResultDto<List<UserRoleResponseDto>>> GetAllAsync()
        {
            var userRoles = await _repository.GetAllAsync();
            var userRoleDtos = userRoles.Select(MapToUserRoleResponseDto).ToList();

            if (userRoleDtos.Count == 0)
                return ResultDto<List<UserRoleResponseDto>>.Failure("No user roles found");

            return ResultDto<List<UserRoleResponseDto>>.Success(userRoleDtos);
        }

        public async Task<ResultDto<bool>> DeleteAsync(Guid userRoleId)
        {
            if (userRoleId == Guid.Empty)
                return ResultDto<bool>.Failure("User role ID cannot be empty");

            var result = await _repository.DeleteAsync(userRoleId);
            if (result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to delete user role");
        }

        public async Task<ResultDto<bool>> DeleteByUserAndRoleAsync(DeleteUserRoleDto dto)
        {
            if (dto is null)
                return ResultDto<bool>.Failure("Delete user role DTO cannot be null");

            if (dto.UserId == Guid.Empty)
                return ResultDto<bool>.Failure("User ID cannot be empty");

            if (dto.RoleId == Guid.Empty)
                return ResultDto<bool>.Failure("Role ID cannot be empty");

            var result = await _repository.DeleteByUserAndRoleAsync(dto);
            if (result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to delete user role");
        }

        public async Task<ResultDto<bool>> DeleteAllByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return ResultDto<bool>.Failure("User ID cannot be empty");

            var result = await _repository.DeleteAllByUserId(userId);
            if (result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to delete user roles");
        }

        public async Task<ResultDto<bool>> ExistsAsync(CheckUserRoleDto dto)
        {
            if (dto is null)
                return ResultDto<bool>.Failure("Check user role DTO cannot be null");

            if (dto.UserId == Guid.Empty)
                return ResultDto<bool>.Failure("User ID cannot be empty");

            if (dto.RoleId == Guid.Empty)
                return ResultDto<bool>.Failure("Role ID cannot be empty");

            var result = await _repository.ExistsAsync(dto);
            return ResultDto<bool>.Success(result);
        }

        #region Mapping Methods

        private UserRoleResponseDto MapToUserRoleResponseDto(UserRole userRole)
        {
            return new UserRoleResponseDto
            {
                UserRoleId = userRole.UserRoleId,
                UserId = userRole.UserId,
                RoleId = userRole.RoleId,
                FullName = userRole.FullName,
                RoleName = userRole.RoleName,
                Email = userRole.Email,
                Description = userRole.Description,
                AssignedDate = userRole.AssignedDate
            };
        }

        #endregion
    }
}