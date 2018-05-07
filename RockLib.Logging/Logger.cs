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
    public sealed class Logger : ILogger, IDisposable
    {
        public const string DefaultName = "default";
        public static readonly IReadOnlyCollection<ILogProvider> DefaultProviders = new ILogProvider[0];

        public const string TraceSourceName = "rocklib.logging";
        private static readonly TraceSource _traceSource = Tracing.GetTraceSource(TraceSourceName);

        private readonly BlockingCollection<(Task, LogEntry, ILogProvider, CancellationTokenSource)> _workItems = new BlockingCollection<(Task, LogEntry, ILogProvider, CancellationTokenSource)>();
        private readonly Lazy<Thread> _startedWorkerThread;

        private volatile bool _isDisposed;

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

        public string Name { get; }
        public LogLevel Level { get; }
        public IReadOnlyCollection<ILogProvider> Providers { get; }
        public bool IsDisabled { get; }

        public void Log(
            LogEntry logEntry,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (logEntry == null)
                throw new ArgumentNullException(nameof(logEntry));

            if (_isDisposed)
                throw new ObjectDisposedException(typeof(Logger).FullName, "Cannot log to a disposed Logger.");

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

        public void Dispose()
        {
            _isDisposed = true;
            _workItems.CompleteAdding();
            if (_startedWorkerThread.IsValueCreated)
                _startedWorkerThread.Value.Join();
            foreach (var provider in Providers.OfType<IDisposable>())
                provider.Dispose();
            _workItems.Dispose();
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
