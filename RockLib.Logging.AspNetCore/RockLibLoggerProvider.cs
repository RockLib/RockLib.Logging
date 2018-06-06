using System;

namespace RockLib.Logging.AspNetCore
{
    internal class RockLibLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider
    {
        public RockLibLoggerProvider(ILogger logger)
        {
            Logger = logger;
        }

        public ILogger Logger { get; }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            return new RockLibLogger(Logger, categoryName);
        }

        public void Dispose()
        {
        }
    }
}
