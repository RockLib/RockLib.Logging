using System;
using Rock.Defaults;
using Rock.Logging.Configuration;

namespace Rock.Logging.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IConfigProvider> _configProvider = new DefaultHelper<IConfigProvider>(() => new FileConfigProvider());

        public static IConfigProvider ConfigProvider
        {
            get { return _configProvider.Current; }
        }

        public static void SetConfigProvider(Func<IConfigProvider> getConfigProviderInstance)
        {
            _configProvider.SetCurrent(getConfigProviderInstance);
        }
    }
}