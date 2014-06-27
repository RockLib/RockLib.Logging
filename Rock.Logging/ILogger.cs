using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Rock.Logging
{
    public interface ILogger : IExceptionHandler
    {
        bool IsEnabled(LogLevel logLevel);
        
        Task LogAsync(
            LogEntry logEntry,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0);
    }
}