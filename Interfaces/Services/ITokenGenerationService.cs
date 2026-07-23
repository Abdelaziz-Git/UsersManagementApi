namespace TailorSoftAPI.Interfaces.Services
{
    public interface ITokenGenerationService
    {
        Task<string> GenerateAccessToken(
            Guid userId,
            List<string> roles,
            Dictionary<string, string>? customClaims = null);

        Task<string> GenerateRefreshToken(); 
    }
}
