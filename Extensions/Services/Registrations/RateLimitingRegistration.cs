using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace UsersManagementApi.Extensions.Services.Registrations
{
    public static class RateLimitingRegistration
    {
        public class RateLimitingLog() { }
        public static IServiceCollection AddApplicationRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, cancellationToken) =>
                {
                    var httpContext = context.HttpContext;
                    var logger = httpContext.RequestServices.GetRequiredService<ILogger<RateLimitingLog>>();

                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        httpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                    }
                    else
                    {
                        httpContext.Response.Headers.RetryAfter = "5 min";
                    }
                    var problem = new ProblemDetails
                    {
                        Type = "https://datatracker.ietf.org/doc/html/rfc6585#section-4",
                        Title = "Too many requests",
                        Status = StatusCodes.Status429TooManyRequests,
                        Detail = "You have exceeded the rate limit for this endpoint. Slow down and retry after the Retry-After header value.",
                        Instance = httpContext.Request.Path
                    };
                    problem.Extensions["traceId"] = httpContext.TraceIdentifier;

                    logger.LogWarning(
                        "Rate limit exceeded. Path: {Path} Client: {Client} TraceId: {TraceId}",
                        httpContext.Request.Path,
                        httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                        httpContext.TraceIdentifier);

                    httpContext.Response.ContentType = "application/problem+json";
                    await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
                };

                
                options.AddSlidingWindowLimiter("auth-endpoints", opt =>
                {
                    opt.PermitLimit = 10;
                    opt.Window = TimeSpan.FromMinutes(5);
                    opt.SegmentsPerWindow = 6;   // 10-second segments
                    opt.QueueLimit = 0;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                options.AddTokenBucketLimiter("public-endpoints", opt =>
                {
                    opt.TokenLimit = 100;
                    opt.TokensPerPeriod = 100;
                    opt.ReplenishmentPeriod = TimeSpan.FromMinutes(1);
                    opt.AutoReplenishment = true;
                    opt.QueueLimit = 0;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>
                (
                  httpContext => RateLimitPartition.GetTokenBucketLimiter
                  (
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                    factory: _ => new TokenBucketRateLimiterOptions
                      {
                          TokenLimit = 200,
                          TokensPerPeriod = 200,
                          ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                          AutoReplenishment = true,
                          QueueLimit = 0
                      }
                  )
                );
            });
            return services;
        }
    }
}
