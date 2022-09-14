using FluentAssertions;
using Moq;
using RockLib.Logging.LogProcessing;
using RockLib.Logging.LogProviders;
using System;
using System.Collections.Generic;
using Xunit;

namespace RockLib.Logging.Tests;

#pragma warning disable CS0618 // Type or member is obsolete
public static class LoggerTests
{
    [Fact]
    public static void NameIsSetFromConstructor()
    {
        using var logger = new Logger("foo");

        logger.Name.Should().BeSameAs("foo");
    }

    [Fact]
    public static void LevelIsSetFromConstructor()
    {
        using var logger = new Logger(level: LogLevel.Error);

        logger.Level.Should().Be(LogLevel.Error);
    }

    [Fact]
    public static void ProvidersIsSetFromConstructor()
    {
        var logProviders = Array.Empty<ILogProvider>();

        using var logger = new Logger(logProviders: logProviders);

        logger.LogProviders.Should().BeSameAs(logProviders);
    }

    [Fact]
    public static void IsDisabledIsSetFromConstructor()
    {
        using var logger = new Logger(isDisabled: true);

        logger.IsDisabled.Should().Be(true);
    }

    [Fact]
    public static void LogProcessorIsSetFromConstructor()
    {
        var logProcessor = new Mock<ILogProcessor>().Object;

        using var logger = new Logger(logProcessor);

        logger.LogProcessor.Should().BeSameAs(logProcessor);
    }

    [Fact]
    public static void LogProcessorUsesBackgroundLogProcessorWhenProcessingModeIsBackground()
    {
        using var logger = new Logger(processingMode: Logger.ProcessingMode.Background);

        logger.LogProcessor.Should().BeOfType<BackgroundLogProcessor>();
    }

    [Fact]
    public static void LogProcessorUsesSynchronousLogProcessorWhenProcessingModeIsSynchronous()
    {
        using var logger = new Logger(processingMode: Logger.ProcessingMode.Synchronous);

        logger.LogProcessor.Should().BeOfType<SynchronousLogProcessor>();
    }

    [Fact]
    public static void LogProcessorUsesFireAndForgetLogProcessorWhenProcessingModeIsFireAndForget()
    {
        using var logger = new Logger(processingMode: Logger.ProcessingMode.FireAndForget);

        logger.LogProcessor.Should().BeOfType<FireAndForgetLogProcessor>();
    }

    [Fact]
    public static void LogThrowsArgumentNullExceptionIfLogEntryIsNull()
    {
        using var logger = new Logger();

        Assert.Throws<ArgumentNullException>(() => logger.Log(null!));
    }

    [Fact]
    public static void LogDoesNotCallLogProcessorWhenItHasBeenDisposed()
    {
        var mockLogProcessor = new Mock<ILogProcessor>();
        mockLogProcessor.Setup(m => m.IsDisposed).Returns(true);

        using var logger = new Logger(mockLogProcessor.Object);

        mockLogProcessor.Verify(m => m.ProcessLogEntry(It.IsAny<ILogger>(), It.IsAny<LogEntry>()),
            Times.Never);
    }

    [Fact]
    public static void LogCallsProcessLogEntryOnItsLogProcessor()
    {
        var mockLogProcessor = new Mock<ILogProcessor>();

        using var logger = new Logger(mockLogProcessor.Object, logProviders: new ILogProvider[] { new ConsoleLogProvider() });

        var logEntry = new LogEntry();

        logger.Log(logEntry);

        mockLogProcessor.Verify(m => m.ProcessLogEntry(logger, logEntry), Times.Once);
    }

    [Fact]
    public static void LogDoesNotCallLogProcessorWhenIsDisabledIsTrue()
    {
        var mockLogProcessor = new Mock<ILogProcessor>();

        using var logger = new Logger(mockLogProcessor.Object, isDisabled: true, logProviders: new ILogProvider[] { new ConsoleLogProvider() });

        var logEntry = new LogEntry();

        logger.Log(logEntry);

        mockLogProcessor.Verify(m => m.ProcessLogEntry(It.IsAny<ILogger>(), It.IsAny<LogEntry>()),
            Times.Never);
    }

    [Fact]
    public static void LogDoesNotCallLogProcessorWhenThereAreNoLogProviders()
    {
        var mockLogProcessor = new Mock<ILogProcessor>();

        using var logger = new Logger(mockLogProcessor.Object, logProviders: Array.Empty<ILogProvider>());

        var logEntry = new LogEntry();

        logger.Log(logEntry);

        mockLogProcessor.Verify(m => m.ProcessLogEntry(It.IsAny<ILogger>(), It.IsAny<LogEntry>()),
            Times.Never);
    }

    [Fact]
    public static void LogDoesNotCallLogProcessorWhenLevelIsHigherThanTheLogEntryLevel()
    {
        var mockLogProcessor = new Mock<ILogProcessor>();

        using var logger = new Logger(mockLogProcessor.Object, level: LogLevel.Error, logProviders: new ILogProvider[] { new ConsoleLogProvider() });

        var logEntry = new LogEntry() { Level = LogLevel.Info };

        logger.Log(logEntry);

        mockLogProcessor.Verify(m => m.ProcessLogEntry(It.IsAny<ILogger>(), It.IsAny<LogEntry>()),
            Times.Never);
    }

    [Fact]
    public static void LogAddsCallerInfoToLogEntry()
    {
        var logProviders = new[]
        {
            new ConsoleLogProvider()
        };

        using var logger = new Logger(logProviders: logProviders, level: LogLevel.Info, processingMode: Logger.ProcessingMode.Synchronous);

        var logEntry = new LogEntry("Hello, world!", LogLevel.Info);

        logger.Log(logEntry);

        logEntry.CallerInfo.Should().NotBeNullOrWhiteSpace();
    }

    public static IEnumerable<object[]> LogLevelResolverProviderTestsCases
    {
        get
        {
#if NETCOREAPP3_1
#pragma warning disable CS8605 // Unboxing a possibly null value.
#endif
            foreach (LogLevel level in Enum.GetValues(typeof(LogLevel)))
            {
                yield return new object[] { level };
            }
#if NETCOREAPP3_1
#pragma warning restore CS8605 // Unboxing a possibly null value.
#endif

            yield return new object[] { null! };
        }
    }
    [Theory, MemberData(nameof(LogLevelResolverProviderTestsCases))]
    public static void LogLevelResolverProvider(LogLevel? expected)
    {
        var logLevelResolver = new Mock<ILogLevelResolver>();
        logLevelResolver.Setup(i => i.GetLogLevel()).Returns(expected);

        using var logger = new Logger(level: LogLevel.Warn, logLevelResolver: logLevelResolver.Object);

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
#pragma warning restore CS0618 // Type or member is obsolete
