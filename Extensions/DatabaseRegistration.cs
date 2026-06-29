using TailorSoftAPI.Data;

public static class DatabaseRegistration
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddScoped<DapperContext>();

        return services;
    }
}