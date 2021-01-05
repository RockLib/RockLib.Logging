using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using RockLib.Logging.DependencyInjection;
using System;

namespace RockLib.Logging.Http
{
    public abstract class LoggingActionFilter : Attribute, IActionFilter
    {
        public const string DefaultMessageFormat = "Request handled by {0}.";

        protected LoggingActionFilter(string messageFormat, string loggerName, LogLevel logLevel)
        {
            MessageFormat = string.IsNullOrEmpty(messageFormat)
                ? DefaultMessageFormat
                : messageFormat;
            LoggerName = loggerName;
            LogLevel = logLevel;
        }

        public string MessageFormat { get; }

        public string LoggerName { get; }

        public LogLevel LogLevel { get; }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var loggerLookup = context.HttpContext.RequestServices.GetRequiredService<LoggerLookup>();
            var logger = loggerLookup(LoggerName);

            var message = string.Format(MessageFormat, context.ActionDescriptor.DisplayName);
            var logEntry = new LogEntry(message, LogLevel, context.ActionArguments);

            SetLoggingContext(context, logger, logEntry);
        }

        public void OnActionExecuted(ActionExecutedContext context)
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
