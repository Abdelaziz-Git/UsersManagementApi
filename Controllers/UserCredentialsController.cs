using Microsoft.AspNetCore.Mvc;
using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.UserCredentials;
using TailorSoftAPI.Interfaces.Services;

namespace TailorSoftAPI.Controllers
{
    /// <summary>
    /// API Controller for managing user credentials
    /// </summary>
    [ApiController]
    [Route("api/UserCredentials")]
    [Produces("application/json")]
    public class UserCredentialsController : ControllerBase
    {
        private readonly IUserCredentialService _userCredentialService;

        /// <summary>
        /// Initializes a new instance of the UserCredentialsController class
        /// </summary>
        /// <param name="userCredentialService">The user credential service dependency</param>
        public UserCredentialsController(IUserCredentialService userCredentialService)
        {
            _userCredentialService = userCredentialService ?? throw new ArgumentNullException(nameof(userCredentialService));
        }

        /// <summary>
        /// Creates a new user credential
        /// </summary>
        /// <param name="dto">The user credential data to create</param>
        /// <returns>The ID of the created user credential</returns>
        /// <response code="201">User credential created successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateUserCredentialDto dto)
        {
            var result = await _userCredentialService.CreateAsync(dto);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetByUserId), new { userId = dto.UserId }, new { CredentialId = result.Value });
            }
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Retrieves user credentials by user ID
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>The user credential information</returns>
        /// <response code="200">User credential found and returned</response>
        /// <response code="404">User credential not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{userId:guid}", Name = "GetUserCredentialByUserId")]
        [ProducesResponseType(typeof(UserCredentialResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserCredentialResponseDto>> GetByUserId(Guid userId)
        {
            var result = await _userCredentialService.GetByUserIdAsync(userId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Updates the password for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="dto">The updated password data</param>
        /// <returns>No content on success</returns>
        /// <response code="204">Password updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("password/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdatePassword(Guid userId, [FromBody] UpdateUserCredentialDto dto)
        {
            var result = await _userCredentialService.UpdatePasswordAsync(userId, dto);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Increments the failed login attempts counter for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="dto">The failed login request data</param>
        /// <returns>No content on success</returns>
        /// <response code="204">Failed login attempts incremented successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("increment-failed-login-attempts/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> IncrementFailedLoginAttempts(Guid userId, [FromBody] FailedLoginRequestDto dto)
        {
            var result = await _userCredentialService.IncrementFailedLoginAttemptsAsync(userId, dto);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Resets the failed login attempts counter for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>No content on success</returns>
        /// <response code="204">Failed login attempts reset successfully</response>
        /// <response code="400">Invalid user ID</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("reset-failed-login-attempts/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ResetFailedLoginAttempts(Guid userId)
        {
            var result = await _userCredentialService.ResetFailedLoginAttemptsAsync(userId);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Checks if a user account is locked
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>True if the account is locked, otherwise false</returns>
        /// <response code="200">Account lock status retrieved successfully</response>
        /// <response code="400">Invalid user ID</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("is-account-locked/{userId:guid}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> IsAccountLocked(Guid userId)
        {
            var result = await _userCredentialService.IsAccountLockedAsync(userId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}