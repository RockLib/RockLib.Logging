using System;
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
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(old);
            }
        }
    }
}
