using System;

namespace Rock.Logging.Diagnostics
{
    public interface IStopwatchLogger
    {
        IStopwatchLogger Start();
        IStopwatchLogger Stop();
        IStopwatchLogger Reset();

        /// <summary>
        /// Log the elapsed time of the stopwatch. The stopwatch will be stopped while logging is taking place, then
        /// started again once logging is complete. If <paramref name="andRestartStopwatch"/> is true, then the
        /// stopwatch is reset before it is started again.
        /// </summary>
        /// <param name="label">A label to identify the elapsed time value.</param>
        /// <param name="andRestartStopwatch">Whether to reset the stopwatch before it is started back up.</param>
        /// <returns>This instance of <see cref="StopwatchLogger"/>.</returns>
        IStopwatchLogger RecordElapsed(string label = null, bool andRestartStopwatch = false);
    }
}