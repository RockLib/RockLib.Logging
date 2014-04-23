namespace Rock.Logging.Diagnostics
{
    public class DefaultStepLoggerFactory : IStepLoggerFactory
    {
        public IStepLogger CreateStepLogger(
            ILogger logger,
            LogLevel logLevel,
            string message,
            string callerMemberName,
            string callerFilePath,
            int callerLineNumber)
        {
            return new StepLogger(logger, logLevel, message, callerMemberName, callerFilePath, callerLineNumber);
        }
    }
}