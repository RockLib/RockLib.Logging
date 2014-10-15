using System;
using Rock.Defaults;

namespace Rock.Logging.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<ILogFormatterFactory> _logFormatterFactory = new DefaultHelper<ILogFormatterFactory>(() => new LogFormatterFactory(LogFormatterConfiguration.Default));

        public static ILogFormatterFactory LogFormatterFactory
        {
            get { return _logFormatterFactory.Current; }
        }

        public static ILogFormatterFactory DefaultLogFormatterFactory
        {
            get { return _logFormatterFactory.DefaultInstance; }
        }

        public static void SetLogFormatterFactory(Func<ILogFormatterFactory> getLogFormatterFactoryInstance)
        {
            _logFormatterFactory.SetCurrent(getLogFormatterFactoryInstance);
        }
    }
}
