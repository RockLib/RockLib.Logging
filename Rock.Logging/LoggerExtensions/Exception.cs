using System;
using System.Runtime.CompilerServices;
using Rock.Logging.Defaults.Implementation;

namespace Rock.Logging
{
    public static partial class LoggerExtensions
    {
        // ReSharper disable ExplicitCallerInfoArgument
        public static void Log(
            this ILogger logger,
            LogLevel logLevel,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            var logEntry = Default.LogEntryFactory.CreateLogEntry();

            logEntry.LogLevel = logLevel;
            logEntry.Exception = exception;
            logEntry.Message = message ?? exception.Message;

            logger.Log(logEntry, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Debug(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Debug, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Info(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Info, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Warn(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Warn, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Error(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Error, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Fatal(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Fatal, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Audit(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Audit, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }
        // ReSharper restore ExplicitCallerInfoArgument
    }
}
