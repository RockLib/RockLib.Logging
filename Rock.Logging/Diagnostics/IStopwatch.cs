using System;

namespace Rock.Logging.Diagnostics
{
    public interface IStopwatch
    {
        IStopwatch Start();
        IStopwatch Stop();
        IStopwatch Reset();
        TimeSpan Elapsed { get; }
        void AddStep(IStep step);
    }
}