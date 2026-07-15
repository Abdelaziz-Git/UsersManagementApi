using Microsoft.Extensions.Configuration;

namespace TailorSoftAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
               .AddRepositories()
               .AddServices()
               .AddDatabase()
               .AddApplicationLogging()
               .AddApplicationProblemDetails()
               .AddApplicationExceptionHandling()
               .AddCorsPolicies(configuration);

            return services;
        }
    }
}