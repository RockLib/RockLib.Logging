using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RockLib.Logging.DependencyInjection;

namespace RockLib.Logging.AspNetCore;

/// <summary>
/// A middleware used to automatically log when a 404 status code occurs.
/// </summary>
public class RouteNotFoundMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// The default value of the <see cref="LogLevel"/> property.
    /// </summary>
    public const LogLevel DefaultLogLevel = LogLevel.Warn;

    /// <summary>
    /// The default value of the <see cref="LogMessage"/> property.
    /// </summary>
    public const string DefaultLogMessage = "There was an attempt to access a non-existant endpoint.";

    /// <summary>
    /// The name of the key used to store the route in a <see cref=
    /// "LogEntry.ExtendedProperties"/> dictionary.
    /// </summary>
    public const string RouteExtendedPropertiesKey = "Route";

    /// <summary>
    /// Initializes a new instance of the <see cref="RouteNotFoundMiddleware"/> class.
    /// </summary>
    /// <param name="next">The <see cref="RequestDelegate"/> used to process the HTTP request.</param>
    /// <param name="options">The options for the <see cref="RouteNotFoundMiddleware"/> instance.</param>
    public RouteNotFoundMiddleware(RequestDelegate next, IOptionsMonitor<RouteNotFoundMiddlewareOptions>? options = null)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));

        var middlewareOptions = options?.CurrentValue ?? new RouteNotFoundMiddlewareOptions();

        LoggerName = string.IsNullOrEmpty(middlewareOptions.LoggerName)
            ? Logger.DefaultName
            : middlewareOptions.LoggerName!;
        LogLevel = middlewareOptions.LogLevel == LogLevel.NotSet
            ? DefaultLogLevel
            : middlewareOptions.LogLevel;
        LogMessage = string.IsNullOrEmpty(middlewareOptions.LogMessage)
            ? DefaultLogMessage
            : middlewareOptions.LogMessage!;
    }

    /// <summary>
    /// Gets the name of the logger.
    /// </summary>
    public string LoggerName { get; set; }

    /// <summary>
    /// Gets the level used when sending logs.
    /// </summary>
    public LogLevel LogLevel { get; set; }

    /// <summary>
    /// Gets the message used when sending logs.
    /// </summary>
    public string LogMessage { get; set; }

    /// <summary>
    /// Automatically logs when a 404 status code occurs.
    /// </summary>
    /// <param name="context">The HttpContext for checking the status code.</param>
    /// <param name="loggerLookup">The lookup used to retrieve the logger for this request.</param>
    public async Task InvokeAsync(HttpContext context, LoggerLookup loggerLookup)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(context);
#else
        if (context is null) { throw new ArgumentNullException(nameof(context)); }
#endif

        await _next(context).ConfigureAwait(false);

        if (context.Response.StatusCode == 404)
        {
            if (loggerLookup is null)
            {
                return;
            }

            var logger = loggerLookup(LoggerName);

            var logEntry = new LogEntry()
            {
                Message = LogMessage,
                Level = LogLevel
            };

            logEntry.ExtendedProperties[RouteExtendedPropertiesKey] = context.Request.Path;

            logger.Log(logEntry);
        }
    }
}