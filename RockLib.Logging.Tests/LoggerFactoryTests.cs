using FluentAssertions;
using Microsoft.Extensions.Configuration;
using RockLib.Configuration;
using RockLib.Immutable;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.Tests
{
    public class LoggerFactoryTests
    {
        [Theory]
        [InlineData(Logger.DefaultName, typeof(FooLogProvider))]
        [InlineData("bar", typeof(BarLogProvider))]
        public void CreateFromConfigWorksWithListOfLoggers(string name, Type expectedLogProviderType)
        {
            ResetConfig();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    ["rocklib.logging:0:Providers:type"] = "RockLib.Logging.Tests.FooLogProvider, RockLib.Logging.Tests",
                    ["rocklib.logging:1:name"] = "bar",
                    ["rocklib.logging:1:Providers:type"] = "RockLib.Logging.Tests.BarLogProvider, RockLib.Logging.Tests",
                })
                .Build();

            Config.SetRoot(config);

            var logger = LoggerFactory.CreateFromConfig(name);

            logger.Name.Should().Be(name);
            logger.Providers.Count.Should().Be(1);
            logger.Providers.First().Should().BeOfType(expectedLogProviderType);
        }

        [Fact]
        public void CreateFromConfigWorksWithSingleUnnamedLogger()
        {
            ResetConfig();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    ["rocklib.logging:Providers:type"] = "RockLib.Logging.Tests.FooLogProvider, RockLib.Logging.Tests",
                })
                .Build();

            Config.SetRoot(config);

            var logger = LoggerFactory.CreateFromConfig();

            logger.Name.Should().Be(Logger.DefaultName);
            logger.Providers.Count.Should().Be(1);
            logger.Providers.First().Should().BeOfType(typeof(FooLogProvider));
        }

        [Fact]
        public void CreateFromConfigWorksWithSingleNamedLogger()
        {
            ResetConfig();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    ["rocklib.logging:name"] = "bar",
                    ["rocklib.logging:Providers:type"] = "RockLib.Logging.Tests.BarLogProvider, RockLib.Logging.Tests",
                })
                .Build();

            Config.SetRoot(config);

            var logger = LoggerFactory.CreateFromConfig("bar");

            logger.Name.Should().Be("bar");
            logger.Providers.Count.Should().Be(1);
            logger.Providers.First().Should().BeOfType(typeof(BarLogProvider));
        }

        [Fact]
        public void CreateFromConfigThrowsWhenNotFoundInListOfLoggers()
        {
            ResetConfig();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    ["rocklib.logging:0:Providers:type"] = "RockLib.Logging.Tests.FooLogProvider, RockLib.Logging.Tests",
                    ["rocklib.logging:1:name"] = "bar",
                    ["rocklib.logging:1:Providers:type"] = "RockLib.Logging.Tests.BarLogProvider, RockLib.Logging.Tests",
                })
                .Build();

            Config.SetRoot(config);

            var name = "baz";
            Action action = () =>  LoggerFactory.CreateFromConfig(name);

            action.ShouldThrow<KeyNotFoundException>().WithMessage($"The {LoggerFactory.SectionName} section in RockLib.Configuration.Config.Root does not contain a Logger configuration with the name '{name}'.");
        }

        [Fact]
        public void CreateFromConfigThrowsWhenNotFoundInSingleLogger()
        {
            ResetConfig();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    ["rocklib.logging:name"] = "bar",
                    ["rocklib.logging:Providers:type"] = "RockLib.Logging.Tests.BarLogProvider, RockLib.Logging.Tests",
                })
                .Build();

            Config.SetRoot(config);

            var name = "baz";
            Action action = () => LoggerFactory.CreateFromConfig(name);

            action.ShouldThrow<KeyNotFoundException>().WithMessage($"The {LoggerFactory.SectionName} section in RockLib.Configuration.Config.Root does not contain a Logger configuration with the name '{name}'.");
        }

        [Fact]
        public void DefaultValueOfLoggersComesFromConfig()
        {
            ResetConfig();
            ResetLoggerFactoryLoggers();

            LoggerFactory.Loggers.Count.Should().Be(1);
            var logger = LoggerFactory.Loggers.First();
            logger.Name.Should().Be("TestLogger");
            logger.Level.Should().Be(LogLevel.Info);
            logger.Providers.Count.Should().Be(1);
            logger.Providers.First().Should().BeOfType<ConsoleLogProvider>();
            var provider = (ConsoleLogProvider)logger.Providers.First();
            provider.Level.Should().Be(LogLevel.Warn);
            provider.Formatter.Should().BeOfType<TemplateLogFormatter>();
            var formatter = (TemplateLogFormatter)provider.Formatter;
            formatter.Template.Should().Be("foo bar");
        }

        [Fact]
        public void CanSpecifyLoggersProgrammatically()
        {
            ResetLoggerFactoryLoggers();

            LoggerFactory.SetLoggers(new []
            {
                new Logger("foo"),
                new Logger("bar"),
                new Logger("baz")
            });

            LoggerFactory.Loggers.Count.Should().Be(3);

            LoggerFactory.Loggers.First().Name.Should().Be("foo");
            LoggerFactory.Loggers.Skip(1).First().Name.Should().Be("bar");
            LoggerFactory.Loggers.Skip(2).First().Name.Should().Be("baz");
        }

        [Fact]
        public void GetInstanceWithNameReturnsTheLoggerWithTheSameName()
        {
            ResetLoggerFactoryLoggers();

            Logger expectedLogger = new Logger("foo");

            LoggerFactory.SetLoggers(new[] { expectedLogger });

            var logger = LoggerFactory.GetInstance("foo");

            logger.Should().BeSameAs(expectedLogger);
        }

        [Fact]
        public void GetInstanceWithNameReturnsTheLoggerWithTheSameCaseInsensitiveName()
        {
            ResetLoggerFactoryLoggers();

            Logger expectedLogger = new Logger("foo");

            LoggerFactory.SetLoggers(new[] { expectedLogger });

            var logger = LoggerFactory.GetInstance("FOO");

            logger.Should().BeSameAs(expectedLogger);
        }

        [Fact]
        public void GetInstanceWithDefaultNameReturnsTheDefaultLogger()
        {
            ResetLoggerFactoryLoggers();

            var expectedLogger = new Logger();

            LoggerFactory.SetLoggers(new[] { expectedLogger });

            var logger = LoggerFactory.GetInstance();

            logger.Should().BeSameAs(expectedLogger);
        }

        [Fact]
        public void GetInstanceWithNullNameReturnsTheDefaultLogger()
        {
            ResetLoggerFactoryLoggers();

            var expectedLogger = new Logger();

            LoggerFactory.SetLoggers(new[] { expectedLogger });

            var logger = LoggerFactory.GetInstance(null);

            logger.Should().BeSameAs(expectedLogger);
        }

        [Fact]
        public void GetInstanceWithNoMatchThrowsKeyNotFoundException()
        {
            ResetLoggerFactoryLoggers();

            LoggerFactory.SetLoggers(new Logger[0]);

            Assert.Throws<KeyNotFoundException>(() => LoggerFactory.GetInstance());
        }

        private void ResetLoggerFactoryLoggers()
        {
            var loggersField = typeof(LoggerFactory).GetField("_loggers", BindingFlags.NonPublic | BindingFlags.Static);
            var lookupField = typeof(LoggerFactory).GetField("_lookup", BindingFlags.NonPublic | BindingFlags.Static);

            var loggers = (Semimutable<IReadOnlyCollection<Logger>>)loggersField.GetValue(null);
            loggers.GetUnlockValueMethod().Invoke(loggers, null);
            loggers.ResetValue();

            var lookup = (ConcurrentDictionary<string, Logger>)lookupField.GetValue(null);
            lookup.Clear();
        }

        private void ResetConfig()
        {
            var rootField = typeof(Config).GetField("_root", BindingFlags.NonPublic | BindingFlags.Static);

            var root = (Semimutable<IConfiguration>)rootField.GetValue(null);
            root.GetUnlockValueMethod().Invoke(root, null);
            root.ResetValue();
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
}
