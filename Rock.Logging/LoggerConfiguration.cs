namespace Rock.Logging
{
    public class LoggerConfiguration : ILoggerConfiguration
    {
        public bool IsLoggingEnabled { get; set; }
        public LogLevel LoggingLevel { get; set; }
    }
}