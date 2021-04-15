#if !NET451
using System;

namespace RockLib.Logging.DependencyInjection
{
    /// <summary>
    /// Defines an options class for creating an <see cref="ILogProvider"/>.
    /// </summary>
    public abstract class LogProviderOptions
    {
        /// <summary>
        /// The logging level of the log provider.
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// The timeout of the log provider.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// Whether to create a log provider that automatically reloads itself when its options change.
        /// </summary>
        public bool ReloadOnChange { get; set; }
    }
}
#endif
