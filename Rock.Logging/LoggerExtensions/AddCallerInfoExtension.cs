namespace Rock.Logging
{
    public static partial class LoggerExtensions
    {
        public static LogEntry AddCallerInfo(
            this LogEntry logEntry,
            string callerMemberName,
            string callerFilePath,
            int callerLineNumber)
        {
            logEntry.ExtendedProperties.Add("CallerInfo", string.Format("{0}:{1}({2})", callerFilePath, callerMemberName, callerLineNumber));
            return logEntry;
        }
    }
}