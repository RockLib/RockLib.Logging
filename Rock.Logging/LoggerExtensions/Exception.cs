using System;
using System.Runtime.CompilerServices;

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
            [CallerMemberName] string callerMemberName = CallerMemberNameNotSet,
            [CallerFilePath] string callerFilePath = CallerFilePathNotSet,
            [CallerLineNumber] int callerLineNumber = CallerLineNumberNotSet)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            var logEntry =
                new LogEntry
                {
                    LogLevel = logLevel,
                    Exception = exception,
                    Message = message ?? exception.Message
                };

            logger.Log(logEntry, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Debug(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = CallerMemberNameNotSet,
            [CallerFilePath] string callerFilePath = CallerFilePathNotSet,
            [CallerLineNumber] int callerLineNumber = CallerLineNumberNotSet)
        {
            logger.Log(LogLevel.Debug, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Info(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = CallerMemberNameNotSet,
            [CallerFilePath] string callerFilePath = CallerFilePathNotSet,
            [CallerLineNumber] int callerLineNumber = CallerLineNumberNotSet)
        {
            logger.Log(LogLevel.Info, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Warn(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = CallerMemberNameNotSet,
            [CallerFilePath] string callerFilePath = CallerFilePathNotSet,
            [CallerLineNumber] int callerLineNumber = CallerLineNumberNotSet)
        {
            logger.Log(LogLevel.Warn, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Error(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = CallerMemberNameNotSet,
            [CallerFilePath] string callerFilePath = CallerFilePathNotSet,
            [CallerLineNumber] int callerLineNumber = CallerLineNumberNotSet)
        {
            logger.Log(LogLevel.Error, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Fatal(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = CallerMemberNameNotSet,
            [CallerFilePath] string callerFilePath = CallerFilePathNotSet,
            [CallerLineNumber] int callerLineNumber = CallerLineNumberNotSet)
        {
            logger.Log(LogLevel.Fatal, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Audit(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = CallerMemberNameNotSet,
            [CallerFilePath] string callerFilePath = CallerFilePathNotSet,
            [CallerLineNumber] int callerLineNumber = CallerLineNumberNotSet)
        {
            logger.Log(LogLevel.Audit, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }
        // ReSharper restore ExplicitCallerInfoArgument
    }
}
