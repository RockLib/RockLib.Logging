#if !NET451
using System;

namespace RockLib.Logging.DependencyInjection
{
    /// <summary>
    /// A builder used to add log providers and context providers to a logger.
    /// </summary>
    public interface ILoggerBuilder
    {
        /// <summary>
        /// The name of the logger to build.
        /// </summary>
        string LoggerName { get; }

        /// <summary>
        /// Adds an <see cref="ILogProvider"/> to the logger.
        /// </summary>
        /// <param name="logProviderRegistration">A method that creates the <see cref="ILogProvider"/>.</param>
        /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
        ILoggerBuilder AddLogProvider(Func<IServiceProvider, ILogProvider> logProviderRegistration);

        /// <summary>
        /// Adds an <see cref="IContextProvider"/> to the logger.
        /// </summary>
        /// <param name="contextProviderRegistration">A method that creates the <see cref="IContextProvider"/>.</param>
        /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
        ILoggerBuilder AddContextProvider(Func<IServiceProvider, IContextProvider> contextProviderRegistration);
    }
}
#endif
