using System;

namespace Rock.Logging.Diagnostics
{
    public class Stopwatch : IStopwatch
    {
        private readonly IStepLogger _stepLogger;
        private readonly System.Diagnostics.Stopwatch _stopwatch;

        public Stopwatch(IStepLogger stepLogger)
        {
            _stepLogger = stepLogger;
            _stopwatch = new System.Diagnostics.Stopwatch();
        }

        public IStopwatch Start()
        {
            _stopwatch.Start();
            return this;
        }

        public IStopwatch Stop()
        {
            _stopwatch.Stop();
            return this;
        }

        public IStopwatch Reset()
        {
            _stopwatch.Reset();
            return this;
        }

        public TimeSpan Elapsed
        {
            get { return _stopwatch.Elapsed; }
        }

        public void AddStep(IStep step)
        {
            _stepLogger.AddStep(step);
        }
    }
}