using Microsoft.AspNetCore.Builder;

namespace RockLib.Logging.AspNetCore;

/// <summary>
/// Provides extension methods for the <see cref="RouteNotFoundMiddleware"/> class.
/// </summary>
public static class RouteNotFoundMiddlewareExtensions
{
    /// <summary>
    /// Adds <see cref="RouteNotFoundMiddleware"/> to the application's request pipeline.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    public static IApplicationBuilder UseRouteNotFoundLogging(this IApplicationBuilder builder) =>
        builder.UseMiddleware<RouteNotFoundMiddleware>();
}
