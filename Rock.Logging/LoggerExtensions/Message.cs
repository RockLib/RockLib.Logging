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
            string message,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (logLevel != LogLevel.Audit && !logger.IsEnabled(logLevel))
            {
                return _completedTask;
            }

            var logEntry = Default.LogEntryFactory.CreateLogEntry();

            logEntry.LogLevel = logLevel;
            logEntry.Message = message;

            return logger.Log(logEntry, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static Task Debug(
            this ILogger logger,
            string message,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return logger.Log(LogLevel.Debug, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static Task Info(
            this ILogger logger,
            string message,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return logger.Log(LogLevel.Info, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static Task Warn(
            this ILogger logger,
            string message,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return logger.Log(LogLevel.Warn, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static Task Error(
            this ILogger logger,
            string message,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return logger.Log(LogLevel.Error, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static Task Fatal(
            this ILogger logger,
            string message,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return logger.Log(LogLevel.Fatal, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static Task Audit(
            this ILogger logger,
            string message,
        [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        {
            return logger.Log(LogLevel.Audit, message, callerMemberName, callerFilePath, callerLineNumber);
        }
        // ReSharper restore ExplicitCallerInfoArgument
    }
}
