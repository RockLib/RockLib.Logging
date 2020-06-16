#if !NET451
namespace RockLib.Logging.DependencyInjection
{
    /// <summary>
    /// Defines an interface for the options for creating an <see cref="ILogger"/>.
    /// </summary>
    public interface ILoggerOptions
    {
        /// <summary>
        /// The logging level of the logger.
        /// </summary>
        LogLevel? Level { get; set; }

        /// <summary>
        /// Whether the logger should be disabled.
        /// </summary>
        bool? IsDisabled { get; set; }

        /// <summary>
        /// Whether to create a logger that automatically reloads itself when its configuration or options change.
        /// </summary>
        bool ReloadOnChange { get; set; }
    }
}
#endif
