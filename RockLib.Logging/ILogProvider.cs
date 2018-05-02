using System.Collections.Generic;
using System.Threading.Tasks;

namespace RockLib.Logging
{
    public interface ILogProvider
    {
        LogLevel LoggingLevel { get; }

        IReadOnlyCollection<string> Categories { get; }

        Task WriteAsync(LogEntry logEntry);
    }
}