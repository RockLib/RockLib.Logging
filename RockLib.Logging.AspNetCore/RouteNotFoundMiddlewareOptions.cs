namespace RockLib.Logging.AspNetCore
{
    /// <summary>
    /// Defines the options for the <see cref="RouteNotFoundMiddleware"/> class.
    /// </summary>
    public class RouteNotFoundMiddlewareOptions
    {
        /// <summary>
        /// Gets or sets the name of the logger.
        /// </summary>
        public string LoggerName { get; set; }

        /// <summary>
        /// Gets or sets the level used when sending logs.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Gets or sets the message used when sending logs.
        /// </summary>
        public string LogMessage { get; set; }
    }
}