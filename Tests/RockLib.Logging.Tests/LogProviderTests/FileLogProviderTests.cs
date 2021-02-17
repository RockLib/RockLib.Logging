using FluentAssertions;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.Tests
{
    public class FileLogProviderTests
    {
        private readonly string _file = Path.GetRandomFileName();

        [Fact]
        public void Constructor1SetsFile()
        {
            var fileLogProvider = new FileLogProvider(_file, level: LogLevel.Warn);

            fileLogProvider.File.Should().Be(_file);
        }

        [Fact]
        public void Constructor1SetsFormatterToTemplateLogFormatter()
        {
            var fileLogProvider = new FileLogProvider(_file, "foo");

            fileLogProvider.Formatter.Should().BeOfType<TemplateLogFormatter>();
            var formatter = (TemplateLogFormatter)fileLogProvider.Formatter;
            formatter.Template.Should().Be("foo");
        }

        [Fact]
        public void Constructor1SetsLevel()
        {
            var fileLogProvider = new FileLogProvider(_file, level: LogLevel.Warn);

            fileLogProvider.Level.Should().Be(LogLevel.Warn);
        }

        [Fact]
        public void Constructor1SetsTimeout()
        {
            var timeout = TimeSpan.FromMilliseconds(1234);
            var fileLogProvider = new FileLogProvider(_file, timeout: timeout);

            fileLogProvider.Timeout.Should().Be(timeout);
        }

        [Fact]
        public void Constructor1SetsTimeoutToDefaultTimeoutWhenParameterIsNull()
        {
            var timeout = TimeSpan.FromMilliseconds(1234);
            var fileLogProvider = new FileLogProvider(_file, timeout: null);

            fileLogProvider.Timeout.Should().Be(FileLogProvider.DefaultTimeout);
        }

        [Fact]
        public void Constructor2SetsFile()
        {
            var logFormatter = new Mock<ILogFormatter>().Object;

            var fileLogProvider = new FileLogProvider(_file, logFormatter);

            fileLogProvider.File.Should().Be(_file);
        }

        [Fact]
        public void Constructor2SetsFormatter()
        {
            var logFormatter = new Mock<ILogFormatter>().Object;

            var fileLogProvider = new FileLogProvider(_file, logFormatter);

            fileLogProvider.Formatter.Should().BeSameAs(logFormatter);
        }

        [Fact]
        public void Constructor2SetsLevel()
        {
            var logFormatter = new Mock<ILogFormatter>().Object;

            var fileLogProvider = new FileLogProvider(_file, logFormatter, level: LogLevel.Warn);

            fileLogProvider.Level.Should().Be(LogLevel.Warn);
        }

        [Fact]
        public void Constructor2SetsTimeout()
        {
            var logFormatter = new Mock<ILogFormatter>().Object;
            var timeout = TimeSpan.FromMilliseconds(1234);

            var fileLogProvider = new FileLogProvider(_file, logFormatter, timeout: timeout);

            fileLogProvider.Timeout.Should().Be(timeout);
        }

        [Fact]
        public void Constructor2SetsTimeoutToDefaultTimeoutWhenParameterIsNull()
        {
            var logFormatter = new Mock<ILogFormatter>().Object;
            var timeout = TimeSpan.FromMilliseconds(1234);

            var fileLogProvider = new FileLogProvider(_file, logFormatter, timeout: null);

            fileLogProvider.Timeout.Should().Be(FileLogProvider.DefaultTimeout);
        }

        [Fact]
        public async Task WriteLineAsyncFormatsTheLogEntryAndWritesItToDisk()
        {
            var fileLogProvider = new FileLogProvider(_file, "{level}:{message}");

            var logEntry = new LogEntry("Hello, world!", LogLevel.Info);

            await fileLogProvider.WriteAsync(logEntry);

            try
            {
                var output = await File.ReadAllTextAsync(_file);
                output.Should().Be($"Info:Hello, world!{Environment.NewLine}");
            }
            finally
            {
                File.Delete(_file);
            }
        }
    }
}
