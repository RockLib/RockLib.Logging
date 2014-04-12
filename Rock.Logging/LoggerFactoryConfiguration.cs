using System;
using Rock.Collections;

namespace Rock.Logging
{
    public class LoggerFactoryConfiguration : ILoggerFactoryConfiguration
    {
        private readonly KeyedCollection<string, Category> _categories = new FunctionalKeyedCollection<string, Category>(c => c.Name);
        private readonly KeyedCollection<string, LogFormatter> _formatters = new FunctionalKeyedCollection<string, LogFormatter>(c => c.Name);
        private readonly KeyedCollection<string, ThrottlingRuleConfiguration> _throttlingRules = new FunctionalKeyedCollection<string, ThrottlingRuleConfiguration>(c => c.Name);

        public bool IsLoggingEnabled { get; set; }
        public LogLevel LoggingLevel { get; set; }
        public Type AuditProviderType { get; set; }

        public KeyedCollection<string, Category> Categories
        {
            get { return _categories; }
        }

        IKeyedEnumerable<string, ICategory> ILoggerFactoryConfiguration.Categories
        {
            get { return Categories; }
        }

        public KeyedCollection<string, LogFormatter> Formatters
        {
            get { return _formatters; }
        }

        IKeyedEnumerable<string, ILogFormatter> ILoggerFactoryConfiguration.Formatters
        {
            get { return Formatters; }
        }

        public KeyedCollection<string, ThrottlingRuleConfiguration> ThrottlingRules
        {
            get { return _throttlingRules; }
        }

        IKeyedEnumerable<string, IThrottlingRuleConfiguration> ILoggerFactoryConfiguration.ThrottlingRules
        {
            get { return ThrottlingRules; }
        }
    }
}