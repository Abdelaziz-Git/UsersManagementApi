using UsersManagementApi.Data;

public static class DatabaseRegistration
{
    public static IServiceCollection AddApplicationDatabase(this IServiceCollection services)
    {
        services.AddScoped<DapperContext>();

        return services;
    }
}