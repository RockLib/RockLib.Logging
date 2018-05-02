using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RockLib.Logging
{
    public interface ILogger
    {
        LogLevel LoggingLevel { get; }
        void Log(LogEntry logEntry,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0);
    }
}