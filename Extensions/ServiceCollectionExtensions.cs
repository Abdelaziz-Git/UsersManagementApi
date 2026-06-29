using TailorSoftAPI.Data;
using TailorSoftAPI.Interfaces.Repositories;
using TailorSoftAPI.Interfaces.Services;
using TailorSoftAPI.Logging;
using TailorSoftAPI.Repositories;
using TailorSoftAPI.Services;
using TailorSoftAPI.Handlers;

namespace TailorSoftAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services
           .AddRepositories()
           .AddServices()
           .AddDatabase()
           .AddApplicationLogging()
           .AddApplicationProblemDetails()
           .AddApplicationExceptionHandling();

            return services;
        }
    }
}
