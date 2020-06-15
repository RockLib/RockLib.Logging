#if !NET451
using System;

namespace RockLib.Logging.DependencyInjection
{
    public delegate IContextProvider ContextProviderRegistration(IServiceProvider serviceProvider);
}
#endif
