using System;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.Logging.Tests.LogProcessingTests
{
    public class FakeLogProvider : ILogProvider
    {
        public TimeSpan Timeout => TimeSpan.FromSeconds(5);

        public LogLevel Level => LogLevel.NotSet;

#pragma warning disable 1998
        public async Task WriteAsync(LogEntry logEntry, CancellationToken cancellationToken)
        {
            throw new Exception("oh, no.");
        }
#pragma warning restore 1998
    }
}
