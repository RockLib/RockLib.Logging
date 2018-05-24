using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Rock.Logging
{
    public class NullLogger : ILogger
    {
        private readonly Task _completedTask = Task.FromResult(0);

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public Task LogAsync(
            ILogEntry logEntry,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return _completedTask;
        }
    }
}