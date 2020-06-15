#if !NET451
using System;

namespace RockLib.Logging.DependencyInjection
{
    public delegate ILogProvider LogProviderRegistration(IServiceProvider serviceProvider);
}
#endif
