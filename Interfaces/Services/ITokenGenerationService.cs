namespace UsersManagementApi.Interfaces.Services
{
    public interface ITokenGenerationService
    {
        Task<string> GenerateAccessToken(Guid userId, List<string> roles, Dictionary<string, string>? customClaims = null);
        Task<string> GenerateRefreshToken();
        /// <summary>
        /// Hash a refresh token using SHA256 with a salt.
        /// Returns format: "salt:hash" so it can be stored and reconstructed
        /// </summary>
        Task<string> HashRefreshToken(string refreshToken);

        /// <summary>
        /// Verify a raw refresh token against a stored hash.
        /// Extracts salt from stored hash and compares.
        /// </summary>
        Task<bool> VerifyRefreshToken(string rawToken, string storedHash);

        /// <summary>
        /// Get just the hash portion (for DB queries).
        /// Used when querying: WHERE RefreshTokenHash = GetHashPortion(token)
        /// </summary>
        Task<string?> GetHashPortion(string refreshTokenHash);
    }
}
