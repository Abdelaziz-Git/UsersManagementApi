using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersManagementApi.DTOs.Common;
using UsersManagementApi.DTOs.Permissions;
using UsersManagementApi.Interfaces.Services;

namespace UsersManagementApi.Controllers
{
    /// <summary>
    /// API Controller for managing permissions
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/permissions")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status403Forbidden)]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionsController> _logger;

        /// <summary>
        /// Initializes a new instance of the PermissionsController class
        /// </summary>
        /// <param name="permissionService">The permission service dependency</param>
        /// <param name="logger">The logger dependency</param>
        public PermissionsController(IPermissionService permissionService, ILogger<PermissionsController> logger)
        {
            _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Deletes a permission
        /// </summary>
        /// <param name="permissionId">The permission ID</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Permission deleted successfully</response>
        /// <response code="404">Permission not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{permissionId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete(Guid permissionId)
        {
            var result = await _permissionService.DeleteAsync(permissionId);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Updates an existing permission
        /// </summary>
        /// <param name="permissionId">The permission ID</param>
        /// <param name="dto">The updated permission data</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Permission updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{permissionId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Update(Guid permissionId, [FromBody] UpdatePermissionDto dto)
        {
            var result = await _permissionService.UpdateAsync(permissionId, dto);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        

        /// <summary>
        /// Creates a new permission
        /// </summary>
        /// <param name="dto">The permission data to create</param>
        /// <returns>The ID of the created permission</returns>
        /// <response code="201">Permission created successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreatePermissionDto dto)
        {
            var result = await _permissionService.CreateAsync(dto);
            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetById), new { permissionId = result.Value }, new { PermissionId = result.Value });
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Retrieves a permission by its ID
        /// </summary>
        /// <param name="permissionId">The permission ID</param>
        /// <returns>The permission information</returns>
        /// <response code="200">Permission found and returned</response>
        /// <response code="404">Permission not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{permissionId:guid}", Name = "GetPermissionById")]
        [ProducesResponseType(typeof(PermissionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PermissionResponseDto>> GetById(Guid permissionId)
        {
            var result = await _permissionService.GetByIdAsync(permissionId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Retrieves all permissions for a specific module
        /// </summary>
        /// <param name="module">The module name</param>
        /// <returns>List of permissions for the module</returns>
        /// <response code="200">Permissions retrieved successfully</response>
        /// <response code="404">No permissions found for the module</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("by-module/{module}")]
        [ProducesResponseType(typeof(List<PermissionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PermissionResponseDto>>> GetByModule(string module)
        {
            var result = await _permissionService.GetAllAsync(module);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

    }
}