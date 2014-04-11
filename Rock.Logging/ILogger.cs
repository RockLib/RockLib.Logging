using System.Threading.Tasks;

namespace Rock.Logging
{
    public interface ILogger
    {
        bool IsEnabled(LogLevel logLevel);
        Task Log(LogEntry logEntry);
    }
}