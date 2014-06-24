using System;
using Rock.Defaults;

namespace Rock.Logging.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<ILoggerFactory> _loggerFactory = new DefaultHelper<ILoggerFactory>(() => new LoggerFactory().WithCaching());

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
    }
}