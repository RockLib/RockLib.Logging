using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using RockLib.Logging.DependencyInjection;
using System;
using System.Collections.Generic;

namespace RockLib.Logging.Http
{
    /// <summary>
    /// An action filter that records a log each time the action is executed.
    /// </summary>
    public abstract class LoggingActionFilter : Attribute, IActionFilter
    {
        /// <summary>
        /// The default value of the <see cref="MessageFormat"/> property.
        /// </summary>
        public const string DefaultMessageFormat = "Request handled by {0}.";

        /// <summary>
        /// The name of the key used to store logging context in an <see cref="HttpContext.Items"/>
        /// dictionary.
        /// </summary>
        public const string LoggingContextItemsKey = "LoggingActionFilter.LoggingContext";
        
        /// <summary>
        /// The name of the key used to store an action's result object in a <see cref=
        /// "LogEntry.ExtendedProperties"/> dictionary.
        /// </summary>
        public const string ResultObjectExtendedPropertiesKey = "ResultObject";

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
            LoggerName = string.IsNullOrEmpty(loggerName)
                ? Logger.DefaultName
                : loggerName;
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

            SetLoggingContext(context.HttpContext.Items, logger, logEntry);
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            if (!TryGetLoggingContext(context.HttpContext.Items, out var logger, out var logEntry))
                return;

            if (context.Exception != null)
                logEntry.Exception = context.Exception;

            if (context.Result is ObjectResult objectResult)
                logEntry.SetSanitizedExtendedProperty(ResultObjectExtendedPropertiesKey, objectResult.Value);

            logger.Log(logEntry);
        }

        private static void SetLoggingContext(IDictionary<object, object> httpContextItems, ILogger logger, LogEntry logEntry) =>
            httpContextItems.Add(LoggingContextItemsKey, (logger, logEntry));

        private static bool TryGetLoggingContext(IDictionary<object, object> httpContextItems, out ILogger logger, out LogEntry logEntry)
        {
            (logger, logEntry) =
                httpContextItems.TryGetValue(LoggingContextItemsKey, out object value)
                    && value is ValueTuple<ILogger, LogEntry> loggingContext
                    ? loggingContext
                    : (null, null);
            return logger != null && logEntry != null;
        }
    }
}
