using System;
using System.Collections.Concurrent;
using Rock.Defaults.Implementation;
using Rock.DependencyInjection;

namespace Rock.Logging
{
    /// <summary>
    /// A implementation of <see cref="ILoggerFactory"/> that creates logger instances
    /// configured so its IsLoggingEnabled is true, logs at the LoggingLevel specified
    /// by the value passed into the <c>SimpleLoggerFactory</c> constructor, has a single
    /// log provider of type <typeparamref name="TLogProvider"/>, uses the application info from
    /// <see cref="Default.ApplicationInfo"/>, uses a new <see cref="NullThrottlingRuleEvaluator"/>
    /// for the throttling rule evaluator, and has no context providers.
    /// </summary>
    public sealed class SimpleLoggerFactory<TLogProvider> : ILoggerFactory
        where TLogProvider : ILogProvider, new()
    {
        private const LogLevel _defaultLogLevel = LogLevel.Debug;
        private const IResolver _defaultSupplementaryContainer = null;

        private readonly ConcurrentDictionary<Type, IResolver> _containers = new ConcurrentDictionary<Type, IResolver>();

        private readonly LogLevel _logLevel;
        private readonly IResolver _supplementaryContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleLoggerFactory{TLogProvider}"/> class.
        /// </summary>
        public SimpleLoggerFactory()                                                                                                                // ReSharper disable RedundantArgumentDefaultValue
            : this(_defaultLogLevel, _defaultSupplementaryContainer)                                                                                // ReSharper restore RedundantArgumentDefaultValue
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleLoggerFactory{TLogProvider}"/> class.
        /// </summary>
        /// <param name="logLevel">
        /// The log level that loggers returned from this instance's <see cref="Get{TLogger}"/> method 
        /// will be configured to.
        /// </param>
        /// <param name="supplementaryContainer">
        /// <para>An optional supplementary container.</para><para>If a constructor of the type specified by the generic
        /// parameter of the <see cref="Get{TLogger}"/> method requires additional dependencies beyond 
        /// those provided by <see cref="SimpleLoggerFactory{TLogProvider}"/>, an implementation of
        /// <see cref="IResolver"/> that *can* resolve those dependencies should be specified here.</para>
        /// </param>
        public SimpleLoggerFactory(
            LogLevel logLevel = _defaultLogLevel,
            IResolver supplementaryContainer = _defaultSupplementaryContainer)
        {
            _logLevel = logLevel;
            _supplementaryContainer = supplementaryContainer;
        }

        /// <summary>
        /// Get an instance of <typeparamref name="TLogger"/> for the given category.
        /// </summary>
        /// <typeparam name="TLogger">The type of <see cref="ILogger"/> to return.</typeparam>
        /// <param name="categoryName">An optional category.</param>
        /// <returns>An instance of <typeparamref name="TLogger"/>.</returns>
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
                Default.ApplicationInfo);

            if (_supplementaryContainer != null)
            {
                container = container.MergeWith(_supplementaryContainer);
            }

            return container;
        }
    }
}