using Microsoft.Extensions.Configuration;

namespace RockLib.Logging.Tests.DependencyInjection
{
    public class TestConfigurationProvider : ConfigurationProvider
    {
        public void Reload() => OnReload();
    }
}
