using System;
using Rock.Defaults;
using Rock.Logging.Diagnostics;

namespace Rock.Logging.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IStepLoggerFactory> _stepLoggerFactory = new DefaultHelper<IStepLoggerFactory>(() => new DefaultStepLoggerFactory());

        public static IStepLoggerFactory StepLoggerFactory
        {
            get { return _stepLoggerFactory.Current; }
        }

        public static void SetStepLoggerFactory(Func<IStepLoggerFactory> getStepLoggerFactoryInstance)
        {
            _stepLoggerFactory.SetCurrent(getStepLoggerFactoryInstance);
        }
    }
}