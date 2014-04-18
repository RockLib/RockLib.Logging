using System.Runtime.CompilerServices;

namespace Rock.Logging
{
    public static partial class LoggerExtensions
    {
        // ReSharper disable ExplicitCallerInfoArgument
        public static void Log(
            this ILogger logger,
            LogLevel logLevel,
            string message,
            [CallerMemberName] string callerMemberName = CallerMemberNameNotSet,
            [CallerFilePath] string callerFilePath = CallerFilePathNotSet,
            [CallerLineNumber] int callerLineNumber = CallerLineNumberNotSet)
        {
            var logEntry = new LogEntry
            {
                LogLevel = logLevel,
                Message = message
            };

            logger.Log(logEntry, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Debug(
            this ILogger logger,
            string message,
            [CallerMemberName] string callerMemberName = CallerMemberNameNotSet,
            [CallerFilePath] string callerFilePath = CallerFilePathNotSet,
            [CallerLineNumber] int callerLineNumber = CallerLineNumberNotSet)
        {
            logger.Log(LogLevel.Debug, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Info(
            this ILogger logger,
            string message,
            [CallerMemberName] string callerMemberName = CallerMemberNameNotSet,
            [CallerFilePath] string callerFilePath = CallerFilePathNotSet,
            [CallerLineNumber] int callerLineNumber = CallerLineNumberNotSet)
        {
            logger.Log(LogLevel.Info, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Warn(
            this ILogger logger,
            string message,
            [CallerMemberName] string callerMemberName = CallerMemberNameNotSet,
            [CallerFilePath] string callerFilePath = CallerFilePathNotSet,
            [CallerLineNumber] int callerLineNumber = CallerLineNumberNotSet)
        {
            logger.Log(LogLevel.Warn, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Error(
            this ILogger logger,
            string message,
            [CallerMemberName] string callerMemberName = CallerMemberNameNotSet,
            [CallerFilePath] string callerFilePath = CallerFilePathNotSet,
            [CallerLineNumber] int callerLineNumber = CallerLineNumberNotSet)
        {
            logger.Log(LogLevel.Error, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Fatal(
            this ILogger logger,
            string message,
            [CallerMemberName] string callerMemberName = CallerMemberNameNotSet,
            [CallerFilePath] string callerFilePath = CallerFilePathNotSet,
            [CallerLineNumber] int callerLineNumber = CallerLineNumberNotSet)
        {
            logger.Log(LogLevel.Fatal, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Audit(
            this ILogger logger,
            string message,
        [CallerMemberName] string callerMemberName = CallerMemberNameNotSet,
        [CallerFilePath] string callerFilePath = CallerFilePathNotSet,
        [CallerLineNumber] int callerLineNumber = CallerLineNumberNotSet)
        {
            logger.Log(LogLevel.Audit, message, callerMemberName, callerFilePath, callerLineNumber);
        }
        // ReSharper restore ExplicitCallerInfoArgument
    }
}
