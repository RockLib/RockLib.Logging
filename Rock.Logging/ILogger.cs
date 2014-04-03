namespace Rock.Logging
{
    public interface ILogger
    {
        bool IsEnabled(LogLevel logLevel);
        void Log(LogEntry logEntry);
    }
}