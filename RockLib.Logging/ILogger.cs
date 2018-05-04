using System.Runtime.CompilerServices;

namespace RockLib.Logging
{
    public interface ILogger
    {
        string Name { get; }
        bool IsDisabled { get; }
        LogLevel Level { get; }

        void Log(
            LogEntry logEntry,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0);
    }
}