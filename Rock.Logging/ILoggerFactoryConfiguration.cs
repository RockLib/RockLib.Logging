using Rock.Collections;

namespace Rock.Logging
{
    public interface ILoggerFactoryConfiguration : ILoggerConfiguration
    {
        IKeyedEnumerable<string, ICategory> Categories { get; }
        IKeyedEnumerable<string, ILogFormatter> Formatters { get; }
        IKeyedEnumerable<string, IThrottlingRuleConfiguration> ThrottlingRules { get; }
    }
}