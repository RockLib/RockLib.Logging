using FluentAssertions;
using Moq;
using Moq.Protected;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.Tests;

public static class DebugLogProviderTests
{
    [Fact]
    public static void Constructor1SetsFormatterToTemplateLogFormatter()
    {
        var debugLogProvider = new DebugLogProvider("foo");

        debugLogProvider.Formatter.Should().BeOfType<TemplateLogFormatter>();
        var formatter = (TemplateLogFormatter)debugLogProvider.Formatter;
        formatter.Template.Should().Be("foo");
    }

    [Fact]
    public static void Constructor1SetsLevel()
    {
        var debugLogProvider = new DebugLogProvider(level: LogLevel.Warn);

        debugLogProvider.Level.Should().Be(LogLevel.Warn);
    }

    [Fact]
    public static void Constructor1SetsTimeout()
    {
        var timeout = TimeSpan.FromMilliseconds(1234);
        var debugLogProvider = new DebugLogProvider(timeout: timeout);

        debugLogProvider.Timeout.Should().Be(timeout);
    }

    [Fact]
    public static void Constructor1SetsTimeoutToDefaultTimeoutWhenParameterIsNull()
    {
        var timeout = TimeSpan.FromMilliseconds(1234);
        var debugLogProvider = new DebugLogProvider(timeout: null);

        debugLogProvider.Timeout.Should().Be(DebugLogProvider.DefaultTimeout);
    }

    [Fact]
    public static void Constructor2SetsFormatter()
    {
        var logFormatter = new Mock<ILogFormatter>().Object;

        var debugLogProvider = new DebugLogProvider(logFormatter);

        debugLogProvider.Formatter.Should().BeSameAs(logFormatter);
    }

    [Fact]
    public static void Constructor2SetsLevel()
    {
        var logFormatter = new Mock<ILogFormatter>().Object;

        var debugLogProvider = new DebugLogProvider(logFormatter, level: LogLevel.Warn);

        debugLogProvider.Level.Should().Be(LogLevel.Warn);
    }

    [Fact]
    public static void Constructor2SetsTimeout()
    {
        var logFormatter = new Mock<ILogFormatter>().Object;
        var timeout = TimeSpan.FromMilliseconds(1234);

        var debugLogProvider = new DebugLogProvider(logFormatter, timeout: timeout);

        debugLogProvider.Timeout.Should().Be(timeout);
    }

    [Fact]
    public static void Constructor2SetsTimeoutToDefaultTimeoutWhenParameterIsNull()
    {
        var logFormatter = new Mock<ILogFormatter>().Object;

        var debugLogProvider = new DebugLogProvider(logFormatter, timeout: null);

        debugLogProvider.Timeout.Should().Be(DebugLogProvider.DefaultTimeout);
    }

    [Fact]
    public static async Task WriteLineAsyncFormatsTheLogEntryAndWritesItToDebug()
    {
        var mockDebugLogProvider = new Mock<DebugLogProvider>("{level}:{message}", LogLevel.NotSet, null);
        mockDebugLogProvider.Protected().As<IProtected>()
            .Setup(m => m.WriteToDebug(It.IsAny<string>()));

        var debugLogProvider = mockDebugLogProvider.Object;

        var logEntry = new LogEntry("Hello, world!", LogLevel.Info);

        await debugLogProvider.WriteAsync(logEntry).ConfigureAwait(true);

        mockDebugLogProvider.Protected().As<IProtected>()
            .Verify(m => m.WriteToDebug("Info:Hello, world!"), Times.Once());
    }

    private interface IProtected
    {
        void WriteToDebug(string formattedLog);
    }
}
