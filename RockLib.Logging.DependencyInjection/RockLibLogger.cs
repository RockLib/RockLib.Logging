using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RockLib.Logging.DependencyInjection
{
    /// <summary>
    /// An implementation of <see cref="Microsoft.Extensions.Logging.ILogger"/> that writes log entries to an
    /// instance of <see cref="ILogger"/>.
    /// </summary>
    public class RockLibLogger : Microsoft.Extensions.Logging.ILogger
    {
        private readonly Lazy<ConcurrentStack<object>> _scope = new Lazy<ConcurrentStack<object>>(() => new ConcurrentStack<object>());

        /// <summary>
        /// Initializes a new instance of the <see cref="RockLibLogger"/> class.
        /// </summary>
        /// <param name="logger">The instance of <see cref="ILogger"/> that log entries are written to.</param>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        public RockLibLogger(ILogger logger, string categoryName)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            CategoryName = categoryName;
        }

        /// <summary>
        /// Gets the instance of <see cref="ILogger"/> that log entries are written to.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the category name for messages produced by the logger.
        /// </summary>
        public string CategoryName { get; }

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">Function to create a string message of the state and exception.</param>
        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId,
            TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            var convertedLogLevel = ConvertLogLevel(logLevel);
            if (!Logger.IsEnabled(convertedLogLevel))
                return;

            var logEntry = new LogEntry(formatter(state, exception), exception, convertedLogLevel);

            logEntry.ExtendedProperties["Microsoft.Extensions.Logging.EventId"] = eventId;
            logEntry.ExtendedProperties["Microsoft.Extensions.Logging.State"] = GetStateObject(state);
            logEntry.ExtendedProperties["Microsoft.Extensions.Logging.CategoryName"] = CategoryName;

            if (_scope.IsValueCreated && _scope.Value.Count > 0)
                logEntry.ExtendedProperties["Microsoft.Extensions.Logging.Scope"] = GetScope();

            Logger.Log(logEntry);
        }

        /// <summary>
        /// Checks if the given logLevel is enabled.
        /// </summary>
        /// <param name="logLevel">level to be checked.</param>
        /// <returns>true if enabled.</returns>
        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            // We do not have a concept of logging level none, so just return.
            if (logLevel == Microsoft.Extensions.Logging.LogLevel.None)
                return false;

            var convertedLogLevel = ConvertLogLevel(logLevel);
            return Logger.IsEnabled(convertedLogLevel);
        }

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            _scope.Value.Push(GetStateObject(state));
            return new DisposeScope(_scope.Value);
        }

        private object GetStateObject(object state)
        {
            if (typeof(IEnumerable<KeyValuePair<string, object>>).IsAssignableFrom(state.GetType())
                && !typeof(IDictionary<string, object>).IsAssignableFrom(state.GetType()))
            {
                var items = (IEnumerable<KeyValuePair<string, object>>)state;
                if (items.GroupBy(x => x.Key).All(g => g.Count() == 1))
                {
                    return items.ToDictionary(x => x.Key, x => x.Value);
                }
            }

            return state;
        }

        private static LogLevel ConvertLogLevel(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case Microsoft.Extensions.Logging.LogLevel.Trace:
                case Microsoft.Extensions.Logging.LogLevel.Debug:
                    return LogLevel.Debug;
                case Microsoft.Extensions.Logging.LogLevel.Information:
                    return LogLevel.Info;
                case Microsoft.Extensions.Logging.LogLevel.Warning:
                    return LogLevel.Warn;
                case Microsoft.Extensions.Logging.LogLevel.Error:
                    return LogLevel.Error;
                case Microsoft.Extensions.Logging.LogLevel.Critical:
                    return LogLevel.Fatal;
                default:
                    return LogLevel.NotSet;
            }
        }

        private object[] GetScope() => _scope.Value.ToArray();

        private class DisposeScope : IDisposable
        {
            private readonly ConcurrentStack<object> _scope;
            public DisposeScope(ConcurrentStack<object> scope) => _scope = scope;
            public void Dispose() => _scope.TryPop(out _);
        }
    }
}
