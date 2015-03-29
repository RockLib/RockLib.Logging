using System;
using System.Runtime.CompilerServices;

namespace Rock.Logging.Diagnostics
{
    public static class StepLoggingExtensions
    {
        public static IStepLogger CreateStepLogger(
            this ILogger logger,
            LogLevel logLevel = LogLevel.Debug,
            string message = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return !logger.IsEnabled(logLevel)
                ? NullStepLogger.Instance
                : DefaultStepLoggerFactory.Current.CreateStepLogger(logger, logLevel, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static T AddValueTo<T>(this T value, IStepLogger stepLogger, string label = "Value")
            where T : struct 
        {
            if (stepLogger != NullStepLogger.Instance)
            {
                stepLogger.AddStep(new LogValueStep<T>(value, label));
            }
            
            return value;
        }

        public static string LogValue(this string value, IStepLogger stepLogger, string label = "Value")
        {
            if (stepLogger != NullStepLogger.Instance)
            {
                stepLogger.AddStep(new LogValueStep(value, label));
            }

            return value;
        }

        public static T LogValue<T, TValue>(this T t, IStepLogger stepLogger, Func<T, TValue> getValue, string label = "Value")
            where TValue : struct
        {
            if (stepLogger != NullStepLogger.Instance)
            {
                stepLogger.AddStep(new LogValueStep<TValue>(getValue(t), label));
            }

            return t;
        }

        public static T LogValue<T>(this T t, IStepLogger stepLogger, Func<T, string> getValue, string label = "Value")
        {
            if (stepLogger != NullStepLogger.Instance)
            {
                stepLogger.AddStep(new LogValueStep(getValue(t), label));
            }
            
            return t;
        }

        public static void AddMessage(this IStepLogger stepLogger, string message)
        {
            if (stepLogger != NullStepLogger.Instance)
            {
                stepLogger.AddStep(new MessageStep(message));
            }
        }

        public static IStopwatchLogger GetStopwatch(this IStepLogger stepLogger)
        {
            return
                stepLogger == NullStepLogger.Instance
                    ? NullStopwatchLogger.Instance
                    : new StopwatchLogger(stepLogger);
        }
    }
}
