using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TailorSoftAPI.Extensions.Services.Registrations
{
    /// <summary>
    /// Provides extension methods for registering authorization services and policies in the application.
    /// </summary>
    public static class AuthorizationRegistration
    {
        public static IServiceCollection AddApplicationAuthorization(
            this IServiceCollection services)
        {
            // Register authorization service
            services.AddAuthorization(options =>
            {
                // Policy: OwnerOrAdmin - User is owner or admin
                options.AddPolicy("OwnerOrAdmin",
                    policy => policy.Requirements.Add(new OwnerOrAdminRequirement()));

                
            });

            // Register the authorization handler
            services.AddScoped<IAuthorizationHandler, OwnerOrAdminHandler>();

            return services;
        }
    }

    /// <summary>
    /// Authorization handler for the "OwnerOrAdmin" policy.
    /// </summary>
    public class OwnerOrAdminHandler : AuthorizationHandler<OwnerOrAdminRequirement, Guid>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OwnerOrAdminRequirement requirement,
            Guid UserId)
        {
            // Check if user is Admin
            if (context.User.HasClaim(ClaimTypes.Role, "Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Check Ownership with detailed info
            var allClaims = context.User.Claims.ToList();
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Log for debugging (remove in production)
            System.Diagnostics.Debug.WriteLine($"Required UserId: {UserId}");
            System.Diagnostics.Debug.WriteLine($"Claim UserId: {userIdClaim}");
            System.Diagnostics.Debug.WriteLine($"All Claims: {string.Join(", ", allClaims.Select(c => $"{c.Type}={c.Value}"))}");
            if (userIdClaim == UserId.ToString())
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Represents a requirement for the "OwnerOrAdmin" authorization policy.
    /// </summary>
    public class OwnerOrAdminRequirement : IAuthorizationRequirement
    {
        public OwnerOrAdminRequirement() { }
    }
}
