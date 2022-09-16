using Microsoft.Extensions.Configuration;

namespace RockLib.Logging.Tests.DependencyInjection;

public class TestConfigurationSource : IConfigurationSource
{
    public TestConfigurationProvider Provider { get; } = new TestConfigurationProvider();

    public IConfigurationProvider Build(IConfigurationBuilder builder) => Provider;
}
