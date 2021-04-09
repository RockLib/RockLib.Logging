using RockLib.Configuration.ObjectFactory;
using RockLib.Logging.LogProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RockLib.Logging
{
    /// <summary>
    /// Defines an object used for logging.
    /// </summary>
    /// <remarks>
    /// This class is expensive to initialize and is intended to be a long-lived object.
    /// All public instance members of this class are thread-safe.
    /// </remarks>
    public sealed class Logger : ILogger
    {
        /// <summary>
        /// Defines types of processing modes used by logger objects.
        /// </summary>
        public enum ProcessingMode
        {
            /// <summary>
            /// The logger should process and track logs on dedicated non-threadpool
            /// background threads.
            /// </summary>
            Background,

            /// <summary>
            /// The logger should process logs on the same thread as the caller.
            /// </summary>
            Synchronous,

            /// <summary>
            /// The logger should process logs asynchronously, but without any
            /// task tracking.
            /// </summary>
            FireAndForget
        }

        /// <summary>
        /// The default logger name.
        /// </summary>
        public const string DefaultName = "";

        private static readonly Lazy<BackgroundLogProcessor> _backgroundLogProcessor = new Lazy<BackgroundLogProcessor>(GetBackgroundLogProcessor);

        private static readonly IReadOnlyCollection<ILogProvider> _emptyLogProviders = new ILogProvider[0];
        private static readonly IReadOnlyCollection<IContextProvider> _emptyContextProviders = new IContextProvider[0];

        /// <summary>
        /// The name of the <see cref="TraceSource"/> used by this class for trace logging.
        /// </summary>
        public const string TraceSourceName = "rocklib.logging";

        private readonly bool _canProcessLogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="level">The logging level of the logger.</param>
        /// <param name="logProviders">A collection of <see cref="ILogProvider"/> objects used by this logger.</param>
        /// <param name="isDisabled">A value indicating whether the logger is disabled.</param>
        /// <param name="processingMode">A value that indicates how the logger will process logs.</param>
        /// <param name="contextProviders">
        /// A collection of <see cref="IContextProvider"/> objects that customize outgoing log entries.
        /// </param>
        public Logger(
            string name = DefaultName,
            LogLevel level = LogLevel.NotSet,
            [AlternateName("providers")] IReadOnlyCollection<ILogProvider> logProviders = null,
            bool isDisabled = false,
            ProcessingMode processingMode = ProcessingMode.Background,
            IReadOnlyCollection<IContextProvider> contextProviders = null)
            : this(GetLogProcessor(processingMode), name, level, logProviders, isDisabled, contextProviders)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="logProcessor">The object responsible for processing logs.</param>
        /// <param name="name">The name of the logger.</param>
        /// <param name="level">The logging level of the logger.</param>
        /// <param name="logProviders">A collection of <see cref="ILogProvider"/> objects used by this logger.</param>
        /// <param name="isDisabled">A value indicating whether the logger is disabled.</param>
        /// <param name="contextProviders">
        /// A collection of <see cref="IContextProvider"/> objects that customize outgoing log entries.
        /// </param>
        public Logger(
            ILogProcessor logProcessor,
            string name = DefaultName,
            LogLevel level = LogLevel.NotSet,
            [AlternateName("providers")] IReadOnlyCollection<ILogProvider> logProviders = null,
            bool isDisabled = false,
            IReadOnlyCollection<IContextProvider> contextProviders = null)
        {
            if (!Enum.IsDefined(typeof(LogLevel), level))
                throw new ArgumentException($"Log level is not defined: {level}.", nameof(level));

            Name = name ?? DefaultName;
            Level = level;
            LogProviders = logProviders ?? _emptyLogProviders;
            IsDisabled = isDisabled;
            ContextProviders = contextProviders ?? _emptyContextProviders;
            LogProcessor = logProcessor ?? throw new ArgumentNullException(nameof(logProcessor));

            _canProcessLogs = !IsDisabled && LogProviders.Count > 0;
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
        public IReadOnlyCollection<ILogProvider> LogProviders { get; }

        /// <summary>
        /// Gets a value indicating whether the logger is disabled.
        /// </summary>
        public bool IsDisabled { get; }

        /// <summary>
        /// The collection of <see cref="IContextProvider"/> objects that customize outgoing log entries.
        /// </summary>
        public IReadOnlyCollection<IContextProvider> ContextProviders { get; }

        /// <summary>
        /// Gets the object responsible for processing logs.
        /// </summary>
        public ILogProcessor LogProcessor { get; }

        /// <summary>
        /// Gets or sets the object that handles errors that occur during log processing.
        /// </summary>
        public IErrorHandler ErrorHandler { get; set; }

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

            if (LogProcessor.IsDisposed || !_canProcessLogs || logEntry.Level < Level)
                return;

            logEntry.CallerInfo = $"{callerFilePath}:{callerMemberName}({callerLineNumber})";

            LogProcessor.ProcessLogEntry(this, logEntry);
        }

        /// <summary>
        /// Does nothing. Exists for backwards compatibility.
        /// </summary>
        public void Dispose() { }

        private static ILogProcessor GetLogProcessor(ProcessingMode processingMode)
        {
            switch (processingMode)
            {
                case ProcessingMode.Background:
                    return _backgroundLogProcessor.Value;
                case ProcessingMode.Synchronous:
                    return new SynchronousLogProcessor();
                case ProcessingMode.FireAndForget:
                    return new FireAndForgetLogProcessor();
                default:
                    throw new ArgumentException($"Processing mode is not defined: {processingMode}.", nameof(processingMode));
            }
        }

        private static BackgroundLogProcessor GetBackgroundLogProcessor()
        {
            var processor = new BackgroundLogProcessor();

            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => processor.Dispose();
            AppDomain.CurrentDomain.DomainUnload += (sender, eventArgs) => processor.Dispose();

            return processor;
        }
    }
}
