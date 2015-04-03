using System.Runtime.CompilerServices;

namespace Rock.Logging
{
    public static class AddCallerInfoExtension
    {
        public static ILogEntry AddCallerInfo(
            this ILogEntry logEntry,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logEntry.ExtendedProperties.Add("CallerInfo", string.Format("{0}:{1}({2})", callerFilePath, callerMemberName, callerLineNumber));
            return logEntry;
        }
    }
}