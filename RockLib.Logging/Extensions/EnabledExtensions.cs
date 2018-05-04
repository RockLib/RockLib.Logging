namespace RockLib.Logging
{
    public static class EnabledExtensions
    {
        public static bool IsDebugEnabled(this ILogger logger) => logger.IsLevelEnabled(LogLevel.Debug);

        public static bool IsInfoEnabled(this ILogger logger) => logger.IsLevelEnabled(LogLevel.Info);

        public static bool IsWarnEnabled(this ILogger logger) => logger.IsLevelEnabled(LogLevel.Warn);

        public static bool IsErrorEnabled(this ILogger logger) => logger.IsLevelEnabled(LogLevel.Error);

        public static bool IsFatalEnabled(this ILogger logger) => logger.IsLevelEnabled(LogLevel.Fatal);

        public static bool IsAuditEnabled(this ILogger logger) => logger.IsLevelEnabled(LogLevel.Audit);

        private static bool IsLevelEnabled(this ILogger logger, LogLevel level) =>
            !logger.IsDisabled && level >= logger.Level;
    }
}
