using TailorSoftAPI.Logging;

public static class LoggingRegistration
{
    public static IServiceCollection AddApplicationLogging(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerProvider, DatabaseLoggerProvider>();

        return services;
    }
}