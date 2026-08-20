using UsersManagementApi.DTOs.Common;
using UsersManagementApi.DTOs.RolePermissions;
using UsersManagementApi.Interfaces.Repositories;
using UsersManagementApi.Interfaces.Services;
using UsersManagementApi.Models;

namespace UsersManagementApi.Services
{
    public class RolePermissionsService : IRolePermissionsService
    {
        private readonly IRolePermissionsRepository _repository;

        public RolePermissionsService(IRolePermissionsRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository), "Role permissions repository cannot be null");
        }

        public async Task<ResultDto<Guid>> GrantAsync(GrantPermissionDto dto)
        {
            if (dto is null)
                return ResultDto<Guid>.Failure("Grant permission DTO cannot be null");

            if (dto.RoleId == Guid.Empty)
                return ResultDto<Guid>.Failure("Role ID cannot be empty");

            if (dto.PermissionId == Guid.Empty)
                return ResultDto<Guid>.Failure("Permission ID cannot be empty");

            var guid = await _repository.GrantAsync(dto);
            if (guid is null || guid == Guid.Empty)
                return ResultDto<Guid>.Failure("Failed to grant permission to role");

            return ResultDto<Guid>.Success(guid.Value);
        }

        public async Task<ResultDto<List<RolePermissionResponseDto>>> GetByRoleIdAsync(Guid roleId)
        {
            if (roleId == Guid.Empty)
                return ResultDto<List<RolePermissionResponseDto>>.Failure("Role ID cannot be empty");

            var rolePermissions = await _repository.GetByRoleIdAsync(roleId);

            if (rolePermissions.Count == 0)
                return ResultDto<List<RolePermissionResponseDto>>.Failure("No permissions found for role ID: " + roleId);

            return ResultDto<List<RolePermissionResponseDto>>.Success(rolePermissions.Select(MapToResponseDto).ToList());
        }

        public async Task<ResultDto<List<RolePermissionResponseDto>>> GetByPermissionIdAsync(Guid permissionId)
        {
            if (permissionId == Guid.Empty)
                return ResultDto<List<RolePermissionResponseDto>>.Failure("Permission ID cannot be empty");

            var rolePermissions = await _repository.GetByPermissionIdAsync(permissionId);

            if (rolePermissions.Count == 0)
                return ResultDto<List<RolePermissionResponseDto>>.Failure("No roles found for permission ID: " + permissionId);

            return ResultDto<List<RolePermissionResponseDto>>.Success(rolePermissions.Select(MapToResponseDto).ToList());
        }

        public async Task<ResultDto<bool>> RevokeAsync(Guid roleId, Guid permissionId)
        {
            if (roleId == Guid.Empty)
                return ResultDto<bool>.Failure("Role ID cannot be empty");

            if (permissionId == Guid.Empty)
                return ResultDto<bool>.Failure("Permission ID cannot be empty");

            var result = await _repository.RevokeAsync(roleId, permissionId);
            if (result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to revoke permission from role");
        }

        public async Task<ResultDto<bool>> RevokeAllAsync(Guid roleId)
        {
            if (roleId == Guid.Empty)
                return ResultDto<bool>.Failure("Role ID cannot be empty");

            var result = await _repository.RevokeAllAsync(roleId);
            if (result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to revoke all permissions from role");
        }

        #region Mapping Methods

        private RolePermissionResponseDto MapToResponseDto(RolePermission rolePermission)
        {
            return new RolePermissionResponseDto
            {
                RolePermissionId = rolePermission.RolePermissionId,
                RoleId = rolePermission.RoleId,
                PermissionId = rolePermission.PermissionId,
                GrantedDate = rolePermission.GrantedDate
            };
        }

        #endregion
    }
}