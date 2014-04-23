using System;
using Rock.Logging.Configuration;
using Rock.Logging.Diagnostics;

namespace Rock.Logging
{
    public static class Default
    {
        private static bool _locked;

        private static readonly Lazy<ILogEntryFactory> _defaultLogEntryFactory;
        private static Lazy<ILogEntryFactory> _logEntryFactory;

        private static readonly Lazy<IConfigProvider> _defaultConfigProvider;
        private static Lazy<IConfigProvider> _configProvider;

        private static readonly Lazy<IStepLoggerFactory> _defaultStepLoggerFactory;
        private static Lazy<IStepLoggerFactory> _stepLoggerFactory;

        private static IContextProvider[] _contextProviders;

        static Default()
        {
            _defaultLogEntryFactory = new Lazy<ILogEntryFactory>(() => LockThenReturn(new LogEntryFactory()));
            _logEntryFactory = _defaultLogEntryFactory;

            _defaultConfigProvider = new Lazy<IConfigProvider>(() => LockThenReturn(new FileConfigProvider()));
            _configProvider = _defaultConfigProvider;

            _defaultStepLoggerFactory = new Lazy<IStepLoggerFactory>(() => LockThenReturn(new DefaultStepLoggerFactory()));
            _stepLoggerFactory = _defaultStepLoggerFactory;
        }

        public static ILogEntryFactory LogEntryFactory
        {
            get { return _logEntryFactory.Value; }
            set
            {
                if (_locked)
                {
                    return;
                }

                if (value == null)
                {
                    _logEntryFactory = _defaultLogEntryFactory;
                }
                else if (!CurrentLogEntryFactoryIsSameAs(value))
                {
                    _logEntryFactory = new Lazy<ILogEntryFactory>(() => LockThenReturn(value));
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
                if (_locked)
                {
                    return;
                }

                if (value == null)
                {
                    _stepLoggerFactory = _defaultStepLoggerFactory;
                }
                else if (!CurrentStepLoggerFactoryIsSameAs(value))
                {
                    _stepLoggerFactory = new Lazy<IStepLoggerFactory>(() => LockThenReturn(value));
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
                if (_locked)
                {
                    return;
                }

                if (value == null)
                {
                    _configProvider = _defaultConfigProvider;
                }
                else if (!CurrentConfigProviderIsSameAs(value))
                {
                    _configProvider = new Lazy<IConfigProvider>(() => LockThenReturn(value));
                }
            }
        }

        private static bool CurrentConfigProviderIsSameAs(IConfigProvider value)
        {
            return _configProvider.IsValueCreated && ReferenceEquals(_configProvider.Value, value);
        }

        public static IContextProvider[] ContextProviders
        {
            get 
            {
                _locked = true;
                return _contextProviders;
            }
            set
            {
                if (_locked)
                {
                    return;
                }

                _contextProviders = value ?? new IContextProvider[0];
            }
        }

        /// <summary>
        /// You probably don't want to do this. But if, for some reason, you did, this would allow you
        /// to set the various values of the <see cref="Default"/> class, even after a value had been
        /// previously retrieved.
        /// </summary>
        public static void Unlock()
        {
            _locked = false;
        }

        private static T LockThenReturn<T>(T value)
        {
            _locked = true;
            return value;
        }
    }
}
