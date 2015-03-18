using System;
using System.Configuration;
using Rock.Defaults;

namespace Rock.Logging.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<ILoggerFactory> _loggerFactory = new DefaultHelper<ILoggerFactory>(CreateDefaultLoggerFactory);

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

        public static void RestoreLoggerFactory()
        {
            _loggerFactory.RestoreDefault();
        }

        private static ILoggerFactory CreateDefaultLoggerFactory()
        {
            var loggerFactory =
                (ILoggerFactory)ConfigurationManager.GetSection("rock.logging")
                ?? new SimpleLoggerFactory<ConsoleLogProvider>(LogLevel.Debug);

            return loggerFactory.WithCaching();
        }
    }
}