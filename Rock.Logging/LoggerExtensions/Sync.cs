using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Rock.Logging
{
    public static partial class LoggerExtensions
    {
        public static ILogger Sync(this ILogger logger, bool synchronous = true)
        {
            return new SyncLogger(logger, synchronous);
        }

        private class SyncLogger : ILogger
        {
            private readonly ILogger _logger;
            private readonly bool _synchronous;

            public SyncLogger(ILogger logger, bool synchronous)
            {
                _logger = logger;
                _synchronous = synchronous;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return _logger.IsEnabled(logLevel);
            }

            // ReSharper disable ExplicitCallerInfoArgument
            public Task Log(
                LogEntry logEntry,
                [CallerMemberName] string callerMemberName = null,
                [CallerFilePath] string callerFilePath = null,
                [CallerLineNumber] int callerLineNumber = 0)
            {
                if (!_synchronous)
                {
                    // If we're asynchronous, just do what we would have done otherwise.
                    return _logger.Log(logEntry, callerMemberName, callerFilePath, callerLineNumber);
                }

                // We're synchronous, so wait on the task to complete.
                _logger.Log(logEntry, callerMemberName, callerFilePath, callerLineNumber).Wait();

                // Then return a task that is already completed.
                return _completedTask;
            }
            // ReSharper restore ExplicitCallerInfoArgument

            public void HandleException(Exception ex)
            {
                _logger.HandleException(ex);
            }
        }
    }
}