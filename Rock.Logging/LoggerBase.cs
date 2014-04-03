using System.Collections.Generic;
using System.Linq;
using Rock.Framework;

namespace Rock.Logging
{
    /// <summary>
    /// Legacy support. Includes protected virtual method, `AddContextData`.
    /// </summary>
    public abstract class LoggerBase : Logger
    {
        private readonly IEnumerable<IContextProvider> _contextProviders;

        protected LoggerBase()
        {
            _contextProviders = new LoggerBaseContextProvider(this).Concat(base.GetContextProviders()).ToList();
        }

        protected LoggerBase(ILoggerConfiguration configuration, IEnumerable<IContextProvider> contextProviders)
            : base(configuration, contextProviders)
        {
            _contextProviders = new LoggerBaseContextProvider(this).Concat(base.GetContextProviders()).ToList();
        }

        protected virtual void AddContextData(LogEntry entry)
        {
        }

        protected override IEnumerable<IContextProvider> GetContextProviders()
        {
            return _contextProviders;
        }

        private class LoggerBaseContextProvider : IContextProvider
        {
            private readonly LoggerBase _loggerBase;

            public LoggerBaseContextProvider(LoggerBase loggerBase)
            {
                _loggerBase = loggerBase;
            }

            public void AddContextData(LogEntry logEntry)
            {
                _loggerBase.AddContextData(logEntry);
            }            
        }
    }
}