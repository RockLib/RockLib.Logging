using System;
using System.Runtime.CompilerServices;

namespace RockLib.Logging
{
    public static class LoggingExtensions
    {
        public static void Debug(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Debug, message, null, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Debug(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Debug, message, exception, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Info(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Info, message, null, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Info(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Info, message, exception, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Warn(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Warn, message, null, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Warn(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Warn, message, exception, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Error(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Error, message, null, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Error(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Error, message, exception, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Fatal(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Fatal, message, null, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Fatal(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Fatal, message, exception, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Audit(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Audit, message, null, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Audit(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0) =>
            logger.Log(LogLevel.Audit, message, exception, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);

        private static void Log(this ILogger logger,
            LogLevel level,
            string message,
            Exception exception,
            object extendedProperties,
            string callerMemberName,
            string callerFilePath,
            int callerLineNumber)
        {
            if (logger.IsDisabled || level < logger.Level)
                return;

            var logEntry = new LogEntry(level, message, exception, extendedProperties);
            logger.Log(logEntry, callerMemberName, callerFilePath, callerLineNumber);
        }
    }
}
