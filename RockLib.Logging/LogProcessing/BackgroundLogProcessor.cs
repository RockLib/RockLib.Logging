using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.Logging.LogProcessing;

/// <summary>
/// A log processor that processes and tracks logs on dedicated non-threadpool
/// background threads. On dispose, it 
/// s until all in-flight logs have
/// finished processing.
/// </summary>
public sealed class BackgroundLogProcessor : LogProcessor
{
    private readonly object _disposeLocker = new object();

    private readonly BlockingCollection<(ILogger, LogEntry)> _processingQueue = new BlockingCollection<(ILogger, LogEntry)>();
    private readonly Thread _processingThread;

    private readonly BlockingCollection<(Task, LogEntry, ILogProvider, CancellationTokenSource, int, IErrorHandler)> _trackingQueue = new BlockingCollection<(Task, LogEntry, ILogProvider, CancellationTokenSource, int, IErrorHandler)>();
    private readonly Thread _trackingThread;

    /// <summary>
    /// Initializes a new instances of the <see cref="BackgroundLogProcessor"/> class.
    /// </summary>
    public BackgroundLogProcessor()
    {
        _processingThread = new Thread(ProcessLogEntries) { IsBackground = true };
        _processingThread.Start();
        _trackingThread = new Thread(TrackWriteTasks) { IsBackground = true };
        _trackingThread.Start();
    }

    /// <summary>
    /// Processes the log entry on behalf of the logger.
    /// </summary>
    /// <param name="logger">
    /// The logger that the log entry is processed on behalf of. Its log
    /// providers and context providers define how the log entry is processed.
    /// </param>
    /// <param name="logEntry">The log entry to process.</param>
    public override void ProcessLogEntry(ILogger logger, LogEntry logEntry)
    {
        if (IsDisposed)
            return;

        try
        {
            _processingQueue.Add((logger, logEntry));
        }
        catch (InvalidOperationException)
        {
            return;
        }
    }

    private void ProcessLogEntries()
    {
        foreach (var (logger, logEntry) in _processingQueue.GetConsumingEnumerable())
            base.ProcessLogEntry(logger, logEntry);
    }

    /// <inheritdoc/>
    protected override void SendToLogProvider(ILogProvider logProvider, LogEntry logEntry, IErrorHandler errorHandler, int failureCount)
    {
        var source = new CancellationTokenSource();
        var task = logProvider.WriteAsync(logEntry, source.Token);
        _trackingQueue.Add((task, logEntry, logProvider, source, failureCount, errorHandler));
    }

    private void TrackWriteTasks()
    {
        foreach (var (task, logEntry, logProvider, source, failureCount, errorHandler) in _trackingQueue.GetConsumingEnumerable())
        {
            try
            {
                if (task.Wait(logProvider.Timeout))
                {
                    TraceSource.TraceEvent(TraceEventType.Information, 0,
                        "[{0:s}] - [" + nameof(BackgroundLogProcessor) + "] - Successfully processed log entry {1} from log provider {2}.",
                        DateTime.Now, logEntry.UniqueId, logProvider);
                }
                else
                {
                    source.Cancel();

                    HandleError(null, logProvider, logEntry, errorHandler, failureCount + 1,
                        "Log entry {0} from log provider {1} timed out after {2}.",
                        logEntry.UniqueId, logProvider, logProvider.Timeout);
                }
            }
            catch (AggregateException aggregateException)
            {
                var ex = aggregateException.InnerExceptions.Count == 1
                    ? aggregateException.InnerException
                    : aggregateException;

                HandleError(ex, logProvider, logEntry, errorHandler, failureCount + 1,
                    "Error while waiting for log entry {0} to be sent by log provider {1}.",
                    logEntry.UniqueId, logProvider);

                continue;
            }
            catch (Exception ex)
            {
                HandleError(ex, logProvider, logEntry, errorHandler, failureCount + 1,
                    "Error while waiting for log entry {0} to be sent by log provider {1}.",
                    logEntry.UniqueId, logProvider);

                continue;
            }
            finally
            {
                source.Dispose();
            }
        }
    }

    /// <inheritdoc/>
    ~BackgroundLogProcessor() => Dispose(false);

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        lock (_disposeLocker)
        {
            if (IsDisposed)
                return;

            base.Dispose(disposing);

            if (disposing)
                GC.SuppressFinalize(this);

            _processingQueue.CompleteAdding();
            _processingThread.Join();
            _processingQueue.Dispose();

            _trackingQueue.CompleteAdding();
            _trackingThread.Join();
            _trackingQueue.Dispose();
        }
    }
}
