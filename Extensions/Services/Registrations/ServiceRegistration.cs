using TailorSoftAPI.Interfaces.Services;
using TailorSoftAPI.Services;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserCredentialService, UserCredentialService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IUserRolesService, UserRolesService>();
        services.AddScoped<IRolePermissionsService, RolePermissionsService>();
        services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();
        services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();
        services.AddScoped<IUserSessionsService, UserSessionsService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}