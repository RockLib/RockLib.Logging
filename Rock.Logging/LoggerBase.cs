using System.Collections.Generic;

namespace Rock.Logging
{
    /// <summary>
    /// Legacy support.
    /// </summary>
    public abstract class LoggerBase : Logger
    {
        protected LoggerBase(
            ILoggerConfiguration configuration,
            IThrottlingRuleEvaluator throttlingRuleEvaluator,
            ILogProvider auditLogProvider,
            IEnumerable<ILogProvider> logProviders,
            IEnumerable<IContextProvider> contextProviders)
            : base(configuration, throttlingRuleEvaluator, auditLogProvider, logProviders, contextProviders)
        {
        }

        protected virtual void OnPreLogSync(LogEntry entry)
        {
            AddContextData(entry);
        }
    }
}