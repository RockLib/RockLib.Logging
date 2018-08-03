using RockLib.Diagnostics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.Logging
{
    /// <summary>
    /// Defines an object used for logging.
    /// </summary>
    /// <remarks>
    /// This class is expensive to initialize and is intended to be a long-lived object.
    /// With the exception of the <see cref="Dispose()"/> method, all public instance members
    /// of this class are thread-safe.
    /// </remarks>
    public sealed class Logger : ILogger, IDisposable
    {
        /// <summary>
        /// The default logger name.
        /// </summary>
        public const string DefaultName = "default";

        /// <summary>
        /// The default collection of <see cref="ILogProvider"/> objects.
        /// </summary>
        public static readonly IReadOnlyCollection<ILogProvider> DefaultProviders = new ILogProvider[0];

        /// <summary>
        /// The name of the <see cref="TraceSource"/> used by this class for trace logging.
        /// </summary>
        public const string TraceSourceName = "rocklib.logging";

        private static readonly TraceSource _traceSource = Tracing.GetTraceSource(TraceSourceName);

        private readonly BlockingCollection<(LogEntry, string, string, int)> _processingQueue = new BlockingCollection<(LogEntry, string, string, int)>();
        private readonly Thread _processingThread;

        private readonly BlockingCollection<(Task, LogEntry, ILogProvider, CancellationTokenSource)> _trackingQueue = new BlockingCollection<(Task, LogEntry, ILogProvider, CancellationTokenSource)>();
        private readonly Thread _trackingThread;

        private readonly bool _canProcessLogs;
        private volatile bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="level">The logging level of the logger.</param>
        /// <param name="providers">A collection of <see cref="ILogProvider"/> objects used by this logger.</param>
        /// <param name="isDisabled">A value indicating whether the logger is disabled.</param>
        public Logger(
            string name = DefaultName,
            LogLevel level = LogLevel.NotSet,
            IReadOnlyCollection<ILogProvider> providers = null,
            bool isDisabled = false)
        {
            if (!Enum.IsDefined(typeof(LogLevel), level))
                throw new ArgumentException($"Log level is not defined: {level}.", nameof(level));

            Name = name ?? DefaultName;
            Level = level;
            Providers = providers ?? DefaultProviders;
            IsDisabled = isDisabled;

            _canProcessLogs = !IsDisabled && Providers.Count > 0;

            if (_canProcessLogs)
            {
                _processingThread = new Thread(ProcessLogEntries) { IsBackground = true };
                _processingThread.Start();
                _trackingThread = new Thread(TrackWriteTasks) { IsBackground = true };
                _trackingThread.Start();
            }
            else
            {
                _processingQueue.CompleteAdding();
                _processingQueue.Dispose();
                _trackingQueue.CompleteAdding();
                _trackingQueue.Dispose();
            }

            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => Dispose();
            AppDomain.CurrentDomain.DomainUnload += (sender, eventArgs) => Dispose();
        }

        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the logging level of the logger.
        /// </summary>
        /// <remarks>
        /// Log entries with a level lower than the value of this property are
        /// not logged by this logger.
        /// </remarks>
        public LogLevel Level { get; }

        /// <summary>
        /// Gets the collection of <see cref="ILogProvider"/> objects used by this logger.
        /// </summary>
        public IReadOnlyCollection<ILogProvider> Providers { get; }

        /// <summary>
        /// Gets a value indicating whether the logger is disabled.
        /// </summary>
        public bool IsDisabled { get; }

        /// <summary>
        /// Logs the specified log entry.
        /// </summary>
        /// <param name="logEntry">The log entry to log.</param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public void Log(
            LogEntry logEntry,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (logEntry == null) throw new ArgumentNullException(nameof(logEntry));
            if (_isDisposed) throw new ObjectDisposedException("Cannot log to a disposed Logger.");

            if (_canProcessLogs)
            {
                try
                {
                    _processingQueue.Add((logEntry, callerMemberName, callerFilePath, callerLineNumber));
                }
                catch (InvalidOperationException ex)
                {
                    throw new ObjectDisposedException("Cannot log to a disposed Logger.", ex);
                }
            }
        }

        private void ProcessLogEntries()
        {
            foreach (var (logEntry, callerFilePath, callerMemberName, callerLineNumber) in _processingQueue.GetConsumingEnumerable())
            {
                if (logEntry.Level < Level)
                    continue;

                logEntry.ExtendedProperties["CallerInfo"] = $"{callerFilePath}:{callerMemberName}({callerLineNumber})";
                // TODO: Invoke any context providers

                foreach (var logProvider in Providers)
                {
                    var source = new CancellationTokenSource();
                    var task = WriteToLogProvider(logProvider, logEntry, source.Token);
                    _trackingQueue.Add((task, logEntry, logProvider, source));
                }
            }
        }

        private async Task WriteToLogProvider(ILogProvider logProvider, LogEntry logEntry, CancellationToken cancellationToken)
        {
            if (logEntry.Level < logProvider.Level)
                return;

            try
            {
                await logProvider.WriteAsync(logEntry, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _traceSource.TraceEvent(TraceEventType.Warning, ex.HResult,
                    "[{0}] - [RockLib.Logging.Logger] - Error while sending log entry {1}:{2}{3}",
                    DateTime.Now, logEntry.UniqueId, Environment.NewLine, ex);

                // TODO: execute retry policy
            }
        }

        private void TrackWriteTasks()
        {
            foreach (var (task, logEntry, logProvider, source) in _trackingQueue.GetConsumingEnumerable())
            {
                if (task.Wait(logProvider.Timeout))
                {
                    _traceSource.TraceEvent(TraceEventType.Information, 0,
                        "[{0}] - [RockLib.Logging.Logger] - Successfully processed log entry {1} from log provider {2}.",
                        DateTime.Now, logEntry.UniqueId, logProvider);
                }
                else
                {
                    source.Cancel();

                    _traceSource.TraceEvent(TraceEventType.Warning, 0,
                        "[{0}] - [RockLib.Logging.Logger] - Log entry {1} from log provider {2} timed out after {3}.",
                        DateTime.Now, logEntry.UniqueId, logProvider, logProvider.Timeout);

                    // TODO: execute retry policy
                }
            }
        }

        /// <summary>
        /// Disposes the logger, if not already disposed.
        /// </summary>
        ~Logger()
        {
            Dispose(false);
        }

        /// <summary>
        /// Shuts down the logger, blocking until all pending logs have been sent.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            lock (this)
            {
                if (_isDisposed)
                    return;

                _isDisposed = true;

                if (disposing)
                    GC.SuppressFinalize(this);

                if (_canProcessLogs)
                {
                    _processingQueue.CompleteAdding();
                    _processingThread.Join();
                    _processingQueue.Dispose();

                    _trackingQueue.CompleteAdding();
                    _trackingThread.Join();
                    _trackingQueue.Dispose();
                }

                foreach (var provider in Providers.OfType<IDisposable>())
                    provider.Dispose();
            }
        }
    }
}
