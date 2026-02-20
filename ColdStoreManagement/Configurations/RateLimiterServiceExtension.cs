using System.Threading.RateLimiting;

namespace ColdStoreManagement.Configurations
{
    /// <summary>
    /// RateLimiter Services Extensions
    /// </summary>
    public static class RateLimiterServicesExtensions
    {
        /// <summary>
        /// Rate Limiter Configs
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigRateLimiter(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                // GLOBAL LIMITER
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var key = httpContext.User.Identity?.IsAuthenticated == true
                        ? httpContext.User.Identity.Name!
                        : httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

                    return RateLimitPartition.GetSlidingWindowLimiter(
                        key,
                        _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 100,
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 6,
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                // LOGIN limiter
                options.AddPolicy("login", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1)
                        }));

                //options.OnRejected = async (context, _) =>
                //{
                //    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                //    await context.HttpContext.Response.WriteAsync(
                //        "Too many requests. Please try again later.");
                //};
                // Rejection Response
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                    // Helpful for frontend developers to know why the request failed
                    context.HttpContext.Response.Headers.RetryAfter = "60";

                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        Message = "Too many requests. Please try again in a minute.",
                        Status = 429
                    }, cancellationToken: token);
                };
            });

            return services;
        }

    }
}
