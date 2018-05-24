namespace Rock.Logging
{
    /// <summary>
    /// Defines a mechanism for creating instances of <see cref="ILogger"/>.
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Get an instance of <typeparamref name="TLogger"/> for the given category.
        /// </summary>
        /// <typeparam name="TLogger">The type of <see cref="ILogger"/> to return.</typeparam>
        /// <param name="categoryName">An optional category.</param>
        /// <returns>An instance of <typeparamref name="TLogger"/>.</returns>
        TLogger Get<TLogger>(string categoryName = null)
            where TLogger : ILogger;
    }
}