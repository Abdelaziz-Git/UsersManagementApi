using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TailorSoftAPI.Extensions
{
    public static class AuthenticationRegistration
    {
        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var issuer= configuration["Jwt:Issuer"]??throw new ArgumentNullException("Jwt:Issuer");
            var audience = configuration["Jwt:Audience"]??throw new ArgumentNullException("Jwt:Audience");
            var secretKey = configuration["Jwt:SecretKey"]??throw new ArgumentNullException("Jwt:SecretKey");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
    {
        // TokenValidationParameters define how incoming JWTs will be validated.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Ensures the token was issued by a trusted issuer.
            ValidateIssuer = true,


            // Ensures the token is intended for this API (audience check).
            ValidateAudience = true,


            // Ensures the token has not expired.
            ValidateLifetime = true,


            // Ensures the token signature is valid and was signed by the API.
            ValidateIssuerSigningKey = true,


            // The expected issuer value (must match the issuer used when creating the JWT).
            ValidIssuer = issuer,


            // The expected audience value (must match the audience used when creating the JWT).
            ValidAudience = audience,


            // The secret key used to validate the JWT signature.
            // This must be the same key used when generating the token.
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey))
        };
    });
            return services;
        }

    }
}
