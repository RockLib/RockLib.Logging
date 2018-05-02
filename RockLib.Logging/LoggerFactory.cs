using RockLib.Configuration;
using RockLib.Configuration.ObjectFactory;
using RockLib.Immutable;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace RockLib.Logging
{
    public static class LoggerFactory
    {
        private static readonly ConcurrentDictionary<string, Logger> _loggerCache = new ConcurrentDictionary<string, Logger>(StringComparer.InvariantCultureIgnoreCase);

        private static readonly Semimutable<LoggerFactoryConfiguration> _configuration = new Semimutable<LoggerFactoryConfiguration>(GetDefaultLoggerFactoryFromConfig);

        public static LoggerFactoryConfiguration Configuration => _configuration.Value;

        public static void SetConfiguration(LoggerFactoryConfiguration configuration) => _configuration.Value = configuration;

        public static Logger GetInstance() => GetInstance(null);

        public static Logger GetInstance(string category) => _loggerCache.GetOrAdd(category ?? "default", CreateLogger);

        private static Logger CreateLogger(string category)
        {
            var providers = Configuration.LogProviders.Where(p => LogProviderHasCategory(p, category)).ToArray();
            return new Logger(Configuration.IsLoggingEnabled, Configuration.LoggingLevel, providers);
        }

        private static bool LogProviderHasCategory(ILogProvider logProvider, string category)
        {
            if (category.Equals("default", StringComparison.InvariantCultureIgnoreCase))
            {
                return logProvider.Categories == null
                    || logProvider.Categories.Count == 0
                    || logProvider.Categories.Contains("default", StringComparer.InvariantCultureIgnoreCase);
            }

            return logProvider.Categories != null
                && logProvider.Categories.Contains(category, StringComparer.InvariantCultureIgnoreCase);
        }

        private static LoggerFactoryConfiguration GetDefaultLoggerFactoryFromConfig() =>
             Config.Root.GetSection("rocklib.logging").Create<LoggerFactoryConfiguration>();
    }
}
