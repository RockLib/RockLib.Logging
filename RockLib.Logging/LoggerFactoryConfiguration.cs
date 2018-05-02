using System;
using System.Collections.Generic;

namespace RockLib.Logging
{
    public sealed class LoggerFactoryConfiguration
    {
        public LoggerFactoryConfiguration(bool isLoggingEnabled, LogLevel loggingLevel, IReadOnlyCollection<ILogProvider> logProviders)
        {
            LoggingLevel = loggingLevel;
            LogProviders = logProviders ?? throw new ArgumentNullException(nameof(logProviders));
        }

        public bool IsLoggingEnabled { get; }
        public LogLevel LoggingLevel { get; }
        public IReadOnlyCollection<ILogProvider> LogProviders { get; }
    }
}
