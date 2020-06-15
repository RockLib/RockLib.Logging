#if !NET451
using System;

namespace RockLib.Logging.DependencyInjection
{
    public delegate ILogFormatter LogFormatterRegistration(IServiceProvider serviceProvider);
}
#endif
