using UsersManagementApi.Handlers;

public static class ExceptionRegistration
{
    public static IServiceCollection AddApplicationExceptionHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}