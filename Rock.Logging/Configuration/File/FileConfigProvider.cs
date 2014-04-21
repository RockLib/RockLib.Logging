using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Rock.Logging.Configuration
{
    public class FileConfigProvider : IConfigProvider
    {
        public ILoggerFactoryConfiguration GetConfiguration()
        {
            var config = new LoggerFactoryConfiguration();

            var settings = (LoggerSettingsSection)ConfigurationManager.GetSection("rock.logging");

            config.IsLoggingEnabled = settings.IsLoggingEnabled;
            config.LoggingLevel = (LogLevel)Enum.Parse(typeof(LogLevel), settings.LoggingLevel);

            if (settings.AuditLogProvider != null)
            {
                config.AuditLogProvider = CreateLogProviderConfiguration(settings.AuditLogProvider);
            }

            LoadFormatters(config, settings);
            LoadThrottlingRules(config, settings);
            LoadCategories(config, settings);

            return config;
        }

        private static void LoadFormatters(LoggerFactoryConfiguration config, LoggerSettingsSection settings)
        {
            foreach (FormatterElement formatter in settings.Formatters)
            {
                config.Formatters.Add(new LogFormatterConfiguration { Name = formatter.Name, Template = formatter.Template });
            }
        }

        private static void LoadThrottlingRules(LoggerFactoryConfiguration config, LoggerSettingsSection settings)
        {
            foreach (ThrottlingRuleElement rule in settings.ThrottlingRules)
            {
                config.ThrottlingRules.Add(
                    new ThrottlingRuleConfiguration
                    {
                        MinEventThreshold = rule.MinEventThreshold,
                        MinInterval = rule.MinInterval,
                        Name = rule.Name
                    });
            }
        }

        private static void LoadCategories(LoggerFactoryConfiguration config, LoggerSettingsSection settings)
        {
            foreach (CategoryElement categoryElement in settings.Categories)
            {
                var category = new Category { Name = categoryElement.Name };

                if (!string.IsNullOrEmpty(categoryElement.ThrottlingRule))
                {
                    if (config.ThrottlingRules.Contains(categoryElement.ThrottlingRule))
                    {
                        category.ThrottlingRule = config.ThrottlingRules[categoryElement.ThrottlingRule];
                    }
                    else
                    {
                        // TODO: Implement LogConfigurationException
                        throw new /*LogConfiguration*/Exception("The throttlingRule " + categoryElement.ThrottlingRule + " specified for the category " + categoryElement.Name + " to use was not found.");
                    }
                }

                category.Providers =
                    (from ProviderElement provider in categoryElement.Providers
                     select CreateLogProviderConfiguration(provider)).ToList();

                config.Categories.Add(category);
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

        private static IEnumerable<IMapper> GetMappers(ProviderElement provider, Type providerType)
        {
            foreach (PropertyMapperElement propertyMapper in provider.PropertyMappers)
            {
                var property = providerType.GetProperty(propertyMapper.Property);
                if (property == null)
                {
                    throw new /*LogConfiguration*/ Exception("The parameters for the provider are misconfigured.");
                }

                yield return new Mapper(property, propertyMapper.Value);
            }
        }

        private static Type GetProviderType(ProviderElement provider)
        {
            var type = Type.GetType(provider.ProviderType);
            if (type == null)
            {
                // TODO: better exception and message.
                throw new Exception("The type " + provider.ProviderType + " was not specified correctly in the config file.");
            }

            if (!typeof(ILogProvider).IsAssignableFrom(type))
            {
                // TODO: better exception and message.
                throw new Exception("The type " + type + " does not implement ILogProvider.");
            }
            return type;
        }
    }
}