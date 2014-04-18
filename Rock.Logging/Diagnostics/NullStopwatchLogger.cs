namespace Rock.Logging.Diagnostics
{
    public class NullStopwatchLogger : IStopwatchLogger
    {
        public static readonly IStopwatchLogger Instance = new NullStopwatchLogger();

        private NullStopwatchLogger()
        {
        }

        public IStopwatchLogger Start()
        {
            return this;
        }

        public IStopwatchLogger Stop()
        {
            return this;
        }

        public IStopwatchLogger Reset()
        {
            return this;
        }

        public IStopwatchLogger RecordElapsed(string label, bool andResetStopwatch)
        {
            return this;
        }
    }
}