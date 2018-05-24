using Rock.Immutable;

namespace Rock.Logging.Diagnostics
{
    public static class DefaultStepLoggerFactory
    {
        private static readonly Semimutable<IStepLoggerFactory> _stepLoggerFactory = new Semimutable<IStepLoggerFactory>(GetDefault);

        public static IStepLoggerFactory Current
        {
            get { return _stepLoggerFactory.Value; }
        }

        public static void SetCurrent(IStepLoggerFactory stepLoggerFactory)
        {
            _stepLoggerFactory.Value = stepLoggerFactory;
        }

        private static IStepLoggerFactory GetDefault()
        {
            return new StepLoggerFactory();
        }
    }
}