using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using RockLib.Configuration.ObjectFactory;
using RockLib.Immutable;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.Tests;

public class LoggerFactoryTests
{
    [Fact]
    public void LegacyConfigurationFormatIsSupported()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["rocklib.logging:Providers:type"] = "RockLib.Logging.Tests.FooLogProvider, RockLib.Logging.Tests",
            })
            .Build()
            .GetSection("rocklib.logging");

        var logger = config.CreateLogger();

        logger.Name.Should().Be(Logger.DefaultName);
        logger.LogProviders.Count.Should().Be(1);
        logger.LogProviders.First().Should().BeOfType(typeof(FooLogProvider));

        config.CreateLogger().Should().NotBeSameAs(logger);
    }

    [Theory]
    [InlineData(Logger.DefaultName, typeof(FooLogProvider))]
    [InlineData("bar", typeof(BarLogProvider))]
    public void CreateLoggerWorksWithListOfLoggers(string name, Type expectedLogProviderType)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["rocklib.logging:0:LogProviders:type"] = "RockLib.Logging.Tests.FooLogProvider, RockLib.Logging.Tests",
                ["rocklib.logging:1:name"] = "bar",
                ["rocklib.logging:1:LogProviders:type"] = "RockLib.Logging.Tests.BarLogProvider, RockLib.Logging.Tests",
            })
            .Build()
            .GetSection("rocklib.logging");

        var logger = config.CreateLogger(name);

        logger.Name.Should().Be(name);
        logger.LogProviders.Count.Should().Be(1);
        logger.LogProviders.First().Should().BeOfType(expectedLogProviderType);

        config.CreateLogger(name).Should().NotBeSameAs(logger);
    }

    [Fact]
    public void CreateLoggerWorksWithSingleUnnamedLogger()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["rocklib.logging:LogProviders:type"] = "RockLib.Logging.Tests.FooLogProvider, RockLib.Logging.Tests",
            })
            .Build()
            .GetSection("rocklib.logging");

        var logger = config.CreateLogger();

        logger.Name.Should().Be(Logger.DefaultName);
        logger.LogProviders.Count.Should().Be(1);
        logger.LogProviders.First().Should().BeOfType(typeof(FooLogProvider));

        config.CreateLogger().Should().NotBeSameAs(logger);
    }

    [Fact]
    public void CreateLoggerWorksWithSingleNamedLogger()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["rocklib.logging:name"] = "bar",
                ["rocklib.logging:LogProviders:type"] = "RockLib.Logging.Tests.BarLogProvider, RockLib.Logging.Tests",
            })
            .Build()
            .GetSection("rocklib.logging");

        var logger = config.CreateLogger("bar");

        logger.Name.Should().Be("bar");
        logger.LogProviders.Count.Should().Be(1);
        logger.LogProviders.First().Should().BeOfType(typeof(BarLogProvider));

        config.CreateLogger("bar").Should().NotBeSameAs(logger);
    }

    [Fact]
    public void CreateLoggerThrowsWhenNotFoundInListOfLoggers()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["rocklib.logging:0:LogProviders:type"] = "RockLib.Logging.Tests.FooLogProvider, RockLib.Logging.Tests",
                ["rocklib.logging:1:name"] = "bar",
                ["rocklib.logging:1:LogProviders:type"] = "RockLib.Logging.Tests.BarLogProvider, RockLib.Logging.Tests",
            })
            .Build()
            .GetSection("rocklib.logging");

        var name = "baz";
        Action action = () =>  config.CreateLogger(name);

        action.Should().Throw<KeyNotFoundException>().WithMessage($"No loggers were found matching the name '{name}'.");
    }

    [Fact]
    public void CreateLoggerThrowsWhenNotFoundInSingleLogger()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["rocklib.logging:name"] = "bar",
                ["rocklib.logging:LogProviders:type"] = "RockLib.Logging.Tests.BarLogProvider, RockLib.Logging.Tests",
            })
            .Build()
            .GetSection("rocklib.logging");

        var name = "baz";
        Action action = () => config.CreateLogger(name);

        action.Should().Throw<KeyNotFoundException>().WithMessage($"No loggers were found matching the name '{name}'.");
    }

    [Theory]
    [InlineData(Logger.DefaultName, typeof(FooLogProvider))]
    [InlineData("bar", typeof(BarLogProvider))]
    public void GetCachedLoggerWorksWithListOfLoggers(string name, Type expectedLogProviderType)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["rocklib.logging:0:LogProviders:type"] = "RockLib.Logging.Tests.FooLogProvider, RockLib.Logging.Tests",
                ["rocklib.logging:1:name"] = "bar",
                ["rocklib.logging:1:LogProviders:type"] = "RockLib.Logging.Tests.BarLogProvider, RockLib.Logging.Tests",
            })
            .Build()
            .GetSection("rocklib.logging");

        var logger = config.GetCachedLogger(name);

        logger.Name.Should().Be(name);
        logger.LogProviders.Count.Should().Be(1);
        logger.LogProviders.First().Should().BeOfType(expectedLogProviderType);

        config.GetCachedLogger(name).Should().BeSameAs(logger);
    }

    [Fact]
    public void GetCachedLoggerWorksWithSingleUnnamedLogger()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["rocklib.logging:LogProviders:type"] = "RockLib.Logging.Tests.FooLogProvider, RockLib.Logging.Tests",
            })
            .Build()
            .GetSection("rocklib.logging");

        var logger = config.GetCachedLogger();

        logger.Name.Should().Be(Logger.DefaultName);
        logger.LogProviders.Count.Should().Be(1);
        logger.LogProviders.First().Should().BeOfType(typeof(FooLogProvider));

        config.GetCachedLogger().Should().BeSameAs(logger);
    }

    [Fact]
    public void GetCachedLoggerWorksWithSingleNamedLogger()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["rocklib.logging:name"] = "bar",
                ["rocklib.logging:LogProviders:type"] = "RockLib.Logging.Tests.BarLogProvider, RockLib.Logging.Tests",
            })
            .Build()
            .GetSection("rocklib.logging");

        var logger = config.GetCachedLogger("bar");

        logger.Name.Should().Be("bar");
        logger.LogProviders.Count.Should().Be(1);
        logger.LogProviders.First().Should().BeOfType(typeof(BarLogProvider));

        config.GetCachedLogger("bar").Should().BeSameAs(logger);
    }

    [Fact]
    public void GetCachedLoggerThrowsWhenNotFoundInListOfLoggers()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["rocklib.logging:0:LogProviders:type"] = "RockLib.Logging.Tests.FooLogProvider, RockLib.Logging.Tests",
                ["rocklib.logging:1:name"] = "bar",
                ["rocklib.logging:1:LogProviders:type"] = "RockLib.Logging.Tests.BarLogProvider, RockLib.Logging.Tests",
            })
            .Build()
            .GetSection("rocklib.logging");

        var name = "baz";
        Action action = () => config.GetCachedLogger(name);

        action.Should().Throw<KeyNotFoundException>().WithMessage($"No loggers were found matching the name '{name}'.");
    }

    [Fact]
    public void GetCachedLoggerThrowsWhenNotFoundInSingleLogger()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["rocklib.logging:name"] = "bar",
                ["rocklib.logging:LogProviders:type"] = "RockLib.Logging.Tests.BarLogProvider, RockLib.Logging.Tests",
            })
            .Build()
            .GetSection("rocklib.logging");

        var name = "baz";
        Action action = () => config.GetCachedLogger(name);

        action.Should().Throw<KeyNotFoundException>().WithMessage($"No loggers were found matching the name '{name}'.");
    }

    [Fact]
    public void SetConfigurationSetsTheConfigurationProperty()
    {
        var configurationField = GetSemimutableConfigurationField();

        var existingConfig = configurationField.Value!;
        configurationField.GetUnlockValueMethod().Invoke(configurationField, null);

        var config = new ConfigurationBuilder().Build();

        LoggerFactory.SetConfiguration(config);

        try
        {
            LoggerFactory.Configuration.Should().BeSameAs(config);
        }
        finally
        {
            configurationField.GetUnlockValueMethod().Invoke(configurationField, null);
            LoggerFactory.SetConfiguration(existingConfig);
        }
    }

    [Fact]
    public void CreateCallsCreateLoggerWithConfigurationProperty()
    {
        var configurationField = GetSemimutableConfigurationField();

        var existingConfig = configurationField.Value!;
        configurationField.GetUnlockValueMethod().Invoke(configurationField, null);

        var config = new InterceptingConfigurationSection(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["rocklib.logging:LogProviders:type"] = "RockLib.Logging.Tests.FooLogProvider, RockLib.Logging.Tests",
            })
            .Build()
            .GetSection("RockLib.Logging"));

        LoggerFactory.SetConfiguration(config);

        try
        {
            var logger = LoggerFactory.Create();

            config.Usages.Should().BeGreaterThan(0);

            logger.Name.Should().Be(Logger.DefaultName);
            logger.LogProviders.Count.Should().Be(1);
            logger.LogProviders.First().Should().BeOfType(typeof(FooLogProvider));

            LoggerFactory.Create().Should().NotBeSameAs(logger);
        }
        finally
        {
            configurationField.GetUnlockValueMethod().Invoke(configurationField, null);
            LoggerFactory.SetConfiguration(existingConfig);
        }
    }

    [Fact]
    public void GetCachedCallsGetCachedLoggerWithConfigurationProperty()
    {
        var configurationField = GetSemimutableConfigurationField();

        var existingConfig = configurationField.Value!;
        configurationField.GetUnlockValueMethod().Invoke(configurationField, null);

        var config = new InterceptingConfigurationSection(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["rocklib.logging:LogProviders:type"] = "RockLib.Logging.Tests.FooLogProvider, RockLib.Logging.Tests",
            })
            .Build()
            .GetSection("RockLib.Logging"));

        LoggerFactory.SetConfiguration(config);

        try
        {
            var logger = LoggerFactory.GetCached();

            config.Usages.Should().BeGreaterThan(0);

            logger.Name.Should().Be(Logger.DefaultName);
            logger.LogProviders.Count.Should().Be(1);
            logger.LogProviders.First().Should().BeOfType(typeof(FooLogProvider));

            LoggerFactory.GetCached().Should().BeSameAs(logger);
        }
        finally
        {
            configurationField.GetUnlockValueMethod().Invoke(configurationField, null);
            LoggerFactory.SetConfiguration(existingConfig);
        }
    }

    [Fact]
    public void DefaultTypesFunctionsProperly()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "RockLib.Logging:Name", "foo" }
            }).Build();

        var defaultTypes = new DefaultTypes
        {
            { typeof(ILogger), typeof(TestLogger) }
        };

        var section = config.GetSection("RockLib.Logging");

        using var logger = section.CreateLogger("foo", defaultTypes: defaultTypes, reloadOnConfigChange: false);

        logger.Should().BeOfType<TestLogger>();
    }

    [Fact]
    public void ValueConvertersFunctionsProperly()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "RockLib.Logging:Type", typeof(TestLogger).AssemblyQualifiedName! },
                { "RockLib.Logging:Value:Name", "foo" },
                { "RockLib.Logging:Value:Location", "2,3" }
            }).Build();

        Point ParsePoint(string value)
        {
            var split = value.Split(',');
            return new Point(int.Parse(split[0], CultureInfo.CurrentCulture), int.Parse(split[1], CultureInfo.CurrentCulture));
        }

        var valueConverters = new ValueConverters
        {
            { typeof(Point), ParsePoint }
        };

        var section = config.GetSection("RockLib.Logging");

        var logger = (TestLogger)section.CreateLogger("foo", valueConverters: valueConverters, reloadOnConfigChange: false);

        logger.Location.X.Should().Be(2);
        logger.Location.Y.Should().Be(3);
    }

    [Fact]
    public void ResolverFunctionsProperly()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "RockLib.Logging:Type", typeof(TestLogger).AssemblyQualifiedName! },
                { "RockLib.Logging:Value:Name", "foo" }
            }).Build();

        var dependency = new TestDependency();
        var resolver = new Resolver(t => dependency, t => t == typeof(ITestDependency));

        var section = config.GetSection("RockLib.Logging");

        using var logger = (TestLogger)section.CreateLogger("foo", resolver: resolver, reloadOnConfigChange: false);

        logger.Dependency.Should().BeSameAs(dependency);
    }

    [Fact]
    public void ReloadOnConfigChangeTrueFunctionsProperly()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "RockLib.Logging:Type", typeof(TestLogger).AssemblyQualifiedName! },
                { "RockLib.Logging:Value:Name", "foo" }
            }).Build();

        var section = config.GetSection("RockLib.Logging");

        using var logger = section.CreateLogger("foo", reloadOnConfigChange: true);

        logger.Should().BeAssignableTo<ConfigReloadingProxy<ILogger>>();
    }

    [Fact]
    public void ReloadOnConfigChangeFalseFunctionsProperly()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "RockLib.Logging:Type", typeof(TestLogger).AssemblyQualifiedName! },
                { "RockLib.Logging:Value:Name", "foo" }
            }).Build();

        var messagingSection = config.GetSection("RockLib.Logging");

        using var sender = messagingSection.CreateLogger("foo", reloadOnConfigChange: false);

        sender.Should().BeOfType<TestLogger>();
    }

    private class InterceptingConfigurationSection : IConfigurationSection
    {
        private readonly IConfigurationSection _configuration;

        public InterceptingConfigurationSection(IConfigurationSection configuration) => _configuration = configuration;

        public int Usages { get; private set; }

        public string this[string key]
        {
            get { Usages++;  return _configuration[key]; }
            set { Usages++; _configuration[key] = value; }
        }

        public string Key
        {
            get { Usages++; return _configuration.Key; }
        }

        public string Path
        {
            get { Usages++; return _configuration.Path; }
        }

        public string Value
        {
            get { Usages++; return _configuration.Value; }
            set { Usages++; _configuration.Value = value; }
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            Usages++; return _configuration.GetChildren();
        }

        public IChangeToken GetReloadToken()
        {
            Usages++; return _configuration.GetReloadToken();
        }

        public IConfigurationSection GetSection(string key)
        {
            Usages++; return _configuration.GetSection(key);
        }
    }

    private static Semimutable<IConfiguration> GetSemimutableConfigurationField()
    {
        var field = typeof(LoggerFactory).GetField("_configuration", BindingFlags.NonPublic | BindingFlags.Static)!;
        return (Semimutable<IConfiguration>)field.GetValue(null)!;
    }

    private class TestLogger : ILogger
    {
        public TestLogger(Point location = default, ITestDependency? dependency = null)
        {
            Name = nameof(TestLogger);
            Location = location;
            Dependency = dependency;
        }

        public Point Location { get; }
        public ITestDependency? Dependency { get; }

        public string Name { get; }
        public bool IsDisabled { get; }
        public LogLevel Level { get; }
        public IReadOnlyCollection<ILogProvider> LogProviders { get; } = new List<ILogProvider>();
        public IReadOnlyCollection<IContextProvider> ContextProviders { get; } = new List<IContextProvider>();
        public IErrorHandler? ErrorHandler { get; set; }

        public void Log(LogEntry logEntry, string? callerMemberName = null, string? callerFilePath = null, int callerLineNumber = 0) => throw new NotImplementedException();

        public void Dispose() { }
    }

    private struct Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }

        public int Y { get; }
    }

    private interface ITestDependency
    {
    }

    private class TestDependency : ITestDependency
    {
    }
}

public class FooLogProvider : ILogProvider
{
    public TimeSpan Timeout => throw new NotImplementedException();
    public LogLevel Level => throw new NotImplementedException();
    public Task WriteAsync(LogEntry logEntry, CancellationToken cancellationToken) => throw new NotImplementedException();
}

public class BarLogProvider : ILogProvider
{
    public TimeSpan Timeout => throw new NotImplementedException();
    public LogLevel Level => throw new NotImplementedException();
    public Task WriteAsync(LogEntry logEntry, CancellationToken cancellationToken) => throw new NotImplementedException();
}
