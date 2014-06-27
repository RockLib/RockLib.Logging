using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Rock.Logging
{
    public class NullLogger : ILogger
    {
        private static readonly NullLogger _instance = new NullLogger();

        private readonly Task _completedTask = Task.FromResult(0);

        private NullLogger()
        {
        }

        public static NullLogger Instance
        {
            get { return _instance; }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public Task LogAsync(
            LogEntry logEntry,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return _completedTask;
        }
    }
}