using System;
using System.Diagnostics;

namespace Rock.Logging.Diagnostics
{
    public static class StepLoggingExtensions
    {
        public static IStepLogger RecordSteps(this ILogger logger, LogLevel logLevel, string message = null)
        {
            return !logger.IsEnabled(logLevel)
                ? NullStepLogger.Instance
                : new StepLogger(logger, logLevel, message);
        }

        public static T LogValue<T>(this T value, IStepLogger stepLogger, string identifier = "Value")
            where T : struct 
        {
            if (stepLogger != NullStepLogger.Instance)
            {
                stepLogger.AddStep(new LogValueStep<T>(value, identifier));
            }
            
            return value;
        }

        public static void LogElapsedTime(this IStepLogger stepLogger, Stopwatch stopwatch, string identifier = null)
        {
            if (stepLogger != NullStepLogger.Instance)
            {
                var elapsed = stopwatch.Elapsed;
                stepLogger.AddStep(
                    new LogValueStep<TimeSpan>(
                        elapsed,
                        string.IsNullOrWhiteSpace(identifier) ? "Elapsed" : identifier));
            }
        }

        public static void LogMessage(this IStepLogger stepLogger, string message)
        {
            if (stepLogger != NullStepLogger.Instance)
            {
                stepLogger.AddStep(new MessageStep(message));
            }
        }
    }
}
