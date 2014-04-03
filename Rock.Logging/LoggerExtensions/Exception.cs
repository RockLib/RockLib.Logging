using System;

namespace Rock.Logging
{
    public static partial class LoggerExtensions
    {
        public static void Log(this ILogger logger, LogLevel logLevel, Exception exception, string message = null)
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

            logger.Log(logEntry);
        }

        public static void Debug(this ILogger logger, Exception exception, string message = null)
        {
            logger.Log(LogLevel.Debug, exception, message);
        }

        public static void Info(this ILogger logger, Exception exception, string message = null)
        {
            logger.Log(LogLevel.Info, exception, message);
        }

        public static void Warn(this ILogger logger, Exception exception, string message = null)
        {
            logger.Log(LogLevel.Warn, exception, message);
        }

        public static void Error(this ILogger logger, Exception exception, string message = null)
        {
            logger.Log(LogLevel.Error, exception, message);
        }

        public static void Fatal(this ILogger logger, Exception exception, string message = null)
        {
            logger.Log(LogLevel.Fatal, exception, message);
        }

        public static void Audit(this ILogger logger, Exception exception, string message = null)
        {
            logger.Log(LogLevel.Audit, exception, message);
        }
    }
}
