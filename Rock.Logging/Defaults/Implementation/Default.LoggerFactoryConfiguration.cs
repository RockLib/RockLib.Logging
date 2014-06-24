using System;
using Rock.Defaults;

namespace Rock.Logging.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<ILoggerFactoryConfiguration> _loggerFactoryConfiguration = new DefaultHelper<ILoggerFactoryConfiguration>(() => new FileLoggerFactoryConfiguration());

        public static ILoggerFactoryConfiguration LoggerFactoryConfiguration
        {
            get { return _loggerFactoryConfiguration.Current; }
        }

        public static ILoggerFactoryConfiguration DefaultLoggerFactoryConfiguration
        {
            get { return _loggerFactoryConfiguration.DefaultInstance; }
        }

        public static void SetLoggerFactoryConfiguration(Func<ILoggerFactoryConfiguration> getLoggerFactoryConfigurationInstance)
        {
            _loggerFactoryConfiguration.SetCurrent(getLoggerFactoryConfigurationInstance);
        }
    }
}