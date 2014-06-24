using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Rock.Logging
{
    public class Logger : ILogger
    {
        private readonly ILoggerConfiguration _configuration;
        private readonly IEnumerable<ILogProvider> _logProviders;
        private readonly ILogProvider _auditLogProvider;
        private readonly IThrottlingRuleEvaluator _throttlingRuleEvaluator;
        private readonly IApplicationInfo _applicationInfo;
        private readonly IEnumerable<IContextProvider> _contextProviders;

        public Logger(
            ILoggerConfiguration configuration,
            IEnumerable<ILogProvider> logProviders,
            IApplicationInfo applicationInfo,
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

            if (applicationInfo == null)
            {
                throw new ArgumentNullException("applicationInfo");
            }

            // Be sure to fully realize lists so we get fast enumeration during logging.
            logProviders = logProviders.ToList();

            if (!logProviders.Any())
            {
                throw new ArgumentException("Must provide at least one log provider.", "logProviders");
            }

            _configuration = configuration;
            _logProviders = logProviders;
            _applicationInfo = applicationInfo;
            _auditLogProvider = auditLogProvider; // NOTE: this can be null, and is expected.
            _throttlingRuleEvaluator = throttlingRuleEvaluator ?? NullThrottlingRuleEvaluator.Instance;
            _contextProviders = (contextProviders ?? Enumerable.Empty<IContextProvider>()).ToList();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return
                _configuration.IsLoggingEnabled
                && logLevel >= _configuration.LoggingLevel
                && logLevel != LogLevel.None;
        }

        public async Task Log(
            LogEntry logEntry,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (logEntry.LogLevel != LogLevel.Audit
                && (!IsEnabled(logEntry.LogLevel)
                    || (_throttlingRuleEvaluator != null && !_throttlingRuleEvaluator.ShouldLog(logEntry))))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(logEntry.ApplicationId))
            {
                logEntry.ApplicationId = _applicationInfo.ApplicationId;
            }

            if (logEntry.SearchKey == null)
            {
                logEntry.SearchKey = Guid.NewGuid().ToString();
            }

            logEntry.AddCallerInfo(callerMemberName, callerFilePath, callerLineNumber);

            AddContextData(logEntry);
            foreach (var contextProvider in _contextProviders)
            {
                contextProvider.AddContextData(logEntry);
            }

            Task writeTask;

            if (logEntry.LogLevel == LogLevel.Audit && _auditLogProvider != null)
            {
                writeTask = _auditLogProvider.Write(logEntry);
            }
            else
            {
                writeTask = Task.WhenAll(_logProviders.Select(logProvider => logProvider.Write(logEntry)));
            }

            await writeTask.ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // TODO: send log entry and exception(s) to system event log.
                }
            });
        }

        protected virtual void AddContextData(LogEntry entry)
        {
        }

        void IExceptionHandler.HandleException(Exception ex)
        {
            this.Error(ex);
        }
    }
}