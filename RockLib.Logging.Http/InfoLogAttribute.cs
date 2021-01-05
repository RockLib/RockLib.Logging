namespace RockLib.Logging.Http
{
    public class InfoLogAttribute : LoggingActionFilter
    {
        public InfoLogAttribute(string messageFormat = DefaultMessageFormat, string loggerName = Logger.DefaultName)
            : base(messageFormat, loggerName, LogLevel.Info)
        {
        }
    }
}
