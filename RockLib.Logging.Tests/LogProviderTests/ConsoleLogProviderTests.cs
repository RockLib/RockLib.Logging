using FluentAssertions;
using Moq;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.Tests
{
    public class ConsoleLogProviderTests
    {
        [Fact]
        public void Constructor1SetsFormatterToTemplateLogFormatter()
        {
            var consoleLogProvider = new ConsoleLogProvider("foo");

            consoleLogProvider.Formatter.Should().BeOfType<TemplateLogFormatter>();
            var formatter = (TemplateLogFormatter)consoleLogProvider.Formatter;
            formatter.Template.Should().Be("foo");
        }

        [Fact]
        public void Constructor1SetsLevel()
        {
            var consoleLogProvider = new ConsoleLogProvider(level: LogLevel.Warn);

            consoleLogProvider.Level.Should().Be(LogLevel.Warn);
        }

        [Fact]
        public void Constructor1SetsTimeout()
        {
            var timeout = TimeSpan.FromMilliseconds(1234);
            var consoleLogProvider = new ConsoleLogProvider(timeout: timeout);

            consoleLogProvider.Timeout.Should().Be(timeout);
        }

        [Fact]
        public void Constructor1SetsTimeoutToDefaultTimeoutWhenParameterIsNull()
        {
            var timeout = TimeSpan.FromMilliseconds(1234);
            var consoleLogProvider = new ConsoleLogProvider(timeout: null);

            consoleLogProvider.Timeout.Should().Be(ConsoleLogProvider.DefaultTimeout);
        }

        [Fact]
        public void Constructor2SetsFormatter()
        {
            var logFormatter = new Mock<ILogFormatter>().Object;

            var consoleLogProvider = new ConsoleLogProvider(logFormatter);

            consoleLogProvider.Formatter.Should().BeSameAs(logFormatter);
        }

        [Fact]
        public void Constructor2SetsLevel()
        {
            var logFormatter = new Mock<ILogFormatter>().Object;

            var consoleLogProvider = new ConsoleLogProvider(logFormatter, level: LogLevel.Warn);

            consoleLogProvider.Level.Should().Be(LogLevel.Warn);
        }

        [Fact]
        public void Constructor2SetsTimeout()
        {
            var logFormatter = new Mock<ILogFormatter>().Object;
            var timeout = TimeSpan.FromMilliseconds(1234);

            var consoleLogProvider = new ConsoleLogProvider(logFormatter, timeout: timeout);

            consoleLogProvider.Timeout.Should().Be(timeout);
        }

        [Fact]
        public void Constructor2SetsTimeoutToDefaultTimeoutWhenParameterIsNull()
        {
            var logFormatter = new Mock<ILogFormatter>().Object;
            var timeout = TimeSpan.FromMilliseconds(1234);

            var consoleLogProvider = new ConsoleLogProvider(logFormatter, timeout: null);

            consoleLogProvider.Timeout.Should().Be(ConsoleLogProvider.DefaultTimeout);
        }

        [Fact]
        public async Task WriteLineAsyncFormatsTheLogEntryAndWritesItToConsole()
        {
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                var original = Console.Out;
                Console.SetOut(writer);

                try
                {
                    var consoleLogProvider = new ConsoleLogProvider("{level}:{message}");

                    var logEntry = new LogEntry("Hello, world!", LogLevel.Info);

                    await consoleLogProvider.WriteAsync(logEntry);
                }
                finally
                {
                    Console.SetOut(original);
                }
            }

            sb.ToString().Should().Be($"Info:Hello, world!{Environment.NewLine}");
        }
    }
}
