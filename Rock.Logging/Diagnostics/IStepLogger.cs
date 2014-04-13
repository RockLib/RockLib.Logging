using System;

namespace Rock.Logging.Diagnostics
{
    public interface IStepLogger : IDisposable
    {
        void AddStep(IStep step);
    }
}