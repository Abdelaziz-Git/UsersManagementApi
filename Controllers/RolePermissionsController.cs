using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.RolePermissions;
using TailorSoftAPI.Interfaces.Services;

namespace TailorSoftAPI.Controllers
{
    /// <summary>
    /// API Controller for managing role permissions
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/role-permissions")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status403Forbidden)]
    public class RolePermissionsController : ControllerBase
    {
        private readonly IRolePermissionsService _rolePermissionsService;
        private readonly ILogger<RolePermissionsController> _logger;

        /// <summary>
        /// Initializes a new instance of the RolePermissionsController class
        /// </summary>
        /// <param name="rolePermissionsService">The role permissions service dependency</param>
        /// <param name="logger">The logger dependency</param>
        public RolePermissionsController(IRolePermissionsService rolePermissionsService, ILogger<RolePermissionsController> logger)
        {
            _rolePermissionsService = rolePermissionsService ?? throw new ArgumentNullException(nameof(rolePermissionsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Grants a permission to a role
        /// </summary>
        /// <param name="dto">The role and permission IDs to link</param>
        /// <returns>The ID of the created role permission</returns>
        /// <response code="201">Permission granted successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("grant", Name = "GrantPermission")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Grant([FromBody] GrantPermissionDto dto)
        {
            var result = await _rolePermissionsService.GrantAsync(dto);
            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetByRoleId), new { roleId = dto.RoleId }, new { RolePermissionId = result.Value });
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Retrieves all permissions assigned to a role
        /// </summary>
        /// <param name="roleId">The role ID</param>
        /// <returns>List of role permissions</returns>
        /// <response code="200">Permissions retrieved successfully</response>
        /// <response code="404">No permissions found for the role</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("by-role/{roleId:guid}", Name = "GetByRoleId")]
        [ProducesResponseType(typeof(List<RolePermissionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<RolePermissionResponseDto>>> GetByRoleId(Guid roleId)
        {
            var result = await _rolePermissionsService.GetByRoleIdAsync(roleId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Retrieves all roles assigned to a permission
        /// </summary>
        /// <param name="permissionId">The permission ID</param>
        /// <returns>List of role permissions</returns>
        /// <response code="200">Role permissions retrieved successfully</response>
        /// <response code="404">No roles found for the permission</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("by-permission/{permissionId:guid}")]
        [ProducesResponseType(typeof(List<RolePermissionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<RolePermissionResponseDto>>> GetByPermissionId(Guid permissionId)
        {
            var result = await _rolePermissionsService.GetByPermissionIdAsync(permissionId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Revokes a specific permission from a role
        /// </summary>
        /// <param name="roleId">The role ID</param>
        /// <param name="permissionId">The permission ID</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Permission revoked successfully</response>
        /// <response code="400">Invalid request data or revoke failed</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("revoke/{roleId:guid}/{permissionId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Revoke(Guid roleId, Guid permissionId)
        {
            var result = await _rolePermissionsService.RevokeAsync(roleId, permissionId);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Revokes all permissions from a role
        /// </summary>
        /// <param name="roleId">The role ID</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">All permissions revoked successfully</response>
        /// <response code="400">Invalid request data or revoke failed</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("revoke-all/{roleId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RevokeAll(Guid roleId)
        {
            var result = await _rolePermissionsService.RevokeAllAsync(roleId);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}