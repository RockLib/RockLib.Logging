using System;
using System.Collections.Generic;
using System.Linq;

namespace RockLib.Logging.AspNetCore
{
    internal class RockLibLogger : Microsoft.Extensions.Logging.ILogger
    {
        private readonly ILogger _logger;
        private readonly string _categoryName;
        private readonly Lazy<Stack<object>> _scope = new Lazy<Stack<object>>(() => new Stack<object>());

        public RockLibLogger(ILogger logger, string categoryName)
        {
            _logger = logger;
            _categoryName = categoryName;
        }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId,
            TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));
            
            var convertLogLevel = ConvertLogLevel(logLevel);
            if (!IsEnabled(convertLogLevel))
                return;

            var logEntry = new LogEntry(formatter(state, exception), exception, convertLogLevel);

            logEntry.ExtendedProperties["Microsoft.Extensions.Logging.EventId"] = eventId;
            logEntry.ExtendedProperties["Microsoft.Extensions.Logging.State"] = GetStateObject(state);
            logEntry.ExtendedProperties["Microsoft.Extensions.Logging.CategoryName"] = _categoryName;

            if (_scope.IsValueCreated && _scope.Value.Count > 0)
                logEntry.ExtendedProperties["Microsoft.Extensions.Logging.Scope"] = GetScope();

            _logger.Log(logEntry);
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

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            // We do not have a concept of logging level none, so just return.
            if (logLevel == Microsoft.Extensions.Logging.LogLevel.None)
                return false;

            var convertLogLevel = ConvertLogLevel(logLevel);
            return IsEnabled(convertLogLevel);
        }

        private bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        private static LogLevel ConvertLogLevel(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case Microsoft.Extensions.Logging.LogLevel.Trace:
                    return LogLevel.Debug;
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

        public IDisposable BeginScope<TState>(TState state)
        {
            _scope.Value.Push(GetStateObject(state));
            return new DisposeScope(_scope.Value);
        }

        public object[] GetScope() => _scope.Value.ToArray();

        private class DisposeScope : IDisposable
        {
            private readonly Stack<object> _scope;
            public DisposeScope(Stack<object> scope) => _scope = scope;
            public void Dispose() => _scope.Pop();
        }
    }
}
