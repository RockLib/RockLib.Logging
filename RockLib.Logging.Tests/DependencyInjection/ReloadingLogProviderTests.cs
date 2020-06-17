using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RockLib.Dynamic;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.Tests.DependencyInjection
{
    public class ReloadingLogProviderTests
    {
        public static readonly Type ReloadingLogProviderOfTestOptions =
            Type.GetType("RockLib.Logging.DependencyInjection.ReloadingLogProvider`1, RockLib.Logging", true)
                .MakeGenericType(typeof(TestOptions));

        [Fact]
        public void ConstructorHappyPath()
        {
            var source = new TestConfigurationSource();
            source.Provider.Set("CustomLogProvider:Foo", "123");
            source.Provider.Set("CustomLogProvider:Bar", "abc");

            var configuration = new ConfigurationBuilder()
                .Add(source)
                .Build();

            var services = new ServiceCollection();
            services.Configure<TestOptions>("MyLogger", configuration.GetSection("CustomLogProvider"));

            var serviceProvider = services.BuildServiceProvider();

            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<TestOptions>>();
            var options = optionsMonitor.Get("MyLogger");
            Func<TestOptions, ILogProvider> createLogProvider = o =>
                new TestLogProvider { Foo = o.Foo, Bar = o.Bar };
            string name = "MyLogger";

            var reloadingLogProvider = ReloadingLogProviderOfTestOptions.New(optionsMonitor, options, createLogProvider, name);

            string actualName = reloadingLogProvider._name;
            
            actualName.Should().BeSameAs(name);

            TestLogProvider logProvider = reloadingLogProvider._logProvider;
            logProvider.Foo.Should().Be(123);
            logProvider.Bar.Should().Be("abc");
        }

        [Fact]
        public void ReloadHappyPath()
        {
            var source = new TestConfigurationSource();
            source.Provider.Set("CustomLogProvider:Foo", "123");
            source.Provider.Set("CustomLogProvider:Bar", "abc");

            var configuration = new ConfigurationBuilder()
                .Add(source)
                .Build();

            var services = new ServiceCollection();
            services.Configure<TestOptions>("MyLogger", configuration.GetSection("CustomLogProvider"));

            var serviceProvider = services.BuildServiceProvider();

            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<TestOptions>>();
            var options = optionsMonitor.Get("MyLogger");
            Func<TestOptions, ILogProvider> createLogProvider = o =>
                new TestLogProvider { Foo = o.Foo, Bar = o.Bar };
            string name = "MyLogger";

            var reloadingLogProvider = ReloadingLogProviderOfTestOptions.New(optionsMonitor, options, createLogProvider, name);

            TestLogProvider logProvider1 = reloadingLogProvider._logProvider;
            logProvider1.Foo.Should().Be(123);
            logProvider1.Bar.Should().Be("abc");

            source.Provider.Set("CustomLogProvider:Foo", "456");
            source.Provider.Set("CustomLogProvider:Bar", "xyz");
            source.Provider.Reload();

            TestLogProvider logProvider2 = reloadingLogProvider._logProvider;
            logProvider2.Should().NotBeSameAs(logProvider1);
            logProvider2.Foo.Should().Be(456);
            logProvider2.Bar.Should().Be("xyz");
        }

        private class TestLogProvider : ILogProvider
        {
            public int Foo { get; set; }

            public string Bar { get; set; }

            public TimeSpan Timeout => TimeSpan.FromSeconds(1);

            public LogLevel Level => LogLevel.Info;

            public Task WriteAsync(LogEntry logEntry, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        private class TestOptions
        {
            public int Foo { get; set; }

            public string Bar { get; set; }
        }
    }
}
