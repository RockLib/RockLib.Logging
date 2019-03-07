using System;
using System.Threading;

namespace RockLib.Logging.LogProcessing
{
    public sealed class FireAndForgetLogProcessor : LogProcessor
    {
        protected override async void WriteToLogProvider(ILogProvider logProvider, LogEntry logEntry,
            Action<ErrorEventArgs> errorHandler, int failureCount)
        {
            try
            {
                await logProvider.WriteAsync(logEntry, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                HandleError(ex, logProvider, logEntry, errorHandler, failureCount + 1,
                    "Error while sending log entry {0} to log provider {1}.", logEntry.UniqueId, logProvider);
            }
        }
    }
}
