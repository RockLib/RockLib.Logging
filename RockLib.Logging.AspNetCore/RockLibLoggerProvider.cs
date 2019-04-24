using Microsoft.Extensions.Logging;
using System;

namespace RockLib.Logging.AspNetCore
{
    internal class RockLibLoggerProvider : ILoggerProvider
    {
        public RockLibLoggerProvider(Func<ILogger> createLogger) => CreateLogger = createLogger;

        public Func<ILogger> CreateLogger { get; }

        Microsoft.Extensions.Logging.ILogger ILoggerProvider.CreateLogger(string categoryName) =>
            new RockLibLogger(CreateLogger(), categoryName);

        public void Dispose() {}
    }
}
