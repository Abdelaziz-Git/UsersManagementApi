using UsersManagementApi.DTOs.Common;
using UsersManagementApi.DTOs.Roles;
using UsersManagementApi.Interfaces.Repositories;
using UsersManagementApi.Interfaces.Services;
using UsersManagementApi.Models;

namespace UsersManagementApi.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repository;

        public RoleService(IRoleRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository), "Role repository cannot be null");
        }

        public async Task<ResultDto<Guid>> CreateAsync(CreateRoleDto dto)
        {
            if (dto is null)
                return ResultDto<Guid>.Failure("Create role DTO cannot be null");

            if (string.IsNullOrWhiteSpace(dto.RoleName))
                return ResultDto<Guid>.Failure("Role name is required");

            var guid = await _repository.CreateAsync(dto);
            if (guid is null || guid == Guid.Empty)
                return ResultDto<Guid>.Failure("Failed to create role");

            return ResultDto<Guid>.Success(guid.Value);
        }

        public async Task<ResultDto<RoleResponseDto>> GetByIdAsync(Guid roleId)
        {
            if (roleId == Guid.Empty)
                return ResultDto<RoleResponseDto>.Failure("Role ID cannot be empty");

            var role = await _repository.GetByIdAsync(roleId);

            if (role == null)
                return ResultDto<RoleResponseDto>.Failure("Role with ID:" + roleId + " not found");

            return ResultDto<RoleResponseDto>.Success(MapToRoleResponseDto(role));
        }

        public async Task<ResultDto<RoleResponseDto>> GetByNameAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return ResultDto<RoleResponseDto>.Failure("Role name cannot be empty");

            var role = await _repository.GetByNameAsync(roleName);

            if (role == null)
                return ResultDto<RoleResponseDto>.Failure("Role with name:" + roleName + " not found");

            return ResultDto<RoleResponseDto>.Success(MapToRoleResponseDto(role));
        }

        public async Task<ResultDto<List<RoleResponseDto>>> GetAllAsync()
        {
            var roles = await _repository.GetAllAsync();
            var roleDtos = roles.Select(MapToRoleResponseDto).ToList();

            if (roleDtos.Count == 0)
                return ResultDto<List<RoleResponseDto>>.Failure("No roles found");

            return ResultDto<List<RoleResponseDto>>.Success(roleDtos);
        }

        public async Task<ResultDto<bool>> UpdateAsync(Guid roleId, UpdateRoleDto dto)
        {
            if (roleId == Guid.Empty)
                return ResultDto<bool>.Failure("Role ID cannot be empty");

            if (dto is null)
                return ResultDto<bool>.Failure("Update role DTO cannot be null");

            if (string.IsNullOrWhiteSpace(dto.RoleName))
                return ResultDto<bool>.Failure("Role name is required");

            var result = await _repository.UpdateAsync(roleId, dto);
            if (result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to update role");
        }

        public async Task<ResultDto<bool>> DeleteAsync(Guid roleId)
        {
            if (roleId == Guid.Empty)
                return ResultDto<bool>.Failure("Role ID cannot be empty");

            var result = await _repository.DeleteAsync(roleId);
            if (result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to delete role");
        }

        #region Mapping Methods

        private RoleResponseDto MapToRoleResponseDto(Role role)
        {
            return new RoleResponseDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Description = role.Description,
                CreatedDate = role.CreatedDate
            };
        }

        #endregion
    }
}