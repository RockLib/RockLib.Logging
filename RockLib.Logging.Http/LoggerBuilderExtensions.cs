using RockLib.Logging.DependencyInjection;

namespace RockLib.Logging.Http
{
    /// <summary>
    /// Extensions for <see cref="ILoggerBuilder"/>.
    /// </summary>
    public static class LoggerBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="HttpContextProvider"/> to the logger builder.
        /// </summary>
        /// <param name="builder">The logger builder.</param>
        /// <returns>The same logger builder.</returns>
        public static ILoggerBuilder AddHttpContextProvider(this ILoggerBuilder builder) =>
            builder.AddContextProvider<HttpContextProvider>();
    }
}
