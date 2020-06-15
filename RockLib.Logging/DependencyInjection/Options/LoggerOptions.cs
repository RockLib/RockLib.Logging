#if !NET451
namespace RockLib.Logging.DependencyInjection
{
    public class LoggerOptions
    {
        public LogLevel? Level { get; set; }

        public bool? IsDisabled { get; set; }

        public bool ReloadOnChange { get; set; }
    }
}
#endif
