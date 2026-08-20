using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersManagementApi.DTOs.Common;
using UsersManagementApi.DTOs.Users;
using UsersManagementApi.Interfaces.Services;

namespace UsersManagementApi.Controllers
{
    /// <summary>
    /// API Controller for managing users
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/Users")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status403Forbidden)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;

        /// <summary>
        /// Initializes a new instance of the UsersController class
        /// </summary>
        /// <param name="userService">The user service dependency</param>
        /// <param name="logger">The logger dependency</param>
        public UsersController(IUserService userService, IAuthorizationService authorizationService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid?>> Create([FromBody] CreateUserDto dto)
        {
            var result = await _userService.CreateAsync(dto);
            if(result.IsSuccess)
                return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { UserId = result.Value });
            else 
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Retrieves a user by its ID
        /// </summary>
        /// <param name="id">The user ID</param>
        /// <returns>The user information</returns>
        /// <response code="200">User found and returned</response>
        /// <response code="400">Invalid user ID</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id:guid}", Name = "GetUserById")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserResponseDto>> GetById(Guid id)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, id, "OwnerOrAdmin");
            if (!authorizationResult.Succeeded)
                return Forbid();

            // Call service to retrieve user
            var result = await _userService.GetByIdAsync(id);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        
        [Authorize(Roles = "Admin,User")]
        [HttpGet("by-email/{email:length(5,255)}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserResponseDto>> GetByEmail(string email)
        {
            var result = await _userService.GetByEmailAsync(email);
            if (result is not null && result.IsSuccess)
            {
                // Check if the user is authorized to access this resource
                var userId = result?.Value?.UserId;
                var authorizationResult = await _authorizationService.AuthorizeAsync(User, userId, "OwnerOrAdmin");
                if (!authorizationResult.Succeeded)
                    return Forbid();

                // Return the user information if authorized
                return Ok(result?.Value);
            }
            else
            {
                return Problem(detail: result?.Error, statusCode: StatusCodes.Status404NotFound);
            }
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(List<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserResponseDto>>> GetAll([FromQuery] PagedRequestDto dto)
        {
            var result = await _userService.GetAllAsync(dto);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        
        [Authorize(Roles = "Admin,User")]
        [HttpPut("{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessageResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorMessageResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Update(Guid userId,[FromBody] UpdateUserDto dto)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, userId, "OwnerOrAdmin");
            if (!authorizationResult.Succeeded)
                return Forbid();

            var result = await _userService.UpdateAsync(userId, dto);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPut("last-login/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessageResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorMessageResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateLastLogin(Guid userId)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, userId, "OwnerOrAdmin");
            if (!authorizationResult.Succeeded)
                return Forbid();
            var result = await _userService.UpdateLastLoginAsync(userId);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessageResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorMessageResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete(Guid userId)
        {
            var result = await _userService.DeleteAsync(userId);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("exists/{email:length(5,255)}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> Exists(string email)
        {
            var result = await _userService.ExistsByEmailAsync(email);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}