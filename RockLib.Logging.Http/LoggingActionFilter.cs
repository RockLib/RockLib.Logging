using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using RockLib.Logging.DependencyInjection;
using System;

namespace RockLib.Logging.Http
{
    /// <summary>
    /// An action filter that records a log for each request.
    /// </summary>
    public abstract class LoggingActionFilter : Attribute, IActionFilter
    {
        /// <summary>
        /// The default message format string.
        /// </summary>
        public const string DefaultMessageFormat = "Request handled by {0}.";

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingActionFilter"/> class.
        /// </summary>
        /// <param name="messageFormat">
        /// The message format string. The action name is used as the <c>{0}</c> placeholder when
        /// formatting the message.
        /// </param>
        /// <param name="loggerName">The name of the logger.</param>
        /// <param name="logLevel">The level to log at.</param>
        protected LoggingActionFilter(string messageFormat, string loggerName, LogLevel logLevel)
        {
            MessageFormat = string.IsNullOrEmpty(messageFormat)
                ? DefaultMessageFormat
                : messageFormat;
            LoggerName = loggerName;
            LogLevel = logLevel;
        }

        /// <summary>
        /// Gets the message format string. The action name is used as the <c>{0}</c> placeholder
        /// when formatting the message.
        /// </summary>
        public string MessageFormat { get; }

        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        public string LoggerName { get; }

        /// <summary>
        /// Gets the level to log at.
        /// </summary>
        public LogLevel LogLevel { get; }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            var loggerLookup = context.HttpContext.RequestServices.GetRequiredService<LoggerLookup>();
            var logger = loggerLookup(LoggerName);

            var message = string.Format(MessageFormat, context.ActionDescriptor.DisplayName);
            var logEntry = new LogEntry(message, LogLevel, context.ActionArguments);

            SetLoggingContext(context, logger, logEntry);
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            var (logger, logEntry) = GetLoggingContext(context);

            if (context.Exception != null)
                logEntry.Exception = context.Exception;

            if (context.Result is ObjectResult objectResult)
                logEntry.SetSanitizedExtendedProperty("ResponseObject", objectResult.Value);

            logger.Log(logEntry);
        }

        private static void SetLoggingContext(ActionContext context, ILogger logger, LogEntry logEntry) =>
            context.HttpContext.Items.Add("RockLib.Logging.LogActionFilter.LoggingContext", (logger, logEntry));

        private static (ILogger, LogEntry) GetLoggingContext(ActionContext context) =>
            ((ILogger, LogEntry))context.HttpContext.Items["RockLib.Logging.LogActionFilter.LoggingContext"];
    }
}
