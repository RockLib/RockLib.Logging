using FluentAssertions;
using Moq;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.Tests;

public static class ConsoleLogProviderTests
{
    [Fact]
    public static void Constructor1SetsFormatterToTemplateLogFormatter()
    {
        var consoleLogProvider = new ConsoleLogProvider("foo");

        consoleLogProvider.Formatter.Should().BeOfType<TemplateLogFormatter>();
        var formatter = (TemplateLogFormatter)consoleLogProvider.Formatter;
        formatter.Template.Should().Be("foo");
    }

    [Fact]
    public static void Constructor1SetsLevel()
    {
        var consoleLogProvider = new ConsoleLogProvider(level: LogLevel.Warn);

        consoleLogProvider.Level.Should().Be(LogLevel.Warn);
    }

    [Fact]
    public static void Constructor1SetsTimeout()
    {
        var timeout = TimeSpan.FromMilliseconds(1234);
        var consoleLogProvider = new ConsoleLogProvider(timeout: timeout);

        consoleLogProvider.Timeout.Should().Be(timeout);
    }

    [Fact]
    public static void Constructor1SetsTimeoutToDefaultTimeoutWhenParameterIsNull()
    {
        var timeout = TimeSpan.FromMilliseconds(1234);
        var consoleLogProvider = new ConsoleLogProvider(timeout: null);

        consoleLogProvider.Timeout.Should().Be(ConsoleLogProvider.DefaultTimeout);
    }

    [Fact]
    public static void Constructor2SetsFormatter()
    {
        var logFormatter = new Mock<ILogFormatter>().Object;

        var consoleLogProvider = new ConsoleLogProvider(logFormatter);

        consoleLogProvider.Formatter.Should().BeSameAs(logFormatter);
    }

    [Fact]
    public static void Constructor2SetsLevel()
    {
        var logFormatter = new Mock<ILogFormatter>().Object;

        var consoleLogProvider = new ConsoleLogProvider(logFormatter, level: LogLevel.Warn);

        consoleLogProvider.Level.Should().Be(LogLevel.Warn);
    }

    [Fact]
    public static void Constructor2SetsTimeout()
    {
        var logFormatter = new Mock<ILogFormatter>().Object;
        var timeout = TimeSpan.FromMilliseconds(1234);

        var consoleLogProvider = new ConsoleLogProvider(logFormatter, timeout: timeout);

        consoleLogProvider.Timeout.Should().Be(timeout);
    }

    [Fact]
    public static void Constructor2SetsTimeoutToDefaultTimeoutWhenParameterIsNull()
    {
        var logFormatter = new Mock<ILogFormatter>().Object;

        var consoleLogProvider = new ConsoleLogProvider(logFormatter, timeout: null);

        consoleLogProvider.Timeout.Should().Be(ConsoleLogProvider.DefaultTimeout);
    }

    [Theory]
    [InlineData(ConsoleLogProvider.Output.StdOut)]
    [InlineData(ConsoleLogProvider.Output.StdErr)]
    public static async Task WriteLineAsyncFormatsTheLogEntryAndWritesItToConsole(ConsoleLogProvider.Output output)
    {
        var sb = new StringBuilder();
        using (var writer = new StringWriter(sb))
        {
            Action revert;

            if (output == ConsoleLogProvider.Output.StdOut)
            {
                var original = Console.Out;
                Console.SetOut(writer);
                revert = () => Console.SetOut(original);
            }
            else
            {
                var original = Console.Error;
                Console.SetError(writer);
                revert = () => Console.SetError(original);
            }

            try
            {
                var consoleLogProvider = new ConsoleLogProvider("{level}:{message}", output: output);

                var logEntry = new LogEntry("Hello, world!", LogLevel.Info);

                await consoleLogProvider.WriteAsync(logEntry).ConfigureAwait(false);
            }
            finally
            {
                revert();
            }
        }

        sb.ToString().Should().Be($"Info:Hello, world!{Environment.NewLine}");
    }
}
