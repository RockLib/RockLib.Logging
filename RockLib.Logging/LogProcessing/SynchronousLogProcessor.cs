using System;
using System.Diagnostics;
using System.Threading;

namespace RockLib.Logging.LogProcessing
{
    public sealed class SynchronousLogProcessor : LogProcessor
    {
        protected override void WriteToLogProvider(ILogProvider logProvider, LogEntry logEntry,
            Action<ErrorEventArgs> errorHandler, int failureCount)
        {
            SynchronizationContext old = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(null);
                logProvider.WriteAsync(logEntry, CancellationToken.None).GetAwaiter().GetResult();

                TraceSource.TraceEvent(TraceEventType.Information, 0,
                    "[{0:s}] - [" + nameof(SynchronousLogProcessor) + "] - Successfully processed log entry {1} from log provider {2}.",
                    DateTime.Now, logEntry.UniqueId, logProvider);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(old);
            }
        }
    }
}
