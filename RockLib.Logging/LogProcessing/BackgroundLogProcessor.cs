using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.Logging.LogProcessing
{
    public sealed class BackgroundLogProcessor : LogProcessor
    {
        private readonly BlockingCollection<(ILogger, LogEntry, Action<ErrorEventArgs>)> _processingQueue = new BlockingCollection<(ILogger, LogEntry, Action<ErrorEventArgs>)>();
        private readonly Thread _processingThread;

        private readonly BlockingCollection<(Task, LogEntry, ILogProvider, CancellationTokenSource, int, Action<ErrorEventArgs>)> _trackingQueue = new BlockingCollection<(Task, LogEntry, ILogProvider, CancellationTokenSource, int, Action<ErrorEventArgs>)>();
        private readonly Thread _trackingThread;

        public BackgroundLogProcessor()
        {
            _processingThread = new Thread(ProcessLogEntries) { IsBackground = true };
            _processingThread.Start();
            _trackingThread = new Thread(TrackWriteTasks) { IsBackground = true };
            _trackingThread.Start();

            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => Dispose(true);
            AppDomain.CurrentDomain.DomainUnload += (sender, eventArgs) => Dispose(true);
        }

        public override void ProcessLogEntry(ILogger logger, LogEntry logEntry, Action<ErrorEventArgs> errorHandler)
        {
            try
            {
                _processingQueue.Add((logger, logEntry, errorHandler));
            }
            catch (InvalidOperationException ex)
            {
                throw new ObjectDisposedException("Cannot log to a disposed Logger.", ex);
            }
        }

        private void ProcessLogEntries()
        {
            foreach (var (logger, logEntry, errorHandler) in _processingQueue.GetConsumingEnumerable())
                ProcessLogEntry(logger, logEntry, errorHandler, 0);
        }

        protected override void WriteToLogProvider(ILogProvider logProvider, LogEntry logEntry, Action<ErrorEventArgs> errorHandler, int failureCount)
        {
            var source = new CancellationTokenSource();
            var task = logProvider.WriteAsync(logEntry, CancellationToken.None);
            _trackingQueue.Add((task, logEntry, logProvider, source, failureCount, errorHandler));
        }

        private void TrackWriteTasks()
        {
            foreach (var (task, logEntry, logProvider, source, failureCount, errorHandler) in _trackingQueue.GetConsumingEnumerable())
            {
                if (task.Wait(logProvider.Timeout))
                {
                    TraceSource.TraceEvent(TraceEventType.Information, 0,
                        "[{0}] - [RockLib.Logging.Logger] - Successfully processed log entry {1} from log provider {2}.",
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
        }

        ~BackgroundLogProcessor() => Dispose(false);

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            lock (this)
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
}
