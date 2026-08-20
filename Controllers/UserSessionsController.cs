// Controllers/UserSessionsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersManagementApi.DTOs.UserSessions;
using UsersManagementApi.Interfaces.Services;

namespace UsersManagementApi.Controllers
{
    /// <summary>
    /// API Controller for managing user sessions (refresh tokens)
    /// </summary>
    [Authorize(Roles ="Admin")]
    [ApiController]
    [Route("api/user-sessions")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status403Forbidden)]
    public class UserSessionsController : ControllerBase
    {
        private readonly IUserSessionsService _sessionsService;
        private readonly ILogger<UserSessionsController> _logger;

        public UserSessionsController(IUserSessionsService sessionsService, ILogger<UserSessionsController> logger)
        {
            _sessionsService = sessionsService ?? throw new ArgumentNullException(nameof(sessionsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>Creates a new session at login</summary>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateUserSessionDto dto)
        {
            var result = await _sessionsService.CreateAsync(dto);
            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetById), new { sessionId = result.Value }, new { SessionId = result.Value });
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>Retrieves a session by its ID</summary>
        [HttpGet("{sessionId:guid}", Name = "GetUserSessionById")]
        [ProducesResponseType(typeof(UserSessionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserSessionResponseDto>> GetById(Guid sessionId)
        {
            var result = await _sessionsService.GetByIdAsync(sessionId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>Looks up the active session behind a presented refresh token</summary>
        [HttpGet("by-refreshTokenHash/{refreshTokenHash:minlength(32)}")]
        [ProducesResponseType(typeof(UserSessionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserSessionResponseDto>> GetByRefreshTokenHash(string refreshTokenHash)
        {
            var result = await _sessionsService.GetByRefreshTokenHashAsync(refreshTokenHash);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>Lists sessions for a user ("your devices" screen)</summary>
        [HttpGet("by-user/{userId:guid}")]
        [ProducesResponseType(typeof(List<UserSessionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<UserSessionResponseDto>>> GetByUserId(Guid userId, [FromQuery] bool activeOnly = true)
        {
            var result = await _sessionsService.GetByUserIdAsync(userId, activeOnly);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>Admin listing across all users</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserSessionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<UserSessionResponseDto>>> GetAll([FromQuery] Guid? userId, [FromQuery] bool activeOnly = false)
        {
            var result = await _sessionsService.GetAllAsync(userId, activeOnly);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>Hard-deletes a session (GDPR/data-removal use only)</summary>
        [HttpDelete("{sessionId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(Guid sessionId)
        {
            var result = await _sessionsService.DeleteAsync(sessionId);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>Logs out a single device/session</summary>
        [HttpPut("revoke/{sessionId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RevokeById(Guid sessionId)
        {
            var result = await _sessionsService.RevokeByIdAsync(sessionId);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>Revokes the session tied to a specific refresh token (explicit client logout)</summary>
        [HttpPut("revoke-by-refreshTokenHash/{refreshTokenHash:minlength(32)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RevokeByRefreshTokenHash(string refreshTokenHash)
        {
            var result = await _sessionsService.RevokeByRefreshTokenHashAsync(refreshTokenHash);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>"Log out everywhere" — revokes every active session for a user</summary>
        [HttpPut("revoke-by-userId/{userId:guid}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> RevokeByUserId(Guid userId)
        {
            var result = await _sessionsService.RevokeByUserIdAsync(userId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>"Log out other devices" — revokes every session except the current one</summary>
        [HttpPut("revoke-except-current-by-userId/{userId:guid}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> RevokeAllExceptCurrent(Guid userId, [FromBody] Guid currentSessionId)
        {
            var result = await _sessionsService.RevokeAllExceptCurrentAsync(userId, currentSessionId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>Refresh-token rotation with reuse detection</summary>
        [HttpPut("rotate-token")]
        [ProducesResponseType(typeof(RotateTokenResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RotateTokenResultDto>> RotateToken([FromBody] RotateTokenRequestDto dto)
        {
            var result = await _sessionsService.RotateTokenAsync(dto);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>Lightweight check: is this refresh token still usable right now?</summary>
        [HttpGet("validate/{refreshTokenHash:minlength(32)}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> IsValid(string refreshTokenHash)
        {
            var result = await _sessionsService.IsValidAsync(refreshTokenHash);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>Nightly maintenance job: hard-deletes expired sessions</summary>
        [HttpDelete("cleanup-expired")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> CleanupExpiredTokens()
        {
            var result = await _sessionsService.CleanupExpiredTokensAsync();
            return Ok(result.Value);
        }

        /// <summary>Active session count for a user ("max concurrent devices" checks)</summary>
        [HttpGet("active-count-by-userId/{userId:guid}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> GetActiveSessionCount(Guid userId)
        {
            var result = await _sessionsService.GetActiveSessionCountAsync(userId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>Admin dashboard aggregate: session counts by state</summary>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(List<SessionStatsDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<SessionStatsDto>>> GetStats()
        {
            var result = await _sessionsService.GetStatsAsync();
            return Ok(result.Value);
        }
    }
}