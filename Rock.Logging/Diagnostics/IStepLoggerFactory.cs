using System.Runtime.CompilerServices;

namespace Rock.Logging.Diagnostics
{
    public interface IStepLoggerFactory
    {
        IStepLogger CreateStepLogger(
            ILogger logger,
            LogLevel logLevel,
            string message,
            string callerMemberName,
            string callerFilePath,
            int callerLineNumber);
    }
}