using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.Roles;
using TailorSoftAPI.Interfaces.Services;

namespace TailorSoftAPI.Controllers
{
    /// <summary>
    /// API Controller for managing roles
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/roles")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status403Forbidden)]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RolesController> _logger;

        /// <summary>
        /// Initializes a new instance of the RolesController class
        /// </summary>
        /// <param name="roleService">The role service dependency</param>
        /// <param name="logger">The logger dependency</param>
        public RolesController(IRoleService roleService, ILogger<RolesController> logger)
        {
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new role
        /// </summary>
        /// <param name="dto">The role data to create</param>
        /// <returns>The ID of the created role</returns>
        /// <response code="201">Role created successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateRoleDto dto)
        {
            var result = await _roleService.CreateAsync(dto);
            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetById), new { roleId = result.Value }, new { RoleId = result.Value });
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Retrieves a role by its ID
        /// </summary>
        /// <param name="roleId">The role ID</param>
        /// <returns>The role information</returns>
        /// <response code="200">Role found and returned</response>
        /// <response code="404">Role not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{roleId:guid}", Name = "GetRoleById")]
        [ProducesResponseType(typeof(RoleResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RoleResponseDto>> GetById(Guid roleId)
        {
            var result = await _roleService.GetByIdAsync(roleId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Retrieves a role by its name
        /// </summary>
        /// <param name="roleName">The role name</param>
        /// <returns>The role information</returns>
        /// <response code="200">Role found and returned</response>
        /// <response code="404">Role not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("by-name/{roleName}")]
        [ProducesResponseType(typeof(RoleResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RoleResponseDto>> GetByName(string roleName)
        {
            var result = await _roleService.GetByNameAsync(roleName);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Retrieves all roles
        /// </summary>
        /// <returns>List of roles</returns>
        /// <response code="200">Roles retrieved successfully</response>
        /// <response code="404">No roles found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<RoleResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<RoleResponseDto>>> GetAll()
        {
            var result = await _roleService.GetAllAsync();
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Updates a role
        /// </summary>
        /// <param name="roleId">The role ID</param>
        /// <param name="dto">The role data to update</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Role updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{roleId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Update(Guid roleId, [FromBody] UpdateRoleDto dto)
        {
            var result = await _roleService.UpdateAsync(roleId, dto);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Deletes a role
        /// </summary>
        /// <param name="roleId">The role ID</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Role deleted successfully</response>
        /// <response code="404">Role not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{roleId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete(Guid roleId)
        {
            var result = await _roleService.DeleteAsync(roleId);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}