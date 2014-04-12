using System.Threading.Tasks;

namespace Rock.Logging
{
    public static partial class LoggerExtensions
    {
        /// <summary>
        /// A Task that has already been completed successfully.
        /// </summary>
        private static readonly Task _completedTask = Task.FromResult(0);

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

            public Task Log(LogEntry logEntry)
            {
                if (!_synchronous)
                {
                    // If we're asynchronous, just do what we would have done otherwise.
                    return _logger.Log(logEntry);
                }

                // We're synchronous, so wait on the task to complete.
                _logger.Log(logEntry).Wait();

                // Then return a task that is already completed.
                return _completedTask;
            }
        }
    }
}