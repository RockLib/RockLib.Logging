using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Rock.Logging.Defaults.Implementation;

namespace Rock.Logging
{
    public static partial class LoggerExtensions
    {
        // ReSharper disable ExplicitCallerInfoArgument
        public static Task Log(
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

            if (logLevel != LogLevel.Audit && !logger.IsEnabled(logLevel))
            {
                return _completedTask;
            }

            var logEntry = Default.LogEntryFactory.CreateLogEntry();

            logEntry.LogLevel = logLevel;
            logEntry.Exception = exception;
            logEntry.Message = message ?? exception.Message;

            return logger.Log(logEntry, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static Task Debug(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return logger.Log(LogLevel.Debug, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static Task Info(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return logger.Log(LogLevel.Info, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static Task Warn(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return logger.Log(LogLevel.Warn, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static Task Error(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return logger.Log(LogLevel.Error, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static Task Fatal(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return logger.Log(LogLevel.Fatal, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static Task Audit(
            this ILogger logger,
            Exception exception,
            string message = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return logger.Log(LogLevel.Audit, exception, message, callerMemberName, callerFilePath, callerLineNumber);
        }
        // ReSharper restore ExplicitCallerInfoArgument
    }
}
