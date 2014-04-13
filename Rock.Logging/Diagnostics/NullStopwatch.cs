using System;

namespace Rock.Logging.Diagnostics
{
    public class NullStopwatch : IStopwatch
    {
        public static readonly IStopwatch Instance = new NullStopwatch();

        private NullStopwatch()
        {
        }

        public IStopwatch Start()
        {
            return this;
        }

        public IStopwatch Stop()
        {
            return this;
        }

        public IStopwatch Reset()
        {
            return this;
        }

        public TimeSpan Elapsed
        {
            get { return TimeSpan.Zero; }
        }

        public void AddStep(IStep step)
        {
        }
    }
}