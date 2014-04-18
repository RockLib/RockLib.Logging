using System;
using System.Configuration;
using Rock.Configuration;

namespace Rock.Logging.Configuration
{
    public class FileConfigProvider : IConfigProvider
    {
        public ILoggerFactoryConfiguration GetConfiguration()
        {
            var config = new LoggerFactoryConfiguration();

            var rockSettings = (RockFrameworkSection)ConfigurationManager.GetSection("rock.framework");
            var settings = rockSettings.LoggerSettings.LoggerSettings;

            config.IsLoggingEnabled = settings.IsLoggingEnabled;
            config.LoggingLevel = (LogLevel)Enum.Parse(typeof(LogLevel), settings.LoggingLevel);
            //config.AuditProviderType = ???; // TODO: figure out what to do with auditing.

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

                config.Categories.Add(category);
                LoadProviders(categoryElement, category);
            }
        }

        private static void LoadProviders(CategoryElement categoryElement, Category category)
        {
            foreach (ProviderElement provider in categoryElement.Providers)
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

                var config = new LogProviderConfiguration { ProviderType = type, FormatterName = provider.Formatter };
                category.Providers.Add(config);
            }
        }
    }
}