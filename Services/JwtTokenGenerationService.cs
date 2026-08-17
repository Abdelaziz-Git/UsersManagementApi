using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.Interfaces.Services;

namespace TailorSoftAPI.Services
{
    public class JwtTokenGenerationService : ITokenGenerationService
    {
        private readonly IOptions<JwtSettingsDto> _jwtSettings;
        private const int SaltLength = 16; // 128 bits of salt

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


        /// <summary>
        /// Hash refresh token: Generate salt + SHA256(token + salt)
        /// Format: "base64Salt:base64Hash"
        /// This format allows us to extract salt during verification
        /// </summary>
        public async Task<string> HashRefreshToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentNullException(nameof(refreshToken));

            // Generate random salt
            using (var rng = RandomNumberGenerator.Create())
            {
                var salt = new byte[SaltLength];
                rng.GetBytes(salt);

                // Hash token with salt
                using (var sha256 = SHA256.Create())
                {
                    var saltedToken = Encoding.UTF8.GetBytes(refreshToken + Convert.ToBase64String(salt));
                    var hash = sha256.ComputeHash(saltedToken);

                    // Return in format that allows reconstruction: "salt:hash"
                    return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
                }
            }
        }

        /// <summary>
        /// Verify raw token against stored hash.
        /// Extracts salt from storedHash and recomputes SHA256
        /// </summary>
        public async Task<bool> VerifyRefreshToken(string rawToken, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(rawToken) || string.IsNullOrWhiteSpace(storedHash))
                return false;

            // Extract salt and stored hash
            var parts = storedHash.Split(':');
            if (parts.Length != 2)
                return false;

            var saltBase64 = parts[0];
            var hashBase64 = parts[1];

            var salt = Convert.FromBase64String(saltBase64);
            var storedHashBytes = Convert.FromBase64String(hashBase64);

            // Recompute hash with extracted salt
            using (var sha256 = SHA256.Create())
            {
                var saltedToken = Encoding.UTF8.GetBytes(rawToken + saltBase64);
                var computedHash = sha256.ComputeHash(saltedToken);

                // Constant-time comparison to prevent timing attacks
                return ConstantTimeEquals(computedHash, storedHashBytes);
            }
        }

        /// <summary>
        /// For database queries: extract just the hash portion without salt.
        /// This allows DB-side filtering with indexed column.
        /// Note: This is only safe for tokens (deterministic), NOT passwords.
        /// </summary>
        public async Task<string?> GetHashPortion(string refreshTokenHash)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenHash))
                return null;

            // Extract just the hash part (after colon)
            var parts = refreshTokenHash.Split(':');
            return parts.Length == 2 ? parts[1] : null;
        }

        /// <summary>
        /// Constant-time comparison to prevent timing attacks.
        /// Important: Always compare full hashes, not substrings.
        /// </summary>
        private static bool ConstantTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            int result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }

            return result == 0;
        }
    }
}
