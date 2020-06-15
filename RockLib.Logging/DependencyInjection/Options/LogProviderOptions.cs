#if !NET451
using System;

namespace RockLib.Logging.DependencyInjection
{
    public abstract class LogProviderOptions
    {
        public LogLevel Level { get; set; }

        public TimeSpan? Timeout { get; set; }
    }
}
#endif
