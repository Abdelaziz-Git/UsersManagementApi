using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using TailorSoftAPI.Interfaces.Services;
using TailorSoftAPI.DTOs.Common;

namespace TailorSoftAPI.Services
{
    public class JwtTokenGenerationService : ITokenGenerationService
    {
        private readonly IOptions<JwtSettingsDto> _jwtSettings;

        public JwtTokenGenerationService(
            IOptions<JwtSettingsDto> jwtSettings)
        {
            _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
        }

        public async Task<string> GenerateAccessToken(
            Guid userId,
            List<string> roles,
            Dictionary<string, string>? customClaims = null)
        {
            // Input validation
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            if (roles == null || roles.Count == 0)
                throw new ArgumentException("At least one role must be provided", nameof(roles));

            // Validate JWT settings
            var settings = _jwtSettings.Value;
            ValidateJwtSettings(settings);

            // Create signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));
            var creds = new SigningCredentials(key, settings.Algorithm);

            // Build claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            };

            // Add roles as separate claims
            foreach (var role in roles)
            {
                if (!string.IsNullOrWhiteSpace(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
                }
            }

            // Add custom claims
            if (customClaims != null && customClaims.Count > 0)
            {
                foreach (var kvp in customClaims)
                {
                    if (!string.IsNullOrWhiteSpace(kvp.Key) && kvp.Value != null)
                    {
                        claims.Add(new Claim(kvp.Key, kvp.Value));
                    }
                }
            }

            // Create token descriptor
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(settings.ExpirationMinutes),
                SigningCredentials = creds,
                Issuer = settings.Issuer,
                Audience = settings.Audience
            };

            // Generate and return token
            var handler = new JsonWebTokenHandler();
            var token = handler.CreateToken(descriptor);

            return token;
            
        }

        public async Task<string> GenerateRefreshToken()
        {
            // Generate a random refresh token (not a JWT, just a secure random string)
            var randomBytes = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        private void ValidateJwtSettings(JwtSettingsDto settings)
        {
            if (settings == null)
                throw new InvalidOperationException("JWT settings are not configured");

            if (string.IsNullOrWhiteSpace(settings.SecretKey))
                throw new InvalidOperationException("JWT SecretKey is not configured");

            if (string.IsNullOrWhiteSpace(settings.Issuer))
                throw new InvalidOperationException("JWT Issuer is not configured");

            if (string.IsNullOrWhiteSpace(settings.Audience))
                throw new InvalidOperationException("JWT Audience is not configured");

            if (settings.ExpirationMinutes <= 0)
                throw new InvalidOperationException("JWT ExpirationMinutes must be greater than zero");

            var keyBytes = Encoding.UTF8.GetBytes(settings.SecretKey);
            if (keyBytes.Length < 32)
                throw new InvalidOperationException("JWT SecretKey must be at least 32 bytes (256 bits) for HS256");
        }
    }
}
