using System;
using System.Collections.Concurrent;
using Rock.Defaults.Implementation;
using Rock.DependencyInjection;

namespace Rock.Logging.Configuration
{
    /// <summary>
    /// A implementation of <see cref="ILoggerFactory"/> that creates logger instances
    /// configured so its IsLoggingEnabled is true, logs at the LoggingLevel specified
    /// by the value passed into the <c>SimpleLoggerFactory</c> constructor, has a single
    /// log provider of type <typeparamref name="TLogProvider"/>, uses the application info from
    /// <see cref="Default.ApplicationInfo"/>, uses the throttling rule evaluator from 
    /// <see cref="NullThrottlingRuleEvaluator.Instance"/>, and has no context providers.
    /// </summary>
    public sealed class SimpleLoggerFactory<TLogProvider> : ILoggerFactory
        where TLogProvider : ILogProvider, new()
    {
        private const LogLevel _defaultLogLevel = LogLevel.Debug;
        private const IResolver _defaultSupplementaryContainer = null;

        private readonly ConcurrentDictionary<Type, IResolver> _containers = new ConcurrentDictionary<Type, IResolver>();

        private readonly LogLevel _logLevel;
        private readonly IResolver _supplementaryContainer;

        public SimpleLoggerFactory()                                                                                                                // ReSharper disable RedundantArgumentDefaultValue
            : this(_defaultLogLevel, _defaultSupplementaryContainer)                                                                                // ReSharper restore RedundantArgumentDefaultValue
        {
        }

        public SimpleLoggerFactory(
            LogLevel logLevel = _defaultLogLevel,
            IResolver supplementaryContainer = _defaultSupplementaryContainer)
        {
            _logLevel = logLevel;
            _supplementaryContainer = supplementaryContainer;
        }

        public TLogger Get<TLogger>(string categoryName = null)
            where TLogger : ILogger
        {
            var container = _containers.GetOrAdd(typeof(TLogger), t => GetContainer());
            return container.Get<TLogger>();
        }

        private IResolver GetContainer()
        {
            var container = new AutoContainer(
                new LoggerConfiguration { IsLoggingEnabled = true, LoggingLevel = _logLevel },
                new ILogProvider[] { new TLogProvider() },
                Default.ApplicationInfo,
                //(ILogProvider)null, // No specified audit log provider
                NullThrottlingRuleEvaluator.Instance,
                new IContextProvider[0]);

            if (_supplementaryContainer != null)
            {
                container = container.MergeWith(_supplementaryContainer);
            }

            return container;
        }
    }
}