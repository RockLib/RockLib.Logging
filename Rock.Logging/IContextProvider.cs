namespace Rock.Logging
{
    public interface IContextProvider
    {
        void AddContextData(ILogEntry logEntry);
    }
}