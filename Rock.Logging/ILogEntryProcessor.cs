namespace Rock.Logging
{
    /// <summary>
    /// Represents the mechanism for processing a log entry.
    /// </summary>
    public interface ILogEntryProcessor
    {
        void Process(LogEntry logEntry);
    }
}