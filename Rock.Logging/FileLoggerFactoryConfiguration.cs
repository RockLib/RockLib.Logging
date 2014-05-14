using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Rock.Collections;
using Rock.Extensions;
using Rock.Logging.Configuration;

namespace Rock.Logging
{
    public class FileLoggerFactoryConfiguration : ILoggerFactoryConfiguration
    {
        private readonly KeyedCollection<string, LogFormatterConfiguration> _formatters = new FunctionalKeyedCollection<string, LogFormatterConfiguration>(c => c.Name);
        private readonly KeyedCollection<string, ThrottlingRuleConfiguration> _throttlingRules = new FunctionalKeyedCollection<string, ThrottlingRuleConfiguration>(c => c.Name);
        private readonly KeyedCollection<string, Category> _categories = new FunctionalKeyedCollection<string, Category>(c => c.Name);

        private readonly bool _isLoggingEnabled;
        private readonly LogLevel _loggingLevel;

        private readonly ILogProviderConfiguration _auditLogProvider;

        public FileLoggerFactoryConfiguration()
        {
            var settings = (LoggerSettingsSection)ConfigurationManager.GetSection("rock.logging");

            _isLoggingEnabled = settings.IsLoggingEnabled;
            _loggingLevel = settings.LoggingLevel.GetEnumValue<LogLevel>();

            if (settings.AuditLogProvider != null)
            {
                _auditLogProvider = CreateLogProviderConfiguration(settings.AuditLogProvider);
            }

            LoadFormatters(settings);
            LoadThrottlingRules(settings);
            LoadCategories(settings);
        }

        public bool IsLoggingEnabled
        {
            get { return _isLoggingEnabled; }
        }

        public LogLevel LoggingLevel
        {
            get { return _loggingLevel; }
        }

        public ILogProviderConfiguration AuditLogProvider
        {
            get { return _auditLogProvider; }
        }

        public IKeyedEnumerable<string, ICategory> Categories
        {
            get { return _categories; }
        }

        public IKeyedEnumerable<string, ILogFormatterConfiguration> Formatters
        {
            get { return _formatters; }
        }

        public IKeyedEnumerable<string, IThrottlingRuleConfiguration> ThrottlingRules
        {
            get { return _throttlingRules; }
        }

        private void LoadFormatters(LoggerSettingsSection settings)
        {
            foreach (FormatterElement formatter in settings.Formatters)
            {
                _formatters.Add(new LogFormatterConfiguration { Name = formatter.Name, Template = formatter.Template });
            }
        }

        private void LoadThrottlingRules(LoggerSettingsSection settings)
        {
            foreach (ThrottlingRuleElement rule in settings.ThrottlingRules)
            {
                _throttlingRules.Add(
                    new ThrottlingRuleConfiguration
                    {
                        MinEventThreshold = rule.MinEventThreshold,
                        MinInterval = rule.MinInterval,
                        Name = rule.Name
                    });
            }
        }

        private void LoadCategories(LoggerSettingsSection settings)
        {
            foreach (CategoryElement categoryElement in settings.Categories)
            {
                var category = new Category { Name = categoryElement.Name };

                if (!string.IsNullOrEmpty(categoryElement.ThrottlingRule))
                {
                    if (_throttlingRules.Contains(categoryElement.ThrottlingRule))
                    {
                        category.ThrottlingRule = _throttlingRules[categoryElement.ThrottlingRule];
                    }
                    else
                    {
                        throw new LogConfigurationException(string.Format("The throttlingRule {0} specified for the category {1} to use was not found.", categoryElement.ThrottlingRule, categoryElement.Name));
                    }
                }

                category.Providers =
                    (from ProviderElement provider in categoryElement.Providers
                     select CreateLogProviderConfiguration(provider)).ToList();

                _categories.Add(category);
            }
        }

        private static LogProviderConfiguration CreateLogProviderConfiguration(ProviderElement provider)
        {
            var providerType = GetProviderType(provider);
            var mappers = GetMappers(provider, providerType);

            return new LogProviderConfiguration
            {
                ProviderType = providerType,
                FormatterName = provider.Formatter,
                Mappers = mappers.ToList()
            };
        }

        private static Type GetProviderType(ProviderElement provider)
        {
            var type = Type.GetType(provider.ProviderType);
            if (type == null)
            {
                throw new LogConfigurationException("The type " + provider.ProviderType + " was not specified correctly in the config file.");
            }

            if (!typeof(ILogProvider).IsAssignableFrom(type))
            {
                throw new LogConfigurationException("The type " + type + " does not implement ILogProvider.");
            }
            return type;
        }

        private static IEnumerable<IMapper> GetMappers(ProviderElement provider, Type providerType)
        {
            foreach (PropertyMapperElement propertyMapper in provider.PropertyMappers)
            {
                var property = providerType.GetProperty(propertyMapper.Property);
                if (property == null)
                {
                    throw new LogConfigurationException("The parameters for the provider are misconfigured.");
                }

                yield return new Mapper(property, propertyMapper.Value);
            }
        }
    }
}