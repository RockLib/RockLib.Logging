namespace RockLib.Logging.AspNetCore
{
    /// <summary>
    /// Provides the logger for RockLib and Microsoft.Extensions.Logging
    /// </summary>
    public class RockLibLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider
    {
        private readonly string _rockLibLoggerName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="rockLibLoggerName">The name of the logger.</param>
        public RockLibLoggerProvider(string rockLibLoggerName = null)
        {
            _rockLibLoggerName = rockLibLoggerName;
        }

        /// <summary>
        /// Create a logger with the given category/>.
        /// </summary>
        /// <param name="categoryName">Category of the logger to be created.</param>
        /// <returns>A logger with the given category</returns>
        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            var logger = LoggerFactory.GetInstance(_rockLibLoggerName);

            return new RockLibLogger(logger, categoryName);
        }

        /// <summary>
        /// Shutdown the provider.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
