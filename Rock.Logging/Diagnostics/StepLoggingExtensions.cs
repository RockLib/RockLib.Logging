using System;

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

        public static string LogValue(this string value, IStepLogger stepLogger, string identifier = "Value")
        {
            if (stepLogger != NullStepLogger.Instance)
            {
                stepLogger.AddStep(new LogValueStep(value, identifier));
            }

            return value;
        }

        public static void LogMessage(this IStepLogger stepLogger, string message)
        {
            if (stepLogger != NullStepLogger.Instance)
            {
                stepLogger.AddStep(new MessageStep(message));
            }
        }

        public static IStopwatch GetStopwatch(this IStepLogger stepLogger)
        {
            return
                stepLogger == NullStepLogger.Instance
                    ? NullStopwatch.Instance
                    : new Stopwatch(stepLogger);
        }

        public static IStopwatch LogElapsed(this IStopwatch stopwatch, string identifier = null)
        {
            if (stopwatch != NullStopwatch.Instance)
            {
                var elapsed = stopwatch.Elapsed;
                stopwatch.AddStep(
                    new LogValueStep<TimeSpan>(
                        elapsed,
                        string.IsNullOrWhiteSpace(identifier) ? "Elapsed" : identifier));
            }

            return stopwatch;
        }

        public static IStopwatch LogElapsedAndRestart(IStopwatch stopwatch, string identifier = null)
        {
            if (stopwatch != NullStopwatch.Instance)
            {
                return stopwatch.Stop().LogElapsed(identifier).Start();
            }

            return stopwatch;
        }
    }
}
