using System.Collections.Generic;

namespace Rock.Logging
{
    /// <summary>
    /// Legacy support. Allows for custom context data to be added via inheritence.
    /// </summary>
    public abstract class LoggerBase : Logger, IContextProvider
    {
        protected LoggerBase()
        {
        }

        protected LoggerBase(ILoggerConfiguration configuration, IEnumerable<IContextProvider> contextProviders, ILogEntryProcessor logEntryProcessor)
            : base(configuration, contextProviders, logEntryProcessor)
        {
        }

        void IContextProvider.AddContextData(LogEntry entry)
        {
            AddContextData(entry);
            OnPreLogSync(entry);
        }

        protected virtual void AddContextData(LogEntry entry)
        {
        }

        protected virtual void OnPreLogSync(LogEntry entry)
        {
        }
    }
}