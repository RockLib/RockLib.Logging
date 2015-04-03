namespace Rock.Logging
{
    public abstract class ContextProvider<TLogEntry> : IContextProvider
        where TLogEntry : ILogEntry
    {
        public void AddContextData(ILogEntry logEntry)
        {
            if (logEntry is TLogEntry)
            {
                var tLogEntry = (TLogEntry)logEntry;
                AddContextData(tLogEntry);
            }
        }

        protected abstract void AddContextData(TLogEntry logEntry);
    }
}