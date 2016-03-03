using System.Configuration;
using Rock.Immutable;

namespace Rock.Logging
{
    /// <summary>
    /// Provides methods for creating instances of logger implementations.
    /// </summary>
    public static class LoggerFactory
    {
        private static readonly Semimutable<ILoggerFactory> _current = new Semimutable<ILoggerFactory>(GetDefault);

        public static ILoggerFactory Current
        {
            get { return _current.Value; }
        }

        public static void SetCurrent(ILoggerFactory value)
        {
            _current.Value = value;
        }

        private static ILoggerFactory GetDefault()
        {
            var loggerFactory =
                (ILoggerFactory)ConfigurationManager.GetSection("rock.logging")
                ?? new SimpleLoggerFactory<ConsoleLogProvider>(LogLevel.Debug);

            return loggerFactory.WithCaching();
        }

        /// <summary>
        /// Get a the default instance of <typeparamref name="TLogger"/>.
        /// </summary>
        /// <typeparam name="TLogger">The type of <see cref="ILogger"/> to return.</typeparam>
        /// <returns>An instance of <typeparamref name="TLogger"/>.</returns>
        public static TLogger GetInstance<TLogger>()
            where TLogger : ILogger
        {
            return GetInstance<TLogger>(null);
        }

        /// <summary>
        /// Get an instance of <see cref="Logger"/> for the given category.
        /// </summary>
        /// <param name="categoryName">The category of the logger to retrieve.</param>
        /// <param name="applicationId">The application that generated the log.</param>
        /// <returns>An instance of <see cref="Logger"/>.</returns>
        public static ILogger GetInstance(string categoryName = null, string applicationId = null)
        {
            var log = Current.Get<Logger>(categoryName);
            if (applicationId != null)
            {
                log.ApplicationId = applicationId;
            }
            return log;
        }

        /// <summary>
        /// Get an instance of <typeparamref name="TLogger"/> for the given category.
        /// </summary>
        /// <typeparam name="TLogger">The type of <see cref="ILogger"/> to return.</typeparam>
        /// <param name="categoryName">The category of the logger to retrieve.</param>
        /// <returns>An instance of <typeparamref name="TLogger"/>.</returns>
        public static TLogger GetInstance<TLogger>(string categoryName)
            where TLogger : ILogger
        {
            return Current.Get<TLogger>(categoryName);
        }
    }
}