using System;
using System.Diagnostics;

namespace Rock.Logging.Diagnostics
{
    public class StopwatchLogger : IStopwatchLogger
    {
        private readonly IStepLogger _stepLogger;
        private readonly Stopwatch _stopwatch;

        public StopwatchLogger(IStepLogger stepLogger)
        {
            _stepLogger = stepLogger;
            _stopwatch = new Stopwatch();
        }

        public IStopwatchLogger Start()
        {
            _stopwatch.Start();
            return this;
        }

        public IStopwatchLogger Stop()
        {
            _stopwatch.Stop();
            return this;
        }

        public IStopwatchLogger Reset()
        {
            _stopwatch.Reset();
            return this;
        }

        /// <summary>
        /// Log the elapsed time of the stopwatch. The stopwatch will be stopped while logging is taking place, then
        /// started again once logging is complete. If <paramref name="andRestartStopwatch"/> is true, then the
        /// stopwatch is reset before it is started again.
        /// </summary>
        /// <param name="label">A label to identify the elapsed time value.</param>
        /// <param name="andRestartStopwatch">Whether to reset the stopwatch before it is started back up.</param>
        /// <returns>This instance of <see cref="StopwatchLogger"/>.</returns>
        public IStopwatchLogger LogElapsed(string label, bool andRestartStopwatch)
        {
            _stopwatch.Stop();

            _stepLogger.AddStep(
                new LogValueStep<TimeSpan>(
                    _stopwatch.Elapsed,
                    string.IsNullOrWhiteSpace(label) ? "Elapsed" : label));

            if (andRestartStopwatch)
            {
                _stopwatch.Reset();
            }

            _stopwatch.Start();

            return this;
        }
    }
}