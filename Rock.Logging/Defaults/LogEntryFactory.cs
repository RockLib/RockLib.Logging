namespace Rock.Logging.Defaults
{
    public class LogEntryFactory : ILogEntryFactory
    {
        public ILogEntry CreateLogEntry()
        {
            return new LogEntry();
        }
    }
}