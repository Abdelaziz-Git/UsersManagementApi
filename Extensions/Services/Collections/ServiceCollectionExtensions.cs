using TailorSoftAPI.Extensions.Services.Registrations;

namespace TailorSoftAPI.Extensions.Services.Collections
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
               .AddApplicationJwtTokenGeneration(configuration)
               .AddApplicationRepositories()
               .AddApplicationServices()
               .AddApplicationDatabase()
               .AddApplicationLogging()
               .AddApplicationProblemDetails()
               .AddApplicationExceptionHandling()
               .AddApplicationRateLimiting()
               .AddApplicationCorsPolicies(configuration)
               .AddApplicationAuthentication(configuration)
               .AddApplicationAuthorization()
               .AddApplicationOpenApi()
               .AddControllers();
            return services;
        }
    }
}