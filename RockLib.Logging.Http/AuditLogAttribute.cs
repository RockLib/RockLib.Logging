namespace RockLib.Logging.Http
{
    public class AuditLogAttribute : LoggingActionFilter
    {
        public AuditLogAttribute(string messageFormat = DefaultMessageFormat, string loggerName = Logger.DefaultName)
            : base(messageFormat, loggerName, LogLevel.Audit)
        {
        }
    }
}
