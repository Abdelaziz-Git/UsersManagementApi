using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.UserCredentials;
using TailorSoftAPI.Interfaces.Services;

namespace TailorSoftAPI.Controllers
{
    /// <summary>
    /// API Controller for managing user credentials (passwords, account locks, failed login tracking).
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/UserCredentials")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public class UserCredentialsController : ControllerBase
    {
        private readonly IUserCredentialService _userCredentialService;
        private readonly IAuthorizationService _authorizationService;

        /// <summary>
        /// Initializes a new instance of the UserCredentialsController class.
        /// </summary>
        /// <param name="userCredentialService">The user credential service dependency for credential operations</param>
        /// <param name="authorizationService">The authorization service for ownership-based access control</param>
        /// <exception cref="ArgumentNullException">Thrown when any dependency is null</exception>
        public UserCredentialsController(
            IUserCredentialService userCredentialService,
            IAuthorizationService authorizationService)
        {
            _userCredentialService = userCredentialService ?? throw new ArgumentNullException(nameof(userCredentialService));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateUserCredentialDto dto)
        {
            var result = await _userCredentialService.CreateAsync(dto);
            if (result.IsSuccess)
            {
                return CreatedAtAction(
                    nameof(GetByUserId),
                    new { userId = dto.UserId },
                    new { CredentialId = result.Value });
            }

            return Problem(
                detail: result.Error,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Credential creation failed");
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet("{userId:guid}", Name = "GetUserCredentialByUserId")]
        [ProducesResponseType(typeof(UserCredentialResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserCredentialResponseDto>> GetByUserId(Guid userId)
        {
            var result = await _userCredentialService.GetByUserIdAsync(userId);
            if (result.IsSuccess)
                return Ok(result.Value);

            return Problem(
                detail: result.Error,
                statusCode: StatusCodes.Status404NotFound,
                title: "Credential not found");
        }

       
        [Authorize(Roles = "Admin,User")]
        [HttpPut("password/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdatePassword(
            Guid userId,
            [FromBody] UpdateUserCredentialDto dto)
        {
            // Ownership-based authorization: only the user themselves or an Admin can change a password
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, userId, "OwnerOrAdmin");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var result = await _userCredentialService.UpdatePasswordAsync(userId, dto);
            if (result.IsSuccess)
                return NoContent();

            return Problem(
                detail: result.Error,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Password update failed");
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPut("increment-failed-login-attempts/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> IncrementFailedLoginAttempts(
            Guid userId,
            [FromBody] FailedLoginRequestDto dto)
        {
            var result = await _userCredentialService.IncrementFailedLoginAttemptsAsync(userId, dto);
            if (result.IsSuccess)
                return NoContent();

            return Problem(
                detail: result.Error,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Failed login increment operation failed");
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPut("reset-failed-login-attempts/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ResetFailedLoginAttempts(Guid userId)
        {
            var result = await _userCredentialService.ResetFailedLoginAttemptsAsync(userId);
            if (result.IsSuccess)
                return NoContent();

            return Problem(
                detail: result.Error,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Failed login reset operation failed");
        }

       
        [Authorize(Roles = "Admin,User")]
        [HttpGet("is-account-locked/{userId:guid}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> IsAccountLocked(Guid userId)
        {
            // Ownership-based authorization: users can check their own lock status, admins can check anyone's
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, userId, "OwnerOrAdmin");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var result = await _userCredentialService.IsAccountLockedAsync(userId);
            if (result.IsSuccess)
                return Ok(result.Value);

            return Problem(
                detail: result.Error,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Account lock status check failed");
        }
    }
}