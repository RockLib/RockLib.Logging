namespace Rock.Logging
{
    public interface IContextProvider
    {
        void AddContextData(LogEntry logEntry);
    }
}