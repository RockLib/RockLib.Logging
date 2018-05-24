using FluentAssertions;
using RockLib.Immutable;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace RockLib.Logging.Tests
{
    public class LoggerFactoryTests
    {
        [Fact]
        public void DefaultValueOfLoggersComesFromConfig()
        {
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
    }
}
