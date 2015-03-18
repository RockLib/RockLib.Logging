using System;
using System.Collections.Generic;
using Rock.Defaults;

namespace Rock.Logging.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IEnumerable<IContextProvider>> _contextProviders = new DefaultHelper<IEnumerable<IContextProvider>>(() => new IContextProvider[0]);

        public static IEnumerable<IContextProvider> ContextProviders
        {
            get { return _contextProviders.Current; }
        }

        public static IEnumerable<IContextProvider> DefaultContextProviders
        {
            get { return _contextProviders.DefaultInstance; }
        }

        public static void SetContextProviders(Func<IEnumerable<IContextProvider>> getContextProviderInstance)
        {
            _contextProviders.SetCurrent(getContextProviderInstance);
        }

        public static void RestoreContextProviders()
        {
            _contextProviders.RestoreDefault();
        }
    }
}
