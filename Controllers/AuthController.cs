using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TailorSoftAPI.DTOs.Authentication;
using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.Interfaces.Services;

namespace TailorSoftAPI.Controllers
{
    /// <summary>
    /// API Controller for managing authentication
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/Auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the AuthController class
        /// </summary>
        /// <param name="authService">The authentication service dependency</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        /// <summary>
        /// Authenticates a user and returns login credentials
        /// </summary>
        /// <param name="dto">The login credentials</param>
        /// <returns>Login response with authentication tokens</returns>
        /// <response code="200">User authenticated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="500">Internal server error</response>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(ResultDto<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status401Unauthorized);
        }

        /// <summary>
        /// Refreshes the authentication token
        /// </summary>
        /// <param name="dto">The refresh token request</param>
        /// <returns>New authentication tokens</returns>
        /// <response code="200">Token refreshed successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">Invalid or expired refresh token</response>
        /// <response code="500">Internal server error</response>
        [AllowAnonymous]
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(ResultDto<RefreshResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RefreshResponseDto>> Refresh([FromBody] RefreshRequestDto dto)
        {
            var result = await _authService.RefreshAsync(dto);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status401Unauthorized);
        }

        /// <summary>
        /// Logs out the current user
        /// </summary>
        /// <param name="dto">The logout request</param>
        /// <returns>Logout confirmation</returns>
        /// <response code="200">User logged out successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("logout")]
        [ProducesResponseType(typeof(ResultDto<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> Logout([FromBody] LogoutRequestDto dto)
        {
            var result = await _authService.LogoutAsync(dto);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status401Unauthorized);
        }
    }
}