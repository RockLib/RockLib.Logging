using System;

namespace RockLib.Logging.LogProcessing
{

    public static class LogProcessorExtensions // TODO: These extension methods should be an abstract base class
    {
        public static void ProcessLogEntry(this ILogProcessor logProcessor,
            Action<ILogProvider, LogEntry, Action<ErrorEventArgs>, int> writeAction,
            ILogger logger, LogEntry logEntry, Action<ErrorEventArgs> errorHandler)
        {
            foreach (var contextProvider in logger.ContextProviders)
                contextProvider.AddContext(logEntry);

            foreach (var logProvider in logger.LogProviders)
                logProcessor.WriteToLogProvider(writeAction, logProvider, logEntry, errorHandler);
        }

        public static void WriteToLogProvider(this ILogProcessor logProcessor,
            Action<ILogProvider, LogEntry, Action<ErrorEventArgs>, int> writeAction,
            ILogProvider logProvider, LogEntry logEntry, Action<ErrorEventArgs> errorHandler, int failureCount = 0)
        {
            if (logEntry.Level < logProvider.Level)
                return;

            try
            {
                writeAction(logProvider, logEntry, errorHandler, failureCount);
            }
            catch (Exception ex)
            {
                if (errorHandler != null)
                {
                    var args = new ErrorEventArgs(
                        $"Error while sending log entry {logEntry.UniqueId} to log provider {logProvider}.",
                        ex, logProvider, logEntry, failureCount + 1);

                    errorHandler(args);

                    if (args.ShouldRetry)
                        logProcessor.WriteToLogProvider(writeAction, logProvider, logEntry, errorHandler, failureCount + 1);
                }
            }
        }
    }
}
