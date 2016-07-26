using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Rock.BackgroundErrorLogging;
using AppId=Rock.ApplicationId;

namespace Rock.Logging
{
    public class Logger : ILogger
    {
        private readonly ILoggerConfiguration _configuration;
        private readonly IEnumerable<ILogProvider> _logProviders;

        private readonly string _applicationId;
        
        private readonly ILogProvider _auditLogProvider;
        private readonly IThrottlingRuleEvaluator _throttlingRuleEvaluator;
        private readonly IEnumerable<IContextProvider> _contextProviders;

        public Logger(
            ILoggerConfiguration configuration,
            IEnumerable<ILogProvider> logProviders,
            IApplicationIdProvider applicationIdProvider = null,
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
            logProviders = logProviders.ToList().AsReadOnly();

            if (!logProviders.Any())
            {
                throw new ArgumentException("Must provide at least one log provider.", "logProviders");
            }

            _configuration = configuration;
            _logProviders = logProviders;

            _applicationId =
                applicationIdProvider != null
                    ? applicationIdProvider.GetApplicationId()
                    : AppId.Current;

            _auditLogProvider = auditLogProvider; // NOTE: this can be null, and is expected.
            _throttlingRuleEvaluator = throttlingRuleEvaluator ?? new NullThrottlingRuleEvaluator();
            _contextProviders = (contextProviders ?? Enumerable.Empty<IContextProvider>()).ToList().AsReadOnly();
        }

        public ILoggerConfiguration Configuration
        {
            get { return _configuration; }
        }

        public string ApplicationId
        {
            get { return _applicationId; }
        }

        public IEnumerable<ILogProvider> LogProviders
        {
            get { return _logProviders; }
        }

        public ILogProvider AuditLogProvider
        {
            get { return _auditLogProvider; }
        }

        public IThrottlingRuleEvaluator ThrottlingRuleEvaluator
        {
            get { return _throttlingRuleEvaluator; }
        }

        public IEnumerable<IContextProvider> ContextProviders
        {
            get { return _contextProviders; }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return
                _configuration.IsLoggingEnabled
                && logLevel >= _configuration.LoggingLevel
                && logLevel != LogLevel.NotSet;
        }

        public async Task LogAsync(
            ILogEntry logEntry,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (logEntry.Level != LogLevel.Audit
                && (!IsEnabled(logEntry.Level)
                    || (_throttlingRuleEvaluator != null && !_throttlingRuleEvaluator.ShouldLog(logEntry))))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(logEntry.ApplicationId))
            {
                logEntry.ApplicationId = _applicationId;
            }

            if (logEntry.UniqueId == null)
            {
                logEntry.UniqueId = Guid.NewGuid().ToString();
            }

            // ReSharper disable ExplicitCallerInfoArgument
            logEntry.AddCallerInfo(callerMemberName, callerFilePath, callerLineNumber);
            // ReSharper restore ExplicitCallerInfoArgument

            foreach (var contextProvider in _contextProviders)
            {
                contextProvider.AddContextData(logEntry);
            }

            OnPreLog(logEntry);

            Task writeTask;

            if (logEntry.Level == LogLevel.Audit && _auditLogProvider != null)
            {
                try
                {
                    writeTask = _auditLogProvider.WriteAsync(logEntry);
                }
                catch (Exception ex)
                {
                    BackgroundErrorLogger.Log(ex, "Error when writing to audit log provider.", "Rock.Logging");
                    writeTask = Task.FromResult(0);
                }
            }
            else
            {
                writeTask =
                    Task.WhenAll(
                        _logProviders
                            .Where(x => logEntry.Level >= x.LoggingLevel)
                            .Select(logProvider =>
                            {
                                try
                                {
                                    return logProvider.WriteAsync(logEntry);
                                }
                                catch (Exception ex)
                                {
                                    BackgroundErrorLogger.Log(ex, "Error when writing to log provider.", "Rock.Logging");
                                    return Task.FromResult(0);
                                }
                            }));
            }

            try
            {
                await writeTask.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BackgroundErrorLogger.Log(ex, "Error when writing to log provider(s)", "Rock.Logging");
                // TODO: Send the log entry to a retry mechanism?
                // TODO ALSO: The error handling here sucks. Do something about it.
            }
        }

        protected virtual void OnPreLog(ILogEntry logEntry)
        {
        }
    }
}