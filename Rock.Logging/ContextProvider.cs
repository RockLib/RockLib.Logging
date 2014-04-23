namespace Rock.Logging
{
    public abstract class ContextProvider<TLogEntry> : IContextProvider
        where TLogEntry : LogEntry
    {
        public void AddContextData(LogEntry logEntry)
        {
            var tLogEntry = logEntry as TLogEntry;
            if (tLogEntry != null)
            {
                AddContextData(tLogEntry);
            }
        }

        protected abstract void AddContextData(TLogEntry logEntry);
    }
}