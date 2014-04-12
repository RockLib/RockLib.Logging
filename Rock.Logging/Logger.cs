using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rock.Logging
{
    public class Logger : ILogger
    {
        private readonly ILoggerConfiguration _configuration;
        private readonly IThrottlingRuleEvaluator _throttlingRuleEvaluator;
        private readonly ILogProvider _auditLogProvider;
        private readonly IEnumerable<ILogProvider> _logProviders;
        private readonly IEnumerable<IContextProvider> _contextProviders;

        public Logger(
            ILoggerConfiguration configuration,
            IEnumerable<ILogProvider> logProviders,
            ILogProvider auditLogProvider = null,
            IThrottlingRuleEvaluator throttlingRuleEvaluator = null,
            IEnumerable<IContextProvider> contextProviders = null)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (logProviders == null)
            {
                throw new ArgumentNullException("logProviders");
            }

            // Be sure to fully realize lists so we get fast enumeration during logging.
            logProviders = logProviders.ToList();

            if (!logProviders.Any())
            {
                throw new ArgumentException("Must provide at least one log provider.", "logProviders");
            }

            _configuration = configuration;
            _logProviders = logProviders;
            _auditLogProvider = auditLogProvider ?? logProviders.First(); // TODO: This needs review from CORE. Is it appropriate to write audit logs to the first provider if no audit provider was supplied?
            _throttlingRuleEvaluator = throttlingRuleEvaluator ?? Null.ThrottlingRuleEvaluator;
            _contextProviders = (contextProviders ?? Enumerable.Empty<IContextProvider>()).ToList();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return // TODO: this needs review from CORE. Should we always process audit logs, even if logging as a whole is configured to be disabled? Or do audit logs only always process when logging is configured to be enabled?
                _configuration.IsLoggingEnabled
                && logLevel >= _configuration.LoggingLevel
                && logLevel != LogLevel.None;
        }

        public async Task Log(LogEntry logEntry)
        {
            if (!IsEnabled(logEntry.LogLevel)
                || (_throttlingRuleEvaluator != null && !_throttlingRuleEvaluator.ShouldLog(logEntry)))
            {
                return;
            }

            AddContextData(logEntry);
            foreach (var contextProvider in _contextProviders)
            {
                contextProvider.AddContextData(logEntry);
            }

            if (logEntry.LogLevel == LogLevel.Audit)
            {
                await _auditLogProvider.Write(logEntry);
            }
            else
            {
                await Task.WhenAll(_logProviders.Select(logProvider => logProvider.Write(logEntry)));
            }
        }

        protected virtual void AddContextData(LogEntry entry)
        {
        }
    }
}