using UsersManagementApi.DTOs.Authentication;
using UsersManagementApi.DTOs.Common;
using UsersManagementApi.DTOs.UserSessions;
using UsersManagementApi.Interfaces.Services;

namespace UsersManagementApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IUserCredentialService _userCredentialService;
        private readonly IUserSessionsService _userSessionsService;
        private readonly ITokenGenerationService _tokenGenerationService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserService userService,
            IUserCredentialService userCredentialService,
            IUserSessionsService sessionsService,
            ITokenGenerationService tokenGenerationService,
            ILogger<AuthService> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userCredentialService = userCredentialService ?? throw new ArgumentNullException(nameof(userCredentialService));
            _userSessionsService = sessionsService ?? throw new ArgumentNullException(nameof(sessionsService));
            _tokenGenerationService = tokenGenerationService ?? throw new ArgumentNullException(nameof(tokenGenerationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        
        public async Task<ResultDto<LoginResponseDto>> LoginAsync(LoginRequestDto loginDTO)
        {
            if (loginDTO is null || string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Password))
            {
                _logger.LogWarning("Login request DTO is null or Email or Password is null or whitespace");
                return ResultDto<LoginResponseDto>.Failure("Invalid credentials");
            }
           
            // Check if user exists by email
            var userExistResult = await _userService.ExistsByEmailAsync(loginDTO.Email);

            // Fetch user details
            var userDetailsResult = await _userService.GetUserDetailsByEmailAsync(loginDTO.Email);

            // Handle early exit for non-existent users
            if (!userExistResult.Value)
            {
                return ResultDto<LoginResponseDto>.Failure("Invalid credentials");
            }
            var userDetails = userDetailsResult?.Value;

            // FIX: Timing Attack Mitigation
            // If user doesn't exist, use a fake hash so BCrypt still runs and takes the exact same time.
            string hashToVerify = userDetails != null ? userDetails.PasswordHash : "$2a$11$123456789012345678901eUxIzrM9O6Y8cT6i8D4W6d6h6O6e6e6e";

            // 2. Perform the intensive BCrypt verification
            // Note: BCrypt.Net doesn't have true async hashing because it's CPU-bound, but running it 
            // on Task.Run keeps the thread pool responsive.
            bool isPasswordValid = await Task.Run(() => BCrypt.Net.BCrypt.Verify(loginDTO.Password, hashToVerify));

            // Handle early exit for non-existent users or locked accounts
            if (userDetails == null)
            {
                _logger.LogWarning("User not found for email {Email}", loginDTO.Email);
                return ResultDto<LoginResponseDto>.Failure("Invalid credentials");
            }


            if (userDetails.IsAccountLocked)
            {
                _logger.LogWarning("Account is temporarily locked due to too many failed login attempts");
                return ResultDto<LoginResponseDto>.Failure("Account is temporarily locked due to too many failed login attempts");
            }

            // 3. Handle Invalid Password
            if (!isPasswordValid)
            {
                _logger.LogWarning("Invalid password for user {UserId}", userDetails.UserId);

                var failedLoginDto = new FailedLoginRequestDto { MaxAttempts = 5, LockoutDurationMinutes = 30 };
                var incrementAttemptsResult = await _userCredentialService.IncrementFailedLoginAttemptsAsync(userDetails.UserId, failedLoginDto);
                if(!incrementAttemptsResult.IsSuccess)
                {
                    _logger.LogError("Failed to increment failed login attempts for user {UserId}", userDetails.UserId);
                    return ResultDto<LoginResponseDto>.Failure("Invalid credentials");
                }

                return ResultDto<LoginResponseDto>.Failure("Invalid credentials");
            }
            
            // 4. Generate tokens 
            var accessToken = await _tokenGenerationService.GenerateAccessToken(userDetails.UserId, userDetails.Roles);
            var refreshToken = await _tokenGenerationService.GenerateRefreshToken();
            var expiryDate = DateTime.UtcNow.AddDays(7);

            // FIX: Do NOT use BCrypt for Refresh Tokens! 
            var refreshTokenHash = await _tokenGenerationService.HashRefreshToken(refreshToken);
            var createSessionDto = new CreateUserSessionDto
            {
                UserId = userDetails.UserId,
                RefreshTokenHash = refreshTokenHash,
                ExpiryDate = expiryDate
            };

            var sessionResult = await _userSessionsService.CreateAsync(createSessionDto);
            if (!sessionResult.IsSuccess)
            {
                return ResultDto<LoginResponseDto>.Failure("Invalid login request");
            }

            return ResultDto<LoginResponseDto>.Success(new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
        
        public async Task<ResultDto<RefreshResponseDto>> RefreshAsync(RefreshRequestDto refreshDTO)
        {
            if (refreshDTO is null || string.IsNullOrWhiteSpace(refreshDTO.Email) || string.IsNullOrWhiteSpace(refreshDTO.RefreshToken))
            {
                _logger.LogWarning("Refresh request DTO is null or contains invalid data in refresh request");
                return ResultDto<RefreshResponseDto>.Failure("Invalid refresh request");
            }

            // Check if user exists by email
            var userExistResult = await _userService.ExistsByEmailAsync(refreshDTO.Email);

            // Get user details by email
            var userDetailsResult = await _userService.GetUserDetailsByEmailAsync(refreshDTO.Email);

            // Handle early exit for non-existent users
            if (!userExistResult.Value)
            {
                _logger.LogWarning("User not exists for email {Email}", refreshDTO.Email);
                return ResultDto<RefreshResponseDto>.Failure("Invalid refresh request");
            }
            
            // Generate new RefreshToken
            string newRefreshToken = await _tokenGenerationService.GenerateRefreshToken();

            if (!userDetailsResult.IsSuccess || userDetailsResult.Value is null)
            {
                _logger.LogWarning("User not found for email {Email} or dont have active subscription in refresh request.", refreshDTO.Email);
                return ResultDto<RefreshResponseDto>.Failure("Invalid refresh request");
            }
            var userDetails = userDetailsResult.Value;

            // Get active user sessions for the user
            var userSessionResult = await _userSessionsService.GetByUserIdAsync(userDetails.UserId, true);

            // hash new refresh token
            var newRefreshTokenHash = await _tokenGenerationService.HashRefreshToken(newRefreshToken);

            if (!userSessionResult.IsSuccess || userSessionResult.Value is null)
            {
                _logger.LogWarning("User session not found for user {UserId} with provided refresh token hash", userDetails.UserId);
                return ResultDto<RefreshResponseDto>.Failure("Invalid refresh request");
            }

            // Find active session for the user that is not revoked and has no revoked date
            var userSession = userSessionResult.Value.Find(session => !session.IsRevoked && session.RevokedDate is null);

            // Verify session belongs to the user
            if (userSession == null || userSession.UserId.ToString() != userDetails.UserId.ToString())
            {
                _logger.LogWarning("Refresh token does not belong to the user {UserId} or is invalid", userDetails.UserId);
                return ResultDto<RefreshResponseDto>.Failure("Invalid refresh request");
            }

            // Validate RefreshToken
            bool isValidRefreshToken = await _tokenGenerationService.VerifyRefreshToken(refreshDTO.RefreshToken, userSession.RefreshTokenHash);
            if (!isValidRefreshToken)
            {
                _logger.LogWarning("Invalid refresh token for user {UserId}", userDetails.UserId);
                return ResultDto<RefreshResponseDto>.Failure("Invalid refresh request");
            }
            
            // Rotate tokens in session
            var rotateDto = new RotateTokenRequestDto
            {
                OldRefreshTokenHash = userSession.RefreshTokenHash,
                NewRefreshTokenHash = newRefreshTokenHash,
                NewExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            var rotateTokenResult = await _userSessionsService.RotateTokenAsync(rotateDto);

            // Generate access token
            string newAccessToken = await _tokenGenerationService.GenerateAccessToken(userDetails.UserId, userDetails.Roles);


            if (!rotateTokenResult.IsSuccess || rotateTokenResult.Value is null)
            {
                _logger.LogWarning("Failed to rotate tokens for user {UserId}", userDetails.UserId);
                return ResultDto<RefreshResponseDto>.Failure("Failed to refresh tokens");
            }

            var rotateTokenResponse = rotateTokenResult.Value;

            if (rotateTokenResponse.IsReuseDetected)
            {
                _logger.LogWarning("Refresh token reuse detected for user {UserId}", userDetails.UserId);
                return ResultDto<RefreshResponseDto>.Failure("Refresh token reuse detected");
            }

            if (!rotateTokenResponse.IsRotated)
            {
                _logger.LogWarning("Token rotation failed for user {UserId}", userDetails.UserId);
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

            if (!userResult.IsSuccess || userResult.Value is null)
            {
                _logger.LogWarning("User not found for email {Email} in logout request", logoutDTO.Email);
                return ResultDto<bool>.Failure("Invalid logout request");
            }
            var user = userResult.Value;

            // Get active user sessions for the user
            var userSessionResult = await _userSessionsService.GetByUserIdAsync(user.UserId, activeOnly: true);
            if (!userSessionResult.IsSuccess || userSessionResult.Value is null)
            {
                _logger.LogWarning("User session not found for user {UserId} in logout request", user.UserId);
                return ResultDto<bool>.Failure("Invalid logout request");
            }

            // Find the session that matches the provided refresh token and is not revoked
            var userSession = userSessionResult.Value.Find(session => !session.IsRevoked && session.RevokedDate is null);
            if (userSession is null)
            {
                _logger.LogWarning("User session not found for user {UserId} with provided refresh token", user.UserId);
                return ResultDto<bool>.Failure("Invalid logout request");
            }
            var IsValidRefreshToken = await _tokenGenerationService.VerifyRefreshToken(logoutDTO.RefreshToken, userSession.RefreshTokenHash);
            if(!IsValidRefreshToken)
            {
                _logger.LogWarning("Invalid refresh token provided for user {UserId}", user.UserId);
                return ResultDto<bool>.Failure("Invalid logout request");
            }

            // Revoke the session
            var revokeResult = await _userSessionsService.RevokeByIdAsync(userSession.SessionId);

            if (!revokeResult.IsSuccess || !revokeResult.Value)
            {
                _logger.LogWarning("Failed to revoke session for user {UserId} with refresh token hash {RefreshTokenHash}",
                    user.UserId, userSession.RefreshTokenHash);
                return ResultDto<bool>.Failure("Failed to logout");
            }

            return ResultDto<bool>.Success(true);
        }

    }
}