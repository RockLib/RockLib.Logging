namespace Rock.Logging
{
    public static partial class LoggerExtensions
    {
        public static bool IsDebugEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Debug);
        }
        
        public static bool IsInfoEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Info);
        }

        public static bool IsWarnEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Warn);
        }

        public static bool IsErrorEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Error);
        }

        public static bool IsFatalEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Fatal);
        }

        public static bool IsAuditEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Audit);
        }
    }
}
