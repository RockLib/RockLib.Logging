﻿using System;
using System.Diagnostics;
using System.Threading;

namespace RockLib.Logging.LogProcessing;

/// <summary>
/// A log processor that processes logs on the same thread as the caller.
/// </summary>
[Obsolete("Please use the FireAndForgetProcessor instead.", false)]
public sealed class SynchronousLogProcessor : LogProcessor
{
    /// <inheritdoc/>
    protected override void SendToLogProvider(ILogProvider logProvider, LogEntry logEntry,
        IErrorHandler errorHandler, int failureCount)
    {
        var old = SynchronizationContext.Current;
        try
        {
            SynchronizationContext.SetSynchronizationContext(null);
            logProvider.WriteAsync(logEntry, CancellationToken.None).GetAwaiter().GetResult();

            TraceSource.TraceEvent(TraceEventType.Information, 0,
                "[{0:s}] - [" + nameof(SynchronousLogProcessor) + "] - Successfully processed log entry {1} from log provider {2}.",
                DateTime.UtcNow, logEntry.UniqueId, logProvider);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(old);
        }
    }
}
