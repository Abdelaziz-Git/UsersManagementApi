using Microsoft.EntityFrameworkCore;
using UsersManagementApi.Data;

public static class DatabaseRegistration
{
    public static IServiceCollection AddApplicationDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<Context>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<DapperContext>();

        return services;
    }
}