using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Rock.DependencyInjection;
using Rock.Logging.Configuration;

namespace Rock.Logging
{
    public static class LoggerFactory
    {
        private static readonly ConcurrentDictionary<Tuple<string, Type>, ILogger> _loggerCache = new ConcurrentDictionary<Tuple<string, Type>, ILogger>();

        private static readonly Lazy<IConfigProvider> _defaultConfigProvider;
        private static Lazy<IConfigProvider> _configProvider;

        static LoggerFactory()
        {
            _defaultConfigProvider = new Lazy<IConfigProvider>(() => new FileConfigProvider());
            _configProvider = _defaultConfigProvider;
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

        /// <summary>
        /// Gets an instance of <see cref="ILogger"/> by category and custom configuration.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="config">The config.</param>
        /// <returns>Returns a <see cref="ILogger"/>.</returns>
        /// <example>
        /// <code>
        ///  // setup logger configuration through the API
        ///  LoggerConfiguration config = new LoggerConfiguration();
        ///  CategoryId cat = new CategoryId{Name = "File"};
        ///  
        /// // add a file provider so log messages get logged to a file
        /// Provider prov = new Provider();
        /// prov.ProviderType = Type.GetType("Rock.Logging.Provider.FileLogProvider, Rock.Framework");
        /// 
        /// // setup the file provider property
        /// Mapper file = new Mapper { Property = "File", Value = "Log.txt" };
        /// 
        /// // add the settings to the config
        /// prov.Mappers.Add(file);
        /// cat.Providers.Add(prov);
        /// config.Categories.Add("File", cat);
        /// 
        /// ILogger logger = LoggerFactory.GetInstance("File", config);
        /// </code>
        /// </example>        
        /// <exception cref="T:System.ArgumentNullException">CategoryId or config is null</exception>
        /// <exception cref="T:Rock.Logging.LogConfigurationException">There was a problem reading the configuration.</exception>
        public static ILogger GetInstance(
            string category = null,
            ILoggerFactoryConfiguration config = null)
        {
            return GetInstance<Logger>(category, config);
        }

        /// <summary>
        /// Used to load and initialize <see cref="ILogger"/> with a custom implemented type and
        /// custom API configuration by category.
        /// </summary>
        /// <typeparam name="TLogger">Custom logger that implements <see cref="ILogger"/>.</typeparam>
        /// <param name="category">The category.</param>
        /// <param name="config">The config.</param>
        /// <param name="container">An <see cref="IResolver"/> that retrieves objects. To be used in order to resolve dependencies for the specified logger type.</param>
        /// <returns>Returns a <see cref="ILogger"/>.</returns>
        /// <remarks>
        /// Developers can create their own logger specific to their applications that
        /// provide additional functionality and/or that help out with logging.
        /// <para>
        /// This method is used to allow developers to load their own types of logger.  This
        /// method loads the first category in configuration.
        /// </para>
        /// </remarks>
        /// <example>
        /// 	<code>
        /// // setup logger configuration through the API
        /// LoggerConfiguration config = new LoggerConfiguration();
        /// // create the default category named "Default"
        /// CategoryId cat = new CategoryId{Name = "Default"};
        /// // add a file provider so log messages get logged to a file
        /// Provider prov = new Provider();
        /// prov.ProviderType = Type.GetType("Rock.Logging.Provider.FileLogProvider, Rock.Framework");
        /// // setup the file provider property
        /// Mapper file = new Mapper { Property = "File", Value = "Log.txt" };
        /// // add the settings to the config
        /// prov.Mappers.Add(file);
        /// cat.Providers.Add(prov);
        /// config.Categories.Add("Default", cat);
        /// MyCustomLogger logger = LoggerFactory.GetInstance{MyCustomLogger}("Default", config);
        /// </code>
        /// </example>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static TLogger GetInstance<TLogger>(
            string category = null,
            ILoggerFactoryConfiguration config = null,
            IResolver container = null)
            where TLogger : ILogger
        {
            config = config ?? ConfigProvider.GetConfiguration();
            category = category ?? GetFirstCategory(config);

            return (TLogger)_loggerCache.GetOrAdd(
                Tuple.Create(category, typeof(TLogger)),
                _ => CreateAndInitializeLogger<TLogger>(category, config, container));
        }

