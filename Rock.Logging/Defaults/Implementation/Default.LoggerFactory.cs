using System;
using System.Configuration;
using Rock.Defaults;
using Rock.Logging.Configuration;

namespace Rock.Logging.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<ILoggerFactory> _loggerFactory = new DefaultHelper<ILoggerFactory>(GetLoggerFactoryFromConfiguration);

        public static ILoggerFactory LoggerFactory
        {
            get { return _loggerFactory.Current; }
        }

        public static ILoggerFactory DefaultLoggerFactory
        {
            get { return _loggerFactory.DefaultInstance; }
        }

        public static void SetLoggerFactory(Func<ILoggerFactory> getLoggerFactoryInstance)
        {
            _loggerFactory.SetCurrent(getLoggerFactoryInstance);
        }

        private static ILoggerFactory GetLoggerFactoryFromConfiguration()
        {
            // TODO: If the configuration is not correct, there should be a "good" exception thrown here.
            return ((XmlDeserializingLoggerFactory)ConfigurationManager.GetSection("rock.logging")).WithCaching();
        }
    }
}