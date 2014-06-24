using System.Runtime.CompilerServices;

namespace Rock.Logging
{
    public static class AddCallerInfoExtension
    {
        public static LogEntry AddCallerInfo(
            this LogEntry logEntry,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logEntry.ExtendedProperties.Add("CallerInfo", string.Format("{0}:{1}({2})", callerFilePath, callerMemberName, callerLineNumber));
            return logEntry;
        }
    }
}