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
    /// With the exception of the <see cref="Dispose"/> method, all public instance members
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

        private readonly BlockingCollection<(Task, LogEntry, ILogProvider, CancellationTokenSource)> _workItems = new BlockingCollection<(Task, LogEntry, ILogProvider, CancellationTokenSource)>();
        private readonly Lazy<Thread> _startedWorkerThread;

        private volatile bool _isDisposed;
        private volatile int _threadsInCriticalSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="level">The logging level of the logger.</param>
        /// <param name="providers">A collection of <see cref="ILogProvider"/> objects used by this logger.</param>
        /// <param name="isDisabled">A value indicating whether the logger is disabled.</param>
        public Logger(
            string name = DefaultName,
            LogLevel level = LogLevel.Warn,
            IReadOnlyCollection<ILogProvider> providers = null,
            bool isDisabled = false)
        {
            Name = name ?? DefaultName;
            Level = level;
            Providers = providers ?? DefaultProviders;
            IsDisabled = isDisabled;

            _startedWorkerThread = new Lazy<Thread>(() =>
            {
                var thread = new Thread(ProcessWorkItems) { IsBackground = true };
                thread.Start();
                return thread;
            });
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
            if (logEntry == null)
                throw new ArgumentNullException(nameof(logEntry));

            // Check _isDisposed immediately before entering the critical section.
            if (_isDisposed)
                throw new ObjectDisposedException(typeof(Logger).FullName, "Cannot log to a disposed Logger.");

            Interlocked.Increment(ref _threadsInCriticalSection);

            try
            {
                if (IsDisabled || logEntry.Level < Level)
                    return;

                EnsureWorkerThreadIsStarted();

                logEntry.ExtendedProperties["CallerInfo"] = $"{callerFilePath}:{callerMemberName}({callerLineNumber})";

                foreach (var logProvider in Providers)
                {
                    var source = new CancellationTokenSource();
                    var task = WriteToLogProvider(logProvider, logEntry, source.Token);
                    _workItems.Add((task, logEntry, logProvider, source));
                }
            }
            finally
            {
                Interlocked.Decrement(ref _threadsInCriticalSection);
            }
        }

        /// <summary>
        /// Shuts down the logger, blocking until all pending logs have been sent.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;
            lock (this)
            {
                if (_isDisposed)
                    return;
                _isDisposed = true;
                WaitForCriticalSection();
                _workItems.CompleteAdding();
                if (_startedWorkerThread.IsValueCreated)
                    _startedWorkerThread.Value.Join();
                foreach (var provider in Providers.OfType<IDisposable>())
                    provider.Dispose();
                _workItems.Dispose();
            }
        }

        private void WaitForCriticalSection()
        {
            do Thread.Sleep(1);
            while (_threadsInCriticalSection > 0);
        }

        private void EnsureWorkerThreadIsStarted()
        {
            if (_startedWorkerThread.IsValueCreated)
                return;

            var dummy = _startedWorkerThread.Value;
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

        private void ProcessWorkItems()
        {
            foreach (var (task, logEntry, logProvider, source) in _workItems.GetConsumingEnumerable())
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
    }
}
