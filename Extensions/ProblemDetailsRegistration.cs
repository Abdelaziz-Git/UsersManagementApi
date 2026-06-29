public static class ProblemDetailsRegistration
{
    public static IServiceCollection AddApplicationProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] =
                    context.HttpContext.TraceIdentifier;

                context.ProblemDetails.Extensions["timestamp"] =
                    DateTime.UtcNow;

                context.ProblemDetails.Instance =
                    $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
            };
        });

        return services;
    }
}