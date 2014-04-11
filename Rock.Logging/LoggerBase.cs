using System.Collections.Generic;

namespace Rock.Logging
{
    /// <summary>
    /// Legacy support.
    /// </summary>
    public abstract class LoggerBase : Logger
    {
        protected virtual void OnPreLogSync(LogEntry entry)
        {
            AddContextData(entry);
        }
    }
}