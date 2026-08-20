using UsersManagementApi.Interfaces.Repositories;
using UsersManagementApi.Repositories;

public static class RepositoryRegistration
{
    public static IServiceCollection AddApplicationRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserCredentialRepository, UserCredentialRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserRolesRepository, UserRolesRepository>();
        services.AddScoped<IRolePermissionsRepository, RolePermissionsRepository>();
        services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
        services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
        services.AddScoped<IUserSessionsRepository, UserSessionsRepository>();

        return services;
    }
}