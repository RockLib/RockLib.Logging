#if !NET451
namespace RockLib.Logging.DependencyInjection
{
    /// <summary>
    /// Defines an options class for creating a <see cref="RollingFileLogProvider"/>.
    /// </summary>
    public class RollingFileLogProviderOptions : FileLogProviderOptions
    {
        /// <summary>
        /// The maximum file size, in bytes, of the file. If the file size is greater than this value,
        /// it is archived.
        /// </summary>
        public int MaxFileSizeKilobytes { get; set; } = RollingFileLogProvider.DefaultMaxFileSizeKilobytes;

        /// <summary>
        /// The maximum number of archive files that will be kept. If the number of archive files is
        /// greater than this value, then they are deleted, oldest first.
        /// </summary>
        public int MaxArchiveCount { get; set; } = RollingFileLogProvider.DefaultMaxArchiveCount;

        /// <summary>
        /// The rollover period, indicating if/how the file should archived on a periodic basis.
        /// </summary>
        public RolloverPeriod RolloverPeriod { get; set; } = RollingFileLogProvider.DefaultRolloverPeriod;
    }
}
#endif
