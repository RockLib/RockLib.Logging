#if !NET451
using System;
using System.Collections.Generic;

namespace RockLib.Logging.DependencyInjection
{
    /// <summary>
    /// Defines the options for creating an <see cref="ILogger"/>.
    /// </summary>
    public class LoggerOptions : ILoggerOptions
    {
        /// <summary>
        /// The list of log provider registrations that create the log providers of the logger.
        /// </summary>
        public IList<Func<IServiceProvider, ILogProvider>> LogProviderRegistrations { get; } = new List<Func<IServiceProvider, ILogProvider>>();

        /// <summary>
        /// The list of context provider registrations that create the context providers of the logger.
        /// </summary>
        public IList<Func<IServiceProvider, IContextProvider>> ContextProviderRegistrations { get; } = new List<Func<IServiceProvider, IContextProvider>>();

        /// <summary>
        /// The logging level of the logger.
        /// </summary>
        public LogLevel? Level { get; set; }

        /// <summary>
        /// Whether the logger should be disabled.
        /// </summary>
        public bool? IsDisabled { get; set; }

        /// <summary>
        /// Whether to create a logger that automatically reloads itself when its configuration or options change.
        /// </summary>
        public bool ReloadOnChange { get; set; }
    }
}
#endif
