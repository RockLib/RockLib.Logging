using System;
using System.Diagnostics;
using System.Threading;

namespace RockLib.Logging.LogProcessing;

/// <summary>
/// A log processor that processes logs asynchronously, but without any
/// task tracking.
/// </summary>
[Obsolete("Please use the FireAndForgetProcessor instead.", false)]
public sealed class FireAndForgetLogProcessor : LogProcessor
{
    /// <inheritdoc/>
    protected async override void SendToLogProvider(ILogProvider logProvider, LogEntry logEntry,
        IErrorHandler errorHandler, int failureCount)
    {
        try
        {
            await logProvider.WriteAsync(logEntry, CancellationToken.None).ConfigureAwait(false);

            TraceSource.TraceEvent(TraceEventType.Information, 0,
                "[{0:s}] - [" + nameof(FireAndForgetLogProcessor) + "] - Successfully processed log entry {1} from log provider {2}.",
                DateTime.UtcNow, logEntry.UniqueId, logProvider);
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
        {
            HandleError(ex, logProvider, logEntry, errorHandler, failureCount + 1,
                "Error while sending log entry {0} to log provider {1}.", logEntry.UniqueId, logProvider);
        }
    }
}
