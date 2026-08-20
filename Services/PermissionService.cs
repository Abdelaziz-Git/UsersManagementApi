using UsersManagementApi.DTOs.Common;
using UsersManagementApi.DTOs.Permissions;
using UsersManagementApi.Interfaces.Repositories;
using UsersManagementApi.Interfaces.Services;
using UsersManagementApi.Models;

namespace UsersManagementApi.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _repository;

        public PermissionService(IPermissionRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository), "Permission repository cannot be null");
        }

        public async Task<ResultDto<Guid>> CreateAsync(CreatePermissionDto dto)
        {
            if (dto is null)
                return ResultDto<Guid>.Failure("Create permission DTO cannot be null");

            if (string.IsNullOrWhiteSpace(dto.PermissionName))
                return ResultDto<Guid>.Failure("Permission name is required");

            if (string.IsNullOrWhiteSpace(dto.Module))
                return ResultDto<Guid>.Failure("Module is required");

            var guid = await _repository.CreateAsync(dto);
            if (guid is null || guid == Guid.Empty)
                return ResultDto<Guid>.Failure("Failed to create permission");

            return ResultDto<Guid>.Success(guid.Value);
        }

        public async Task<ResultDto<PermissionResponseDto>> GetByIdAsync(Guid permissionId)
        {
            if (permissionId == Guid.Empty)
                return ResultDto<PermissionResponseDto>.Failure("Permission ID cannot be empty");

            var permission = await _repository.GetByIdAsync(permissionId);

            if (permission is null)
                return ResultDto<PermissionResponseDto>.Failure($"Permission with ID: {permissionId} not found");

            return ResultDto<PermissionResponseDto>.Success(MapToPermissionResponseDto(permission));
        }

        public async Task<ResultDto<List<PermissionResponseDto>>> GetAllAsync(string module)
        {
            if (string.IsNullOrWhiteSpace(module))
                return ResultDto<List<PermissionResponseDto>>.Failure("Module cannot be empty");

            var permissions = await _repository.GetAllAsync(module);
            var permissionDtos = permissions.Select(MapToPermissionResponseDto).ToList();

            if (permissionDtos.Count == 0)
                return ResultDto<List<PermissionResponseDto>>.Failure($"No permissions found for module '{module}'");

            return ResultDto<List<PermissionResponseDto>>.Success(permissionDtos);
        }

        public async Task<ResultDto<bool>> UpdateAsync(Guid permissionId, UpdatePermissionDto dto)
        {
            if (permissionId == Guid.Empty)
                return ResultDto<bool>.Failure("Permission ID cannot be empty");

            if (dto is null)
                return ResultDto<bool>.Failure("Update permission DTO cannot be null");

            if (string.IsNullOrWhiteSpace(dto.PermissionName))
                return ResultDto<bool>.Failure("Permission name is required");

            if (string.IsNullOrWhiteSpace(dto.Module))
                return ResultDto<bool>.Failure("Module is required");

            var result = await _repository.UpdateAsync(permissionId, dto);
            if (result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to update permission");
        }

        public async Task<ResultDto<bool>> DeleteAsync(Guid permissionId)
        {
            if (permissionId == Guid.Empty)
                return ResultDto<bool>.Failure("Permission ID cannot be empty");

            var result = await _repository.DeleteAsync(permissionId);
            if (result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to delete permission");
        }

        #region Mapping Methods

        private PermissionResponseDto MapToPermissionResponseDto(Permission permission)
        {
            return new PermissionResponseDto
            {
                PermissionId = permission.PermissionId,
                PermissionName = permission.PermissionName,
                Module = permission.Module,
                Description = permission.Description
            };
        }

        #endregion
    }
}