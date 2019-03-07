using System;
using System.Diagnostics;
using System.Threading;

namespace RockLib.Logging.LogProcessing
{
    /// <summary>
    /// A log processor that processes logs asynchronously, but without any
    /// task tracking.
    /// </summary>
    public sealed class FireAndForgetLogProcessor : LogProcessor
    {
        /// <inheritdoc/>
        protected override async void SendToLogProvider(ILogProvider logProvider, LogEntry logEntry,
            Action<ErrorEventArgs> errorHandler, int failureCount)
        {
            try
            {
                await logProvider.WriteAsync(logEntry, CancellationToken.None).ConfigureAwait(false);

                TraceSource.TraceEvent(TraceEventType.Information, 0,
                    "[{0:s}] - [" + nameof(FireAndForgetLogProcessor) + "] - Successfully processed log entry {1} from log provider {2}.",
                    DateTime.Now, logEntry.UniqueId, logProvider);
            }
            catch (Exception ex)
            {
                HandleError(ex, logProvider, logEntry, errorHandler, failureCount + 1,
                    "Error while sending log entry {0} to log provider {1}.", logEntry.UniqueId, logProvider);
            }
        }
    }
}
