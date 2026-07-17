using TailorSoftAPI.Extensions.Services.Registrations;

namespace TailorSoftAPI.Extensions.Services.Collections
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
               .AddApplicationRepositories()
               .AddApplicationServices()
               .AddApplicationDatabase()
               .AddApplicationLogging()
               .AddApplicationProblemDetails()
               .AddApplicationExceptionHandling()
               .AddApplicationCorsPolicies(configuration)
               .AddApplicationAuthentication(configuration)
               .AddApplicationOpenApi()
               .AddControllers();
            return services;
        }
    }
}