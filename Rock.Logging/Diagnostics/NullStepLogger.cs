namespace Rock.Logging.Diagnostics
{
    public class NullStepLogger : IStepLogger
    {
        public static readonly IStepLogger Instance = new NullStepLogger();

        private NullStepLogger()
        {
        }

        public void AddStep(IStep step)
        {
        }

        public void Dispose()
        {
        }
    }
}