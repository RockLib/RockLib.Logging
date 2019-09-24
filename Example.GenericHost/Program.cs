using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RockLib.Configuration;
using RockLib.Logging.DependencyInjection;
using System.Threading.Tasks;

namespace Example.GenericHost
{
    class Program
    {
        private static Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddConfiguration(Config.Root);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddRockLibLoggerSingleton(addLoggerProvider: true);
                    services.AddHostedService<ExampleHostedService>();
                });

            return hostBuilder.RunConsoleAsync();
        }
    }
}
