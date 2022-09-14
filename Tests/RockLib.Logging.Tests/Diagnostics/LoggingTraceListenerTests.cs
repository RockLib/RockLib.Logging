using FluentAssertions;
using Moq;
using RockLib.Logging.Diagnostics;
using RockLib.Logging.Moq;
using System;
using Xunit;

namespace RockLib.Logging.Tests.Diagnostics;

public static class LoggingTraceListenerTests
{
    [Fact(DisplayName = "Constructor sets properties")]
    public static void Constructor1HappyPath1()
    {
        var logger = new Mock<ILogger>().Object;

        using var listener = new LoggingTraceListener(logger, LogLevel.Info);

        listener.Logger.Should().BeSameAs(logger);
        listener.LogLevel.Should().Be(LogLevel.Info);
    }

    [Fact(DisplayName = "Constructor sets LogLevel to logger's level if not provided")]
    public static void Constructor1HappyPath2()
    {
        var logger = new MockLogger(LogLevel.Info).Object;

        using var listener = new LoggingTraceListener(logger);

        listener.Logger.Should().BeSameAs(logger);
        listener.LogLevel.Should().Be(LogLevel.Info);
    }

    [Fact(DisplayName = "Constructor sets LogLevel to logger's level if null")]
    public static void Constructor1HappyPath3()
    {
        var logger = new MockLogger(LogLevel.Info).Object;

        using var listener = new LoggingTraceListener(logger, null);

        listener.Logger.Should().BeSameAs(logger);
        listener.LogLevel.Should().Be(LogLevel.Info);
    }

    [Fact(DisplayName = "Constructor throws if logger is null")]
    public static void Constructor1SadPath()
    {
        var act = () => new LoggingTraceListener((null as ILogger)!, LogLevel.Info);

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*logger*");
    }

    [Fact(DisplayName = "Write logs at the correct level")]
    public static void WriteHappyPath1()
    {
        var mockLogger = new MockLogger();

        using var listener = new LoggingTraceListener(mockLogger.Object, LogLevel.Info);

        listener.Write("Hello, world!");

        mockLogger.VerifyInfo("Hello, world!", Times.Once());
    }

    [Fact(DisplayName = "Write does not log if the level is not high enough for the logger")]
    public static void WriteHappyPath2()
    {
        var mockLogger = new MockLogger(LogLevel.Warn);

        using var listener = new LoggingTraceListener(mockLogger.Object, LogLevel.Info);

        listener.Write("Hello, world!");

        mockLogger.VerifyInfo(Times.Never());
    }

    [Fact(DisplayName = "WriteLine logs at the correct level")]
    public static void WriteLineHappyPath1()
    {
        var mockLogger = new MockLogger();

        using var listener = new LoggingTraceListener(mockLogger.Object, LogLevel.Info);

        listener.WriteLine("Hello, world!");

        mockLogger.VerifyInfo("Hello, world!", Times.Once());
    }

    [Fact(DisplayName = "WriteLine does not log if the level is not high enough for the logger")]
    public static void WriteLineHappyPath2()
    {
        var mockLogger = new MockLogger(LogLevel.Warn);

        using var listener = new LoggingTraceListener(mockLogger.Object, LogLevel.Info);

        listener.WriteLine("Hello, world!");

        mockLogger.VerifyInfo(Times.Never());
    }
}
