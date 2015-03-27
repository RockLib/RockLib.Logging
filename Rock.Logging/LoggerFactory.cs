using Rock.Logging.Defaults.Implementation;

namespace Rock.Logging
{
    /// <summary>
    /// Provides methods for creating instances of logger implementations.
    /// </summary>
    public static class LoggerFactory
    {
        /// <summary>
        /// Get a the default instance of <see cref="Logger"/>.
        /// </summary>
        /// <returns>An instance of <see cref="Logger"/>.</returns>
        public static ILogger GetInstance()
        {
            return GetInstance(null);
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
        /// <returns>An instance of <see cref="Logger"/>.</returns>
        public static ILogger GetInstance(string categoryName)
        {
            return Default.LoggerFactory.Get<Logger>(categoryName);
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
            return Default.LoggerFactory.Get<TLogger>(categoryName);
        }
    }
}