using System;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.Logging
{
    public interface ILogProvider
    {
        TimeSpan Timeout { get; }

        LogLevel Level { get; }

        Task WriteAsync(LogEntry logEntry, CancellationToken cancellationToken);
    }
}