using Microsoft.IdentityModel.Tokens;

namespace TailorSoftAPI.DTOs.Common
{
    public class JwtSettingsDto
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; } = 15;
        public string Algorithm { get; set; } = SecurityAlgorithms.HmacSha256;
    }
}
