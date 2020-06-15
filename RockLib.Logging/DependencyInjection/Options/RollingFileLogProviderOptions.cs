#if !NET451
namespace RockLib.Logging.DependencyInjection
{
    public class RollingFileLogProviderOptions : FileLogProviderOptions
    {
        public int MaxFileSizeKilobytes { get; set; } = RollingFileLogProvider.DefaultMaxFileSizeKilobytes;

        public int MaxArchiveCount { get; set; } = RollingFileLogProvider.DefaultMaxArchiveCount;

        public RolloverPeriod RolloverPeriod { get; set; } = RollingFileLogProvider.DefaultRolloverPeriod;
    }
}
#endif
