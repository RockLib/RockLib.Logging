using System;
using Rock.Logging.Configuration;
using Rock.Logging.Diagnostics;

namespace Rock.Logging
{
    public static class Default
    {
        private static readonly Lazy<ILogEntryFactory> _defaultLogEntryFactory;
        private static Lazy<ILogEntryFactory> _logEntryFactory;

        private static readonly Lazy<IConfigProvider> _defaultConfigProvider;
        private static Lazy<IConfigProvider> _configProvider;

        private static readonly Lazy<IStepLoggerFactory> _defaultStepLoggerFactory;
        private static Lazy<IStepLoggerFactory> _stepLoggerFactory;

        private static IContextProvider[] _contextProviders;

        static Default()
        {
            _defaultLogEntryFactory = new Lazy<ILogEntryFactory>(() => new LogEntryFactory());
            _logEntryFactory = _defaultLogEntryFactory;

            _defaultConfigProvider = new Lazy<IConfigProvider>(() => new FileConfigProvider());
            _configProvider = _defaultConfigProvider;

            _defaultStepLoggerFactory = new Lazy<IStepLoggerFactory>(() => new DefaultStepLoggerFactory());
            _stepLoggerFactory = _defaultStepLoggerFactory;
        }

        public static ILogEntryFactory LogEntryFactory
        {
            get { return _logEntryFactory.Value; }
            set
            {
                if (value == null)
                {
                    _logEntryFactory = _defaultLogEntryFactory;
                }
                else if (!CurrentLogEntryFactoryIsSameAs(value))
                {
                    _logEntryFactory = new Lazy<ILogEntryFactory>(() => value);
                }
            }
        }

        private static bool CurrentLogEntryFactoryIsSameAs(ILogEntryFactory value)
        {
            return _logEntryFactory.IsValueCreated && ReferenceEquals(_logEntryFactory.Value, value);
        }

        public static IStepLoggerFactory StepLoggerFactory
        {
            get { return _stepLoggerFactory.Value; }
            set
            {
                if (value == null)
                {
                    _stepLoggerFactory = _defaultStepLoggerFactory;
                }
                else if (!CurrentStepLoggerFactoryIsSameAs(value))
                {
                    _stepLoggerFactory = new Lazy<IStepLoggerFactory>(() => value);
                }
            }
        }

        private static bool CurrentStepLoggerFactoryIsSameAs(IStepLoggerFactory value)
        {
            return _stepLoggerFactory.IsValueCreated && ReferenceEquals(_stepLoggerFactory.Value, value);
        }

        public static IConfigProvider ConfigProvider
        {
            get { return _configProvider.Value; }
            set
            {
                if (value == null)
                {
                    _configProvider = _defaultConfigProvider;
                }
                else if (!CurrentConfigProviderIsSameAs(value))
                {
                    _configProvider = new Lazy<IConfigProvider>(() => value);
                }
            }
        }

        private static bool CurrentConfigProviderIsSameAs(IConfigProvider value)
        {
            return _configProvider.IsValueCreated && ReferenceEquals(_configProvider.Value, value);
        }

        public static IContextProvider[] ContextProviders
        {
            get { return _contextProviders; }
            set { _contextProviders = value ?? new IContextProvider[0]; }
        }
    }
}
