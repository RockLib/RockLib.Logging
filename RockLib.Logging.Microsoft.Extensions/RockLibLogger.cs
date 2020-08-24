using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RockLib.Logging
{
    /// <summary>
    /// An implementation of <see cref="Microsoft.Extensions.Logging.ILogger"/> that writes log
    /// entries using an instance of <see cref="ILogger"/>.
    /// </summary>
    public class RockLibLogger : Microsoft.Extensions.Logging.ILogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RockLibLogger"/> class.
        /// </summary>
        /// <param name="logger">
        /// The <see cref="ILogger"/> that ultimately records logs.
        /// </param>
        /// <param name="categoryName"></param>
        /// <param name="scopeProvider"></param>
        public RockLibLogger(ILogger logger, string categoryName, IExternalScopeProvider scopeProvider)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            CategoryName = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
            ScopeProvider = scopeProvider;
        }

        /// <summary>
        /// Gets the <see cref="ILogger"/> that ultimately records logs.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the category name of the logger.
        /// </summary>
        public string CategoryName { get; }

        /// <summary>
        /// Gets the external scope information source for the logger.
        /// </summary>
        public IExternalScopeProvider ScopeProvider { get; internal set; }

        /// <inheritdoc/>
        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            var convertedLogLevel = ConvertLogLevel(logLevel);
            if (!Logger.IsEnabled(convertedLogLevel))
                return;

            var logEntry = new LogEntry(formatter(state, exception), exception, convertedLogLevel);

            logEntry.ExtendedProperties["Microsoft.Extensions.Logging.EventId"] = eventId;

            if (state != null)
                logEntry.ExtendedProperties["Microsoft.Extensions.Logging.State"] = GetStateObject(state);

            logEntry.ExtendedProperties["Microsoft.Extensions.Logging.CategoryName"] = CategoryName;

            if (ScopeProvider is IExternalScopeProvider sp && GetScope(sp) is object[] scope)
                logEntry.ExtendedProperties["Microsoft.Extensions.Logging.Scope"] = scope;

            Logger.Log(logEntry);
        }

        /// <inheritdoc/>
        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            if (logLevel == Microsoft.Extensions.Logging.LogLevel.None)
                return false;

            var convertedLogLevel = ConvertLogLevel(logLevel);
            return Logger.IsEnabled(convertedLogLevel);
        }

        /// <inheritdoc/>
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
            scopeProvider.ForEachScope((scope, list) => list.Add(GetStateObject(scope)), scopes);
            return scopes.Count > 0 ? scopes.ToArray() : null;
        }
    }
}
