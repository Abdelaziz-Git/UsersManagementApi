using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.UserRoles;
using TailorSoftAPI.Interfaces.Services;

namespace TailorSoftAPI.Controllers
{
    /// <summary>
    /// API Controller for managing user roles
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/user-roles")]
    [Produces("application/json")]
    public class UserRolesController : ControllerBase
    {
        private readonly IUserRolesService _userRolesService;
        private readonly ILogger<UserRolesController> _logger;

        /// <summary>
        /// Initializes a new instance of the UserRolesController class
        /// </summary>
        /// <param name="userRolesService">The user roles service dependency</param>
        /// <param name="logger">The logger dependency</param>
        public UserRolesController(IUserRolesService userRolesService, ILogger<UserRolesController> logger)
        {
            _userRolesService = userRolesService ?? throw new ArgumentNullException(nameof(userRolesService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Assigns a role to a user
        /// </summary>
        /// <param name="dto">The user role assignment data</param>
        /// <returns>The ID of the created user role assignment</returns>
        /// <response code="201">Role assigned successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("assign")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Assign([FromBody] AssignUserRoleDto dto)
        {
            var result = await _userRolesService.AssignAsync(dto);
            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetById), new { userRoleId = result.Value }, new { UserRoleId = result.Value });
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Retrieves a user role by its ID
        /// </summary>
        /// <param name="userRoleId">The user role ID</param>
        /// <returns>The user role information</returns>
        /// <response code="200">User role found and returned</response>
        /// <response code="404">User role not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{userRoleId:guid}", Name = "GetUserRoleById")]
        [ProducesResponseType(typeof(UserRoleResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserRoleResponseDto>> GetById(Guid userRoleId)
        {
            var result = await _userRolesService.GetByIdAsync(userRoleId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Retrieves all user roles for a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List of user roles</returns>
        /// <response code="200">User roles retrieved successfully</response>
        /// <response code="404">No user roles found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("by-user/{userId:guid}")]
        [ProducesResponseType(typeof(List<UserRoleResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserRoleResponseDto>>> GetByUserId(Guid userId)
        {
            var result = await _userRolesService.GetByUserIdAsync(userId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Retrieves all users assigned to a specific role
        /// </summary>
        /// <param name="roleId">The role ID</param>
        /// <returns>List of user roles</returns>
        /// <response code="200">User roles retrieved successfully</response>
        /// <response code="404">No user roles found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("by-role/{roleId:guid}")]
        [ProducesResponseType(typeof(List<UserRoleResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserRoleResponseDto>>> GetByRoleId(Guid roleId)
        {
            var result = await _userRolesService.GetByRoleIdAsync(roleId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Retrieves all user role assignments
        /// </summary>
        /// <returns>List of user roles</returns>
        /// <response code="200">User roles retrieved successfully</response>
        /// <response code="404">No user roles found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserRoleResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserRoleResponseDto>>> GetAll()
        {
            var result = await _userRolesService.GetAllAsync();
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Checks if a user is assigned to a specific role
        /// </summary>
        /// <param name="dto">The user and role IDs to check</param>
        /// <returns>Boolean indicating if the assignment exists</returns>
        /// <response code="200">Check performed successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("exists")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> Exists([FromBody] CheckUserRoleDto dto)
        {
            var result = await _userRolesService.ExistsAsync(dto);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Deletes a user role assignment by its ID
        /// </summary>
        /// <param name="userRoleId">The user role ID</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">User role deleted successfully</response>
        /// <response code="404">User role not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{userRoleId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete(Guid userRoleId)
        {
            var result = await _userRolesService.DeleteAsync(userRoleId);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Deletes a user role assignment by user and role IDs
        /// </summary>
        /// <param name="dto">The user and role IDs</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">User role deleted successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("by-user-and-role")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteByUserAndRole([FromBody] DeleteUserRoleDto dto)
        {
            var result = await _userRolesService.DeleteByUserAndRoleAsync(dto);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Deletes all role assignments for a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">User roles deleted successfully</response>
        /// <response code="404">No user roles found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("by-user/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAllByUserId(Guid userId)
        {
            var result = await _userRolesService.DeleteAllByUserIdAsync(userId);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}