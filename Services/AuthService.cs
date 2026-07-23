using TailorSoftAPI.DTOs.Authentication;
using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.UserSessions;
using TailorSoftAPI.Interfaces.Services;

namespace TailorSoftAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IUserCredentialService _userCredentialService;
        private readonly IUserSessionsService _userSessionsService;
        private readonly IUserRolesService _userRolesService;
        private readonly ITokenGenerationService _tokenGenerationService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserService userService,
            IUserCredentialService credentialService,
            IUserSessionsService sessionsService,
            IUserRolesService userRolesService,
            ITokenGenerationService tokenGenerationService,
            ILogger<AuthService> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userCredentialService = credentialService ?? throw new ArgumentNullException(nameof(credentialService));
            _userSessionsService = sessionsService ?? throw new ArgumentNullException(nameof(sessionsService));
            _userRolesService = userRolesService ?? throw new ArgumentNullException(nameof(userRolesService));
            _tokenGenerationService = tokenGenerationService ?? throw new ArgumentNullException(nameof(tokenGenerationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ResultDto<LoginResponseDto>> LoginAsync(LoginRequestDto loginDTO)
        {
            if (loginDTO is null)
            {
                _logger.LogWarning("Login request DTO is null");
                return ResultDto<LoginResponseDto>.Failure("Login request DTO cannot be null");
            }

            if (string.IsNullOrWhiteSpace(loginDTO.Email))
            {
                _logger.LogWarning("Email is null or whitespace");
                return ResultDto<LoginResponseDto>.Failure("Invalid credentials");
            }

            if (string.IsNullOrWhiteSpace(loginDTO.Password))
            {
                _logger.LogWarning("Password is null or whitespace");
                return ResultDto<LoginResponseDto>.Failure("Invalid credentials");
            }

            // Get user by email
            var userResult = await _userService.GetByEmailAsync(loginDTO.Email);

            if (!userResult.IsSuccess)
            {
                _logger.LogWarning("User not found for email {Email}", loginDTO.Email);
                return ResultDto<LoginResponseDto>.Failure("Invalid credentials");
            }

            var user = userResult.Value;

            // Get user credentials
            var userCredentialResult = await _userCredentialService.GetByUserIdAsync(user.UserId);

            if (!userCredentialResult.IsSuccess)
            {
                _logger.LogWarning("User credentials not found");
                return ResultDto<LoginResponseDto>.Failure("Invalid credentials");
            }

            var userCredential = userCredentialResult.Value;

            // Check if account is locked
            var isLockedResult = await _userCredentialService.IsAccountLockedAsync(user.UserId);

            if (isLockedResult.IsSuccess && isLockedResult.Value)
            {
                _logger.LogWarning("Account is temporarily locked due to too many failed login attempts");
                return ResultDto<LoginResponseDto>.Failure("Account is temporarily locked due to too many failed login attempts");
            }

            // Verify password using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, userCredential?.PasswordHash))
            {
                _logger.LogWarning("Invalid password for user {UserId}", user.UserId);

                // Increment failed login attempts
                var failedLoginDto = new FailedLoginRequestDto
                {
                    MaxAttempts = 5,
                    LockoutDurationMinutes = 30
                };

                var incrementResult = await _userCredentialService.IncrementFailedLoginAttemptsAsync(user.UserId, failedLoginDto);

                if (!incrementResult.IsSuccess || !incrementResult.Value)
                {
                    _logger.LogWarning("Failed to increment failed login attempts for user {UserId}", user.UserId);
                }

                return ResultDto<LoginResponseDto>.Failure("Invalid credentials");
            }

            // Reset failed login attempts on successful login
            var resetResult = await _userCredentialService.ResetFailedLoginAttemptsAsync(user.UserId);

            if (resetResult.IsSuccess && resetResult.Value)
            {
                _logger.LogInformation("Reset failed login attempts for user {UserId}", user.UserId);
            }
            else
            {
                _logger.LogWarning("Failed to reset failed login attempts for user {UserId}", user.UserId);
            }

            // Get user roles
            var rolesResult = await _userRolesService.GetByUserIdAsync(user.UserId);
            var roles = rolesResult.IsSuccess
                ? rolesResult.Value?.Select(r => r.RoleName).ToList() ?? new List<string>()
                : new List<string>();

            // Generate tokens
            var accessToken = await _tokenGenerationService.GenerateAccessToken(user.UserId, roles);
            var refreshToken = await _tokenGenerationService.GenerateRefreshToken();
            var expiryDate = DateTime.UtcNow.AddDays(7);

            // Create user session
            var createSessionDto = new CreateUserSessionDto
            {
                UserId = user.UserId,
                RefreshToken = refreshToken,
                ExpiryDate = expiryDate
            };

            var sessionResult = await _userSessionsService.CreateAsync(createSessionDto);

            if (!sessionResult.IsSuccess)
                return ResultDto<LoginResponseDto>.Failure("Invalid login request");

            // Update last login
            var updateLastLoginResult = await _userService.UpdateLastLoginAsync(user.UserId);

            if (!updateLastLoginResult.IsSuccess || !updateLastLoginResult.Value)
            {
                _logger.LogWarning("Failed to update last login for user {UserId}", user.UserId);
            }

            return ResultDto<LoginResponseDto>.Success(new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        public async Task<ResultDto<RefreshResponseDto>> RefreshAsync(RefreshRequestDto refreshDTO)
        {
            if (refreshDTO is null)
            {
                _logger.LogWarning("Refresh request DTO is null in refresh request");
                return ResultDto<RefreshResponseDto>.Failure("Invalid refresh request");
            }

            if (string.IsNullOrWhiteSpace(refreshDTO.Email))
            {
                _logger.LogWarning("Email is null or whitespace in refresh request");
                return ResultDto<RefreshResponseDto>.Failure("Invalid refresh request");
            }

            if (string.IsNullOrWhiteSpace(refreshDTO.RefreshToken))
            {
                _logger.LogWarning("Refresh token is null or whitespace in refresh request");
                return ResultDto<RefreshResponseDto>.Failure("Invalid refresh request");
            }

            // Get user by email
            var userResult = await _userService.GetByEmailAsync(refreshDTO.Email);

            if (!userResult.IsSuccess)
            {
                _logger.LogWarning("User not found for email {Email} in refresh request", refreshDTO.Email);
                return ResultDto<RefreshResponseDto>.Failure("Invalid refresh request");
            }

            var user = userResult.Value;

            // Validate refresh token
            var isRefreshTokenValidResult = await _userSessionsService.IsValidAsync(refreshDTO.RefreshToken);

            if (!isRefreshTokenValidResult.IsSuccess || !isRefreshTokenValidResult.Value)
            {
                _logger.LogWarning("Invalid or expired refresh token for user {UserId}", user.UserId);
                return ResultDto<RefreshResponseDto>.Failure("Invalid refresh request");
            }

            // Get session by refresh token to verify it belongs to the user
            var userSessionResult = await _userSessionsService.GetByRefreshTokenAsync(refreshDTO.RefreshToken);

            if (!userSessionResult.IsSuccess || userSessionResult.Value is null)
            {
                _logger.LogWarning("Session not found for refresh token {RefreshToken}", refreshDTO.RefreshToken);
                return ResultDto<RefreshResponseDto>.Failure("Invalid refresh request");
            }

            var userSession = userSessionResult.Value;

            // Verify session belongs to the user
            if (userSession.UserId != user.UserId)
            {
                _logger.LogWarning("Refresh token does not belong to the user {UserId}", user.UserId);
                return ResultDto<RefreshResponseDto>.Failure("Invalid refresh request");
            }

            // Get roles for new token
            var rolesResult = await _userRolesService.GetByUserIdAsync(user.UserId);
            var roles = rolesResult.IsSuccess
                ? rolesResult.Value?.Select(r => r.RoleName).ToList() ?? new List<string>()
                : new List<string>();

            // Generate new tokens
            var newAccessToken = await _tokenGenerationService.GenerateAccessToken(user.UserId, roles);
            var newRefreshToken = await _tokenGenerationService.GenerateRefreshToken();
            var newExpiryDate = DateTime.UtcNow.AddDays(7);

            // Rotate tokens in session
            var rotateDto = new RotateTokenRequestDto
            {
                OldRefreshToken = refreshDTO.RefreshToken,
                NewRefreshToken = newRefreshToken,
                NewExpiryDate = newExpiryDate
            };

            var rotateTokenResult = await _userSessionsService.RotateTokenAsync(rotateDto);

            if (!rotateTokenResult.IsSuccess || rotateTokenResult.Value is null)
            {
                _logger.LogWarning("Failed to rotate tokens for user {UserId}", user.UserId);
                return ResultDto<RefreshResponseDto>.Failure("Failed to refresh tokens");
            }

            var rotateTokenResponse = rotateTokenResult.Value;

            if (rotateTokenResponse.IsReuseDetected)
            {
                _logger.LogWarning("Refresh token reuse detected for user {UserId}", user.UserId);
                return ResultDto<RefreshResponseDto>.Failure("Refresh token reuse detected");
            }

            if (!rotateTokenResponse.IsRotated)
            {
                _logger.LogWarning("Token rotation failed for user {UserId}", user.UserId);
                return ResultDto<RefreshResponseDto>.Failure("Failed to refresh tokens");
            }

            return ResultDto<RefreshResponseDto>.Success(new RefreshResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        public async Task<ResultDto<bool>> LogoutAsync(LogoutRequestDto logoutDTO)
        {
            if (logoutDTO is null)
            {
                _logger.LogWarning("Logout request DTO is null");
                return ResultDto<bool>.Failure("Invalid logout request");
            }

            if (string.IsNullOrWhiteSpace(logoutDTO.Email))
            {
                _logger.LogWarning("Email is null or whitespace in logout request");
                return ResultDto<bool>.Failure("Invalid logout request");
            }

            if (string.IsNullOrWhiteSpace(logoutDTO.RefreshToken))
            {
                _logger.LogWarning("Refresh token is null or whitespace in logout request");
                return ResultDto<bool>.Failure("Invalid logout request");
            }

            // Get user by email
            var userResult = await _userService.GetByEmailAsync(logoutDTO.Email);

            if (!userResult.IsSuccess)
            {
                _logger.LogWarning("User not found for email {Email} in logout request", logoutDTO.Email);
                return ResultDto<bool>.Failure("Invalid logout request");
            }

            // Revoke session by refresh token
            var revokeResult = await _userSessionsService.RevokeByRefreshTokenAsync(logoutDTO.RefreshToken);

            if (!revokeResult.IsSuccess || !revokeResult.Value)
            {
                _logger.LogWarning("Failed to revoke session for user {UserId} with refresh token {RefreshToken}",
                    userResult?.Value?.UserId, logoutDTO.RefreshToken);
                return ResultDto<bool>.Failure("Failed to logout");
            }

            return ResultDto<bool>.Success(true);
        }

       
    }
}