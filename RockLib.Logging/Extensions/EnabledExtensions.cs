namespace RockLib.Logging
{
    /// <summary>
    /// Defines extension method that determine if a given logging level is enabled.
    /// </summary>
    public static class EnabledExtensions
    {
        /// <summary>
        /// Returns whether debug logging is enabled for the logger.
        /// </summary>
        /// <param name="logger">The logger to check.</param>
        /// <returns>True if debug logging is enabled, otherwise false.</returns>
        public static bool IsDebugEnabled(this ILogger logger) => logger.IsLevelEnabled(LogLevel.Debug);

        /// <summary>
        /// Returns whether info logging is enabled for the logger.
        /// </summary>
        /// <param name="logger">The logger to check.</param>
        /// <returns>True if info logging is enabled, otherwise false.</returns>
        public static bool IsInfoEnabled(this ILogger logger) => logger.IsLevelEnabled(LogLevel.Info);

        /// <summary>
        /// Returns whether warn logging is enabled for the logger.
        /// </summary>
        /// <param name="logger">The logger to check.</param>
        /// <returns>True if warn logging is enabled, otherwise false.</returns>
        public static bool IsWarnEnabled(this ILogger logger) => logger.IsLevelEnabled(LogLevel.Warn);

        /// <summary>
        /// Returns whether error logging is enabled for the logger.
        /// </summary>
        /// <param name="logger">The logger to check.</param>
        /// <returns>True if error logging is enabled, otherwise false.</returns>
        public static bool IsErrorEnabled(this ILogger logger) => logger.IsLevelEnabled(LogLevel.Error);

        /// <summary>
        /// Returns whether fatal logging is enabled for the logger.
        /// </summary>
        /// <param name="logger">The logger to check.</param>
        /// <returns>True if fatal logging is enabled, otherwise false.</returns>
        public static bool IsFatalEnabled(this ILogger logger) => logger.IsLevelEnabled(LogLevel.Fatal);

        /// <summary>
        /// Returns whether audit logging is enabled for the logger.
        /// </summary>
        /// <param name="logger">The logger to check.</param>
        /// <returns>True if audit logging is enabled, otherwise false.</returns>
        public static bool IsAuditEnabled(this ILogger logger) => logger.IsLevelEnabled(LogLevel.Audit);

        private static bool IsLevelEnabled(this ILogger logger, LogLevel level) =>
            !logger.IsDisabled && level >= logger.Level;
    }
}
