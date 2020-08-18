using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RockLib.Logging
{
    public class RockLibLogger : Microsoft.Extensions.Logging.ILogger
    {
        public RockLibLogger(ILogger logger, string categoryName, IExternalScopeProvider scopeProvider)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            CategoryName = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
            ScopeProvider = scopeProvider;
        }

        public ILogger Logger { get; }

        public string CategoryName { get; }

        public IExternalScopeProvider ScopeProvider { get; internal set; }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
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

            if (ScopeProvider is IExternalScopeProvider sp && GetScope(sp) is object[] scope)
                logEntry.ExtendedProperties["Microsoft.Extensions.Logging.Scope"] = scope;

            Logger.Log(logEntry);
        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            if (logLevel == Microsoft.Extensions.Logging.LogLevel.None)
                return false;

            var convertedLogLevel = ConvertLogLevel(logLevel);
            return Logger.IsEnabled(convertedLogLevel);
        }

        public IDisposable BeginScope<TState>(TState state) =>
            ScopeProvider?.Push(state) ?? null;

        private static object GetStateObject(object state)
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

        private static object[] GetScope(IExternalScopeProvider scopeProvider)
        {
            var scopes = new List<object>();
            scopeProvider.ForEachScope((scope, list) => list.Add(scope), scopes);
            return scopes.Count > 0 ? scopes.ToArray() : null;
        }
    }
}
