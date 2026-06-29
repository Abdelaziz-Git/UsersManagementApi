using TailorSoftAPI.Interfaces.Services;
using TailorSoftAPI.Services;

public static class ServiceRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserCredentialService, UserCredentialService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IUserRolesService, UserRolesService>();
        services.AddScoped<IRolePermissionsService, RolePermissionsService>();
        services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();

        return services;
    }
}