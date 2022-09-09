using FluentAssertions;
using Moq;
using RockLib.Logging.LogProcessing;
using RockLib.Logging.LogProviders;
using System;
using System.Collections.Generic;
using Xunit;

namespace RockLib.Logging.Tests;

public class LoggerTests
{
    [Fact]
    public void NameIsSetFromConstructor()
    {
        var logger = new Logger("foo");

        logger.Name.Should().BeSameAs("foo");
    }

    [Fact]
    public void LevelIsSetFromConstructor()
    {
        var logger = new Logger(level: LogLevel.Error);

        logger.Level.Should().Be(LogLevel.Error);
    }

    [Fact]
    public void ProvidersIsSetFromConstructor()
    {
        var logProviders = new ILogProvider[0];

        var logger = new Logger(logProviders: logProviders);

        logger.LogProviders.Should().BeSameAs(logProviders);
    }

    [Fact]
    public void IsDisabledIsSetFromConstructor()
    {
        var logger = new Logger(isDisabled: true);

        logger.IsDisabled.Should().Be(true);
    }

    [Fact]
    public void LogProcessorIsSetFromConstructor()
    {
        var logProcessor = new Mock<ILogProcessor>().Object;

        var logger = new Logger(logProcessor);

        logger.LogProcessor.Should().BeSameAs(logProcessor);
    }

    [Fact]
    public void LogProcessorUsesBackgroundLogProcessorWhenProcessingModeIsBackground()
    {
        var logger = new Logger(processingMode: Logger.ProcessingMode.Background);

        logger.LogProcessor.Should().BeOfType<BackgroundLogProcessor>();
    }

    [Fact]
    public void LogProcessorUsesSynchronousLogProcessorWhenProcessingModeIsSynchronous()
    {
        var logger = new Logger(processingMode: Logger.ProcessingMode.Synchronous);

        logger.LogProcessor.Should().BeOfType<SynchronousLogProcessor>();
    }

    [Fact]
    public void LogProcessorUsesFireAndForgetLogProcessorWhenProcessingModeIsFireAndForget()
    {
        var logger = new Logger(processingMode: Logger.ProcessingMode.FireAndForget);

        logger.LogProcessor.Should().BeOfType<FireAndForgetLogProcessor>();
    }

    [Fact]
    public void LogThrowsArgumentNullExceptionIfLogEntryIsNull()
    {
        var logger = new Logger();

        Assert.Throws<ArgumentNullException>(() => logger.Log(null));
    }

    [Fact]
    public void LogDoesNotCallLogProcessorWhenItHasBeenDisposed()
    {
        var mockLogProcessor = new Mock<ILogProcessor>();
        mockLogProcessor.Setup(m => m.IsDisposed).Returns(true);

        var logger = new Logger(mockLogProcessor.Object);

        mockLogProcessor.Verify(m => m.ProcessLogEntry(It.IsAny<ILogger>(), It.IsAny<LogEntry>()),
            Times.Never);
    }

    [Fact]
    public void LogCallsProcessLogEntryOnItsLogProcessor()
    {
        var mockLogProcessor = new Mock<ILogProcessor>();

        var logger = new Logger(mockLogProcessor.Object, logProviders: new ILogProvider[] { new ConsoleLogProvider() });

        var logEntry = new LogEntry();

        logger.Log(logEntry);

        mockLogProcessor.Verify(m => m.ProcessLogEntry(logger, logEntry), Times.Once);
    }

    [Fact]
    public void LogDoesNotCallLogProcessorWhenIsDisabledIsTrue()
    {
        var mockLogProcessor = new Mock<ILogProcessor>();

        var logger = new Logger(mockLogProcessor.Object, isDisabled: true, logProviders: new ILogProvider[] { new ConsoleLogProvider() });

        var logEntry = new LogEntry();

        logger.Log(logEntry);

        mockLogProcessor.Verify(m => m.ProcessLogEntry(It.IsAny<ILogger>(), It.IsAny<LogEntry>()),
            Times.Never);
    }

    [Fact]
    public void LogDoesNotCallLogProcessorWhenThereAreNoLogProviders()
    {
        var mockLogProcessor = new Mock<ILogProcessor>();

        var logger = new Logger(mockLogProcessor.Object, logProviders: new ILogProvider[0]);

        var logEntry = new LogEntry();

        logger.Log(logEntry);

        mockLogProcessor.Verify(m => m.ProcessLogEntry(It.IsAny<ILogger>(), It.IsAny<LogEntry>()),
            Times.Never);
    }

    [Fact]
    public void LogDoesNotCallLogProcessorWhenLevelIsHigherThanTheLogEntryLevel()
    {
        var mockLogProcessor = new Mock<ILogProcessor>();

        var logger = new Logger(mockLogProcessor.Object, level: LogLevel.Error, logProviders: new ILogProvider[] { new ConsoleLogProvider() });

        var logEntry = new LogEntry() { Level = LogLevel.Info };

        logger.Log(logEntry);

        mockLogProcessor.Verify(m => m.ProcessLogEntry(It.IsAny<ILogger>(), It.IsAny<LogEntry>()),
            Times.Never);
    }

    [Fact]
    public void LogAddsCallerInfoToLogEntry()
    {
        var logProviders = new[]
        {
            new ConsoleLogProvider()
        };

        var logger = new Logger(logProviders: logProviders, level: LogLevel.Info, processingMode: Logger.ProcessingMode.Synchronous);

        var logEntry = new LogEntry("Hello, world!", LogLevel.Info);

        logger.Log(logEntry);

        logEntry.CallerInfo.Should().NotBeNullOrWhiteSpace();
    }

    public static IEnumerable<object[]> LogLevelResolverProvider_TestsCases
    {
        get
        {
            foreach (LogLevel level in Enum.GetValues(typeof(LogLevel)))
            {
                yield return new object[] { level };
            }

            yield return new object[] { null };
        }
    }
    [Theory, MemberData(nameof(LogLevelResolverProvider_TestsCases))]
    public void LogLevelResolverProvider(LogLevel? expected)
    {
        var logLevelResolver = new Mock<ILogLevelResolver>();
        logLevelResolver.Setup(i => i.GetLogLevel()).Returns(expected);

        var logger = new Logger(level: LogLevel.Warn, logLevelResolver: logLevelResolver.Object);

        var actual = logger.Level;

        if (expected is not null)
        {
            actual.Should().Be(expected);
        }
        else
        {
            actual.Should().Be(LogLevel.Warn);
        }
    }
}
