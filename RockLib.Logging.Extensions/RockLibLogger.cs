using System;

namespace RockLib.Logging.Extensions
{
    /// <summary>
    /// Wrap RockLib's Logger in a Microsoft.Extensions.Logging's interface <see cref="Microsoft.Extensions.Logging.ILogger"/>.
    /// </summary>
    internal class RockLibLogger : Microsoft.Extensions.Logging.ILogger
    {
        private readonly ILogger _logger;
        private readonly string _categoryName;

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

            var logEntry = new LogEntry(convertLogLevel, formatter(state, exception), exception);
            logEntry.ExtendedProperties["Microsoft.Extensions.Logging.EventId"] = eventId;
            logEntry.ExtendedProperties["Microsoft.Extensions.Logging.State"] = state;
            logEntry.ExtendedProperties["Microsoft.Extensions.Logging.CategoryName"] = _categoryName;

            _logger.Log(logEntry);
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

        /// <summary>
        /// Convert loglevel to RockLib variant.
        /// </summary>
        /// <param name="logLevel">level to be converted.</param>
        /// <returns></returns>
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
            // TODO: Figure out what to do with scope
            throw new NotImplementedException();
        }
    }
}
