using System;

namespace Rock.Logging.Diagnostics
{
    public static class StepLoggingExtensions
    {
        public static readonly Func<ILogger, LogLevel, string, IStepLogger> DefaultStepLoggerFactory =
            (logger, level, message) => new StepLogger(logger, level, message);

        private static Func<ILogger, LogLevel, string, IStepLogger> _stepLoggerFactory = DefaultStepLoggerFactory;

        public static Func<ILogger, LogLevel, string, IStepLogger> StepLoggerFactory
        {
            get { return _stepLoggerFactory; }
            set { _stepLoggerFactory = value ?? DefaultStepLoggerFactory; }
        }

        public static IStepLogger RecordSteps(this ILogger logger, LogLevel logLevel, string message = null)
        {
            return !logger.IsEnabled(logLevel)
                ? NullStepLogger.Instance
                : StepLoggerFactory(logger, logLevel, message);
        }

        public static T LogValue<T>(this T value, IStepLogger stepLogger, string label = "Value")
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

        public static void LogMessage(this IStepLogger stepLogger, string message)
        {
            if (stepLogger != NullStepLogger.Instance)
            {
                stepLogger.AddStep(new MessageStep(message));
            }
        }

        public static void LogMessage(this IStepLogger stepLogger, Func<string> getMessage)
        {
            if (stepLogger != NullStepLogger.Instance)
            {
                stepLogger.AddStep(new MessageStep(getMessage()));
            }
        }

        public static IStopwatchLogger GetStopwatch(this IStepLogger stepLogger)
        {
            return
                stepLogger == NullStepLogger.Instance
                    ? NullStopwatchLogger.Instance
                    : new StopwatchLogger(stepLogger);
        }


        /// <summary>
        /// Log the elapsed time of the stopwatch. The stopwatch will be stopped while logging is taking place, then
        /// started again once logging is complete. If <paramref name="andRestartStopwatch"/> is true, then the
        /// stopwatch is reset before it is started again.
        /// </summary>
        /// <param name="stopwatchLogger">This instance of <see cref="StopwatchLogger"/>.</param>
        /// <param name="label">A label to identify the elapsed time value.</param>
        /// <param name="andRestartStopwatch">Whether to reset the stopwatch before it is started back up.</param>
        /// <returns>This instance of <see cref="StopwatchLogger"/>.</returns>
        public static IStopwatchLogger LogElapsed(this IStopwatchLogger stopwatchLogger, string label = null, bool andRestartStopwatch = false)
        {
            return stopwatchLogger.LogElapsed(label, andRestartStopwatch);
        }
    }
}