        private static ILogger CreateAndInitializeLogger<TLogger>(
            string category,
            ILoggerFactoryConfiguration config,
            IResolver container)
            where TLogger : ILogger
        {
            var throttlingRuleEvaluator = CreateThrottlingRuleEvaluator(category, config);
            var auditLogProvider = CreateAuditLogProvider(category, config, container);
            var logProviders = CreateLogProviders(category, config, container).ToList();
            var contextProviders = CreateContextProviders(category, config).ToList();

            var autoContainer = new AutoContainer(config, throttlingRuleEvaluator, auditLogProvider, logProviders, contextProviders);
            var mergedContainer = container == null ? autoContainer : container.MergeWith(autoContainer);

            var logger = mergedContainer.Get<TLogger>();
            return logger;
        }

        private static IThrottlingRuleEvaluator CreateThrottlingRuleEvaluator(string category, ILoggerFactoryConfiguration config)
        {
            var throttlingRule = config.Categories[category].ThrottlingRule;

            return
                throttlingRule != null
                    ? new ThrottlingRuleEvaluator(throttlingRule)
                    : null;
        }

        private static ILogProvider CreateAuditLogProvider(
            string category,
            ILoggerFactoryConfiguration config,
            IResolver originalContainer)
        {
            // TODO: Implement
            return null;
        }

        private static IEnumerable<ILogProvider> CreateLogProviders(
            string category,
            ILoggerFactoryConfiguration loggerFactoryConfiguration,
            IResolver originalContainer)
        {
            return
                from logProviderConfiguration in loggerFactoryConfiguration.Categories[category ?? ""].Providers
                let formatterFactory = GetLogFormatterFactory(loggerFactoryConfiguration, logProviderConfiguration)
                let autoContainer = new AutoContainer(formatterFactory)
                let container = originalContainer == null ? autoContainer : originalContainer.MergeWith(autoContainer)
                select CreateAndInitializeLogProvider(container, logProviderConfiguration);
        }

        private static ILogProvider CreateAndInitializeLogProvider(
            IResolver container,
            ILogProviderConfiguration logProviderConfiguration)
        {
            var logProvider = (ILogProvider)container.Get(logProviderConfiguration.ProviderType);
            
            foreach (var mapper in logProviderConfiguration.Mappers)
            {
                mapper.SetValue(logProvider);
            }

            return logProvider;
        }

        private static ILogFormatterFactory GetLogFormatterFactory(
            ILoggerFactoryConfiguration loggerFactoryConfiguration,
            ILogProviderConfiguration logProviderConfiguration)
        {
            if (string.IsNullOrEmpty(logProviderConfiguration.FormatterName))
            {
                return new LogFormatterFactory(LogFormatterConfiguration.Default);
            }

            if (loggerFactoryConfiguration.Formatters.Contains(logProviderConfiguration.FormatterName))
            {
                var formatterConfig = loggerFactoryConfiguration.Formatters[logProviderConfiguration.FormatterName];
                return new LogFormatterFactory(formatterConfig);
            }

            throw new /*LogConfiguration*/Exception("Unable to determine formatter template for the provider " + logProviderConfiguration.ProviderType.Name);
        }

        private static IEnumerable<IContextProvider> CreateContextProviders(string category, ILoggerFactoryConfiguration config)
        {
            // TODO: Implement
            yield break;
        }

        private static string GetFirstCategory(ILoggerFactoryConfiguration config)
        {
            var firstCategory = config.Categories.FirstOrDefault();

            if (firstCategory == null)
            {
                // TODO: Implement LogConfigurationException
                throw new /*LogConfiguration*/Exception("There are no categories specified in configuration.");
            }

            return firstCategory.Name;
        }

        private static bool CurrentConfigProviderIsSameAs(IConfigProvider value)
        {
            return _configProvider.IsValueCreated && ReferenceEquals(_configProvider.Value, value);
        }
    }
}