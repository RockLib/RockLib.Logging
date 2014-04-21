using Rock.Collections;

namespace Rock.Logging
{
    public interface ILoggerFactoryConfiguration : ILoggerConfiguration
    {
        ILogProviderConfiguration AuditLogProvider { get; }
        IKeyedEnumerable<string, ICategory> Categories { get; }
        IKeyedEnumerable<string, ILogFormatterConfiguration> Formatters { get; }
        IKeyedEnumerable<string, IThrottlingRuleConfiguration> ThrottlingRules { get; }
    }
}