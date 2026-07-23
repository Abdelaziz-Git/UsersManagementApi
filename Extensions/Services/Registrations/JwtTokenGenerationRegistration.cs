using TailorSoftAPI.DTOs.Common;

namespace TailorSoftAPI.Extensions.Services.Registrations
{
    public static class JwtTokenGenerationRegistration
    {
        public static IServiceCollection AddApplicationJwtTokenGeneration(
    this IServiceCollection services,
    IConfiguration configuration)
        {
            // Validate JWT configuration at startup
            var jwtSection = configuration.GetSection("Jwt");
            services.Configure<JwtSettingsDto>(jwtSection);

            return services;
        }
    }
}
