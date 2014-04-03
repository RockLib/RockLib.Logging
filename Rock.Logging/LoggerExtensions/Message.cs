namespace Rock.Logging
{
    public static partial class LoggerExtensions
    {
        public static void Log(this ILogger logger, LogLevel logLevel, string message)
        {
            var logEntry = new LogEntry
            {
                LogLevel = logLevel,
                Message = message
            };

            logger.Log(logEntry);
        }

        public static void Debug(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Debug, message);
        }

        public static void Info(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Info, message);
        }

        public static void Warn(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Warn, message);
        }

        public static void Error(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Error, message);
        }

        public static void Fatal(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Fatal, message);
        }

        public static void Audit(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Audit, message);
        }
    }
}
