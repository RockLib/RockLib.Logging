#if !NET451
namespace RockLib.Logging.DependencyInjection
{
    /// <summary>
    /// Defines an options class for creating a <see cref="ConsoleLogProvider"/>.
    /// </summary>
    public class ConsoleLogProviderOptions : FormattableLogProviderOptions
    {
        /// <summary>
        /// The type of console output stream to write to.
        /// </summary>
        public ConsoleLogProvider.Output Output { get; set; }
    }
}
#endif
