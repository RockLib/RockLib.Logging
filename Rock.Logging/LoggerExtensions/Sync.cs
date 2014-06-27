using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Rock.Logging
{
    public static partial class LoggerExtensions
    {
        private static readonly ConditionalWeakTable<ILogger, SyncLogger> _syncLoggers = new ConditionalWeakTable<ILogger, SyncLogger>(); 

        public static ILogger AsSync(this ILogger logger)
        {
            return
                logger as SyncLogger
                ?? _syncLoggers.GetValue(logger, log => new SyncLogger(log));
        }

        public static ILogger AsAsync(this ILogger logger)
        {
            var syncLogger = logger as SyncLogger;

            return
                syncLogger == null
                    ? logger
                    : syncLogger.AsyncLogger;
        }

        private class SyncLogger : ILogger
        {
            private readonly ILogger _asyncLogger;

            public SyncLogger(ILogger asyncLogger)
            {
                _asyncLogger = asyncLogger;
            }

            public ILogger AsyncLogger
            {
                get { return _asyncLogger; }
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return _asyncLogger.IsEnabled(logLevel);
            }

            // ReSharper disable ExplicitCallerInfoArgument
            public Task LogAsync(
                LogEntry logEntry,
                [CallerMemberName] string callerMemberName = null,
                [CallerFilePath] string callerFilePath = null,
                [CallerLineNumber] int callerLineNumber = 0)
            {
                // Wait on the asynchronous logging operation to complete...
                _asyncLogger.LogAsync(logEntry, callerMemberName, callerFilePath, callerLineNumber).Wait();

                // ...then return a task that is already completed.
                return _completedTask;
            }
            // ReSharper restore ExplicitCallerInfoArgument

            public void HandleException(Exception ex)
            {
                _asyncLogger.HandleException(ex);
            }
        }
    }
}