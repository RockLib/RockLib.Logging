using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Rock.DependencyInjection;

namespace Rock.Logging
{
    public static class LoggerFactory
    {
        private static readonly ConcurrentDictionary<Tuple<string, Type>, ILogger> _loggerCache = new ConcurrentDictionary<Tuple<string, Type>, ILogger>();

        private static readonly Lazy<ILoggerFactoryConfiguration> _defaultLoggerConfiguration; 
        private static Lazy<ILoggerFactoryConfiguration> _loggerConfiguration;

        static LoggerFactory()
        {
            _defaultLoggerConfiguration = new Lazy<ILoggerFactoryConfiguration>(() => null); // TODO: actually give it the default
            _loggerConfiguration = _defaultLoggerConfiguration;
        }

        public static ILoggerFactoryConfiguration LoggerConfiguration
        {
            get { return _loggerConfiguration.Value; }
            set
            {
                if (value == null)
                {
                    _loggerConfiguration = _defaultLoggerConfiguration;
                }
                else if (!CurrentLoggerConfigurationIsSameAs(value))
                {
                    _loggerConfiguration = new Lazy<ILoggerFactoryConfiguration>(() => value);
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
        /// <param name="container">An <see cref="IResolver"/> that retrieves objects. For use by a logger's <see cref="Logger.Init"/> method.</param>
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
            category = category ?? GetFirstCategory(config);
            config = config ?? LoggerConfiguration;

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
            var auditLogProvider = CreateAuditLogProvider(category, config);
            var logProviders = CreateLogProviders(category, config);
            var contextProviders = CreateContextProviders(category, config);

            container =
                container.MergeWith(
                    new AutoContainer(config, throttlingRuleEvaluator, auditLogProvider, logProviders, contextProviders));
            var logger = container.Get<TLogger>();
            
            return logger;
        }

        private static IThrottlingRuleEvaluator CreateThrottlingRuleEvaluator(string category, ILoggerFactoryConfiguration config)
        {
            var throttlingRule = config.Categories.Single(c => c.Name == category).ThrottlingRule;

            if (throttlingRule != null)
            {
                return new ThrottlingRuleEvaluator(throttlingRule);
            }

            return null;
        }

        private static ILogProvider CreateAuditLogProvider(string category, ILoggerFactoryConfiguration config)
        {
            // TODO: Implement
            return null;
        }

        private static IEnumerable<ILogProvider> CreateLogProviders(string category, ILoggerFactoryConfiguration config)
        {
            foreach (var logProviderConfiguration in config.Categories.Single(c => c.Name == category).Providers)
            {
                var template = GetLogProviderTemplate(config, logProviderConfiguration);
            }

            // TODO: Implement
            return null;
        }

        private static string GetLogProviderTemplate(ILoggerFactoryConfiguration config, ILogProviderConfiguration providerConfig)
        {
            if (string.IsNullOrEmpty(providerConfig.FormatterName))
            {
                return LogProvider.DefaultTemplate;
            }

            var formatter = config.Formatters.FirstOrDefault(f => f.Name == providerConfig.FormatterName);
            if (formatter != null)
            {
                return formatter.Template;
            }

            throw new /*LogConfiguration*/Exception("Unable to determine formatter template for the provider " + providerConfig.ProviderType.Name);
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

        private static bool CurrentLoggerConfigurationIsSameAs(ILoggerFactoryConfiguration value)
        {
            return _loggerConfiguration.IsValueCreated && ReferenceEquals(_loggerConfiguration.Value, value);
        }
    }
}