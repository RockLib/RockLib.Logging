namespace Rock.Logging
{
    public interface ILogFormatter
    {
        string Format(LogEntry entry);
    }
}