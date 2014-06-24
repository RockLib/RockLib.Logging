using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Rock.DependencyInjection;
using Rock.Logging.Configuration;
using Rock.Logging.Defaults.Implementation;

namespace Rock.Logging
{
    public class LoggerFactory : ILoggerFactory
    {
        public static ILogger GetInstance(string category = null)
        {
            return Default.LoggerFactory.Get<Logger>(category);
        }

        public static TLogger GetInstance<TLogger>(string category = null)
            where TLogger : ILogger
        {
            return Default.LoggerFactory.Get<TLogger>(category);
        }

        private readonly IResolver _supplimentaryContainer;
        private readonly ConcurrentDictionary<string, IResolver> _resolvers = new ConcurrentDictionary<string, IResolver>();

        private readonly ILoggerFactoryConfiguration _config;
        private readonly IApplicationInfo _applicationInfo;
        private readonly Lazy<string> _firstCategory;

        public LoggerFactory(
            ILoggerFactoryConfiguration config = null,
            IApplicationInfo applicationInfo = null,
            IResolver supplimentaryContainer = null)
        {
            _config = config ?? Default.LoggerFactoryConfiguration;
            _applicationInfo = applicationInfo ?? Rock.Defaults.Implementation.Default.ApplicationInfo;
            _supplimentaryContainer = supplimentaryContainer;
            _firstCategory = new Lazy<string>(() => GetFirstCategory(_config));
        }

        public TLogger Get<TLogger>(string category = null)
            where TLogger : ILogger
        {
            var resolver =
                _resolvers.GetOrAdd(
                    category ?? _firstCategory.Value,
                    c =>
                    {
                        var logProviders = CreateLogProviders(c, _config, _supplimentaryContainer).ToList();
                        var auditLogProvider = CreateAuditLogProvider(_config, _supplimentaryContainer);
                        var throttlingRuleEvaluator = CreateThrottlingRuleEvaluator(c, _config);
                        var contextProviders = CreateContextProviders(c, _config).ToList();

                        var autoContainer = new AutoContainer(c, _applicationInfo, throttlingRuleEvaluator, auditLogProvider, logProviders, contextProviders);

                        if (_supplimentaryContainer == null)
                        {
                            return autoContainer;
                        }

                        return _supplimentaryContainer.MergeWith(autoContainer);
                    });

            return resolver.Get<TLogger>();
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
            ILoggerFactoryConfiguration config,
            IResolver providedContainer)
        {
            if (config.AuditLogProvider == null)
            {
                return null;
            }

            return CreateLogProvider(providedContainer, config, config.AuditLogProvider);
        }

        private static IEnumerable<ILogProvider> CreateLogProviders(
            string category,
            ILoggerFactoryConfiguration config,
            IResolver providedContainer)
        {
            return
                from logProviderConfig in config.Categories[category ?? ""].Providers
                select CreateLogProvider(providedContainer, config, logProviderConfig);
        }

        private static ILogProvider CreateLogProvider(
            IResolver providedContainer,
            ILoggerFactoryConfiguration loggerFactoryConfiguration,
            ILogProviderConfiguration logProviderConfiguration)
        {
            var formatterFactory = GetLogFormatterFactory(loggerFactoryConfiguration, logProviderConfiguration);
            var autoContainer = new AutoContainer(formatterFactory);
            var container = providedContainer == null ? autoContainer : providedContainer.MergeWith(autoContainer);

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

            throw new LogConfigurationException("Unable to determine formatter template for the provider " + logProviderConfiguration.ProviderType.Name);
        }

        private static IEnumerable<IContextProvider> CreateContextProviders(string category, ILoggerFactoryConfiguration config)
        {
            return Default.ContextProviders; // TODO: append context providers from configuration.
        }

        private static string GetFirstCategory(ILoggerFactoryConfiguration config)
        {
            var firstCategory = config.Categories.FirstOrDefault();

            if (firstCategory == null)
            {
                throw new LogConfigurationException("There are no categories specified in configuration.");
            }

            return firstCategory.Name;
        }
    }
}