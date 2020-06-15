#if !NET451
namespace RockLib.Logging.DependencyInjection
{
    public class ConsoleLogProviderOptions : FormattableLogProviderOptions
    {
        public ConsoleLogProvider.Output Output { get; set; }
    }
}
#endif
