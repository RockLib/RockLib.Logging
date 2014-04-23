namespace Rock.Logging
{
    public class LogEntryFactory : ILogEntryFactory
    {
        public LogEntry CreateLogEntry()
        {
            return new LogEntry();
        }
    }
}