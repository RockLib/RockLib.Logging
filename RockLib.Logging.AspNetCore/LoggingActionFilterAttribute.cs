using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using RockLib.Logging.DependencyInjection;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace RockLib.Logging.AspNetCore;

/// <summary>
/// An action filter that records a log each time an action is executed.
/// </summary>
public abstract class LoggingActionFilterAttribute : Attribute, IAsyncActionFilter
{
    /// <summary>
    /// The default value of the <see cref="MessageFormat"/> property.
    /// </summary>
    public const string DefaultMessageFormat = "Request handled by {0}.";

    /// <summary>
    /// The default value of the <see cref="ExceptionMessageFormat"/> property.
    /// </summary>
    public const string DefaultExceptionMessageFormat = "Uncaught exception in request handled by {0}.";

    /// <summary>
    /// The default value of the <see cref="LoggingActionFilterAttribute.ExceptionLogLevel"/> property.
    /// </summary>
    public const LogLevel DefaultExceptionLogLevel = LogLevel.Error;

    /// <summary>
    /// The name of the key used to store logging context in an <see cref="HttpContext.Items"/>
    /// dictionary.
    /// </summary>
    public const string LoggingContextItemsKey = "LoggingActionFilter.LoggingContext";

    /// <summary>
    /// The name of the key used to store the type of an action's result in a <see cref=
    /// "LogEntry.ExtendedProperties"/> dictionary.
    /// </summary>
    public const string ResultTypeExtendedPropertiesKey = "ResultType";

    /// <summary>
    /// The name of the key used to store an action's result object in a <see cref=
    /// "LogEntry.ExtendedProperties"/> dictionary.
    /// </summary>
    public const string ResultObjectExtendedPropertiesKey = "ResultObject";

    /// <summary>
    /// The name of the key used to store an HTTP response status code in a <see cref=
    /// "LogEntry.ExtendedProperties"/> dictionary.
    /// </summary>
    public const string ResponseStatusCodeExtendedPropertiesKey = "ResponseStatusCode";

    private string _exceptionMessageFormat = DefaultExceptionMessageFormat;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingActionFilterAttribute"/> class.
    /// </summary>
    /// <param name="messageFormat">
    /// The message format string. The action name is used as the <c>{0}</c> placeholder when
    /// formatting the message.
    /// </param>
    /// <param name="loggerName">The name of the logger.</param>
    /// <param name="logLevel">The level to log at.</param>
    protected LoggingActionFilterAttribute(string? messageFormat, string? loggerName, LogLevel logLevel)
    {
        LoggerName = string.IsNullOrEmpty(loggerName)
            ? Logger.DefaultName
            : loggerName!;
        MessageFormat = string.IsNullOrEmpty(messageFormat)
            ? DefaultMessageFormat
            : messageFormat!;
        LogLevel = logLevel;
    }

    /// <summary>
    /// Gets the name of the logger.
    /// </summary>
    public string LoggerName { get; }

    /// <summary>
    /// Gets the message format string. The action name is used as the <c>{0}</c> placeholder
    /// when formatting the message.
    /// </summary>
    public string MessageFormat { get; }

    /// <summary>
    /// Gets the level to log at.
    /// </summary>
    public LogLevel LogLevel { get; }

    /// <summary>
    /// Gets or sets the message format string to use when a request has an uncaught exception.
    /// The action name is used as the <c>{0}</c> placeholder when formatting the message.
    /// </summary>
    public string ExceptionMessageFormat
    {
        get => _exceptionMessageFormat;
        set => _exceptionMessageFormat = value ?? throw new ArgumentNullException(nameof(value));
    }
    /// <summary>
    /// Gets or sets the level to log at when a request has an uncaught exception.
    /// </summary>
    public LogLevel ExceptionLogLevel { get; set; } = DefaultExceptionLogLevel;

#pragma warning disable CA1033 // Interface methods should be callable by child types
    async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
#pragma warning restore CA1033 // Interface methods should be callable by child types
    {
        var actionExecutedContext = await next().ConfigureAwait(false);

        var loggerLookup = context.HttpContext.RequestServices.GetService<LoggerLookup>();
        if (loggerLookup is null)
        {
            return;
        }

        var logger = loggerLookup(LoggerName);

        var logEntry = new LogEntry().SetSanitizedExtendedProperties(context.ActionArguments);

        if (actionExecutedContext.Exception is null)
        {
            logEntry.Message = string.Format(CultureInfo.CurrentCulture, MessageFormat, context.ActionDescriptor.DisplayName);
            logEntry.Level = LogLevel;
        }
        else
        {
            logEntry.Message = string.Format(CultureInfo.CurrentCulture, ExceptionMessageFormat, context.ActionDescriptor.DisplayName);
            logEntry.Level = ExceptionLogLevel;
            logEntry.Exception = actionExecutedContext.Exception;
            logEntry.ExtendedProperties[ResponseStatusCodeExtendedPropertiesKey] = 500;
        }

        if (actionExecutedContext.Result is not null)
        {
            logEntry.ExtendedProperties[ResultTypeExtendedPropertiesKey] = actionExecutedContext.Result.GetType().Name;

            if (actionExecutedContext.Result is ObjectResult objectResult)
            {
                logEntry.SetSanitizedExtendedProperty(ResultObjectExtendedPropertiesKey, objectResult.Value ?? "[null]");
            }

            if (actionExecutedContext.Result is IStatusCodeActionResult statusCodeActionResult)
            {
                logEntry.ExtendedProperties[ResponseStatusCodeExtendedPropertiesKey] = statusCodeActionResult.StatusCode ?? 200;
            }
        }

        logger.Log(logEntry);
    }
}
