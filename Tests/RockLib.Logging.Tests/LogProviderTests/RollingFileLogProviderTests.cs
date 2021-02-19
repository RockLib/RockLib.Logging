using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.Tests
{
    public class RollingFileLogProviderTests : IDisposable
    {
        private static readonly string _logFileDirectory = Path.Combine(Path.GetTempPath(), "RockLib.Logging.Tests");
        private static readonly string _logFilePath = Path.Combine(_logFileDirectory, "log.txt");

        public RollingFileLogProviderTests()
        {
            if (Directory.Exists(_logFileDirectory))
            {
                Directory.Delete(_logFileDirectory, true);
            }

            Directory.CreateDirectory(_logFileDirectory);
        }

        public void Dispose()
        {
            if (Directory.Exists(_logFileDirectory))
            {
                Directory.Delete(_logFileDirectory, true);
            }
        }

        [Fact]
        public void Constructor1SetsFile()
        {
            var fileLogProvider = new RollingFileLogProvider(_logFilePath, level: LogLevel.Warn);

            fileLogProvider.File.Should().Be(_logFilePath);
        }

        [Fact]
        public void Constructor1SetsFormatterToTemplateLogFormatter()
        {
            var fileLogProvider = new RollingFileLogProvider(_logFilePath, "foo");

            fileLogProvider.Formatter.Should().BeOfType<TemplateLogFormatter>();
            var formatter = (TemplateLogFormatter)fileLogProvider.Formatter;
            formatter.Template.Should().Be("foo");
        }

        [Fact]
        public void Constructor1SetsLevel()
        {
            var fileLogProvider = new RollingFileLogProvider(_logFilePath, level: LogLevel.Warn);

            fileLogProvider.Level.Should().Be(LogLevel.Warn);
        }

        [Fact]
        public void Constructor1SetsTimeout()
        {
            var timeout = TimeSpan.FromMilliseconds(1234);
            var fileLogProvider = new RollingFileLogProvider(_logFilePath, timeout: timeout);

            fileLogProvider.Timeout.Should().Be(timeout);
        }

        [Fact]
        public void Constructor1SetsTimeoutToDefaultTimeoutWhenParameterIsNull()
        {
            var timeout = TimeSpan.FromMilliseconds(1234);
            var fileLogProvider = new RollingFileLogProvider(_logFilePath, timeout: null);

            fileLogProvider.Timeout.Should().Be(RollingFileLogProvider.DefaultTimeout);
        }
        
        [Fact]
        public void Constructor1SetsMaxFileSizeKilobytes()
        {
            var fileLogProvider = new RollingFileLogProvider(_logFilePath, maxFileSizeKilobytes: 1);

            fileLogProvider.MaxFileSizeBytes.Should().Be(1024);
        }

        [Fact]
        public void Constructor1SetsMaxArchiveCount()
        {
            var fileLogProvider = new RollingFileLogProvider(_logFilePath, maxArchiveCount: 3);

            fileLogProvider.MaxArchiveCount.Should().Be(3);
        }

        [Fact]
        public void Constructor1SetsRolloverPeriod()
        {
            var fileLogProvider = new RollingFileLogProvider(_logFilePath, rolloverPeriod: RolloverPeriod.Hourly);

            fileLogProvider.RolloverPeriod.Should().Be(RolloverPeriod.Hourly);
        }

        [Fact]
        public void Constructor2SetsFile()
        {
            var logFormatter = new Mock<ILogFormatter>().Object;

            var fileLogProvider = new RollingFileLogProvider(_logFilePath, logFormatter);

            fileLogProvider.File.Should().Be(_logFilePath);
        }

        [Fact]
        public void Constructor2SetsFormatter()
        {
            var logFormatter = new Mock<ILogFormatter>().Object;

            var fileLogProvider = new RollingFileLogProvider(_logFilePath, logFormatter);

            fileLogProvider.Formatter.Should().BeSameAs(logFormatter);
        }

        [Fact]
        public void Constructor2SetsLevel()
        {
            var logFormatter = new Mock<ILogFormatter>().Object;

            var fileLogProvider = new RollingFileLogProvider(_logFilePath, logFormatter, level: LogLevel.Warn);

            fileLogProvider.Level.Should().Be(LogLevel.Warn);
        }

        [Fact]
        public void Constructor2SetsTimeout()
        {
            var logFormatter = new Mock<ILogFormatter>().Object;
            var timeout = TimeSpan.FromMilliseconds(1234);

            var fileLogProvider = new RollingFileLogProvider(_logFilePath, logFormatter, timeout: timeout);

            fileLogProvider.Timeout.Should().Be(timeout);
        }

        [Fact]
        public void Constructor2SetsTimeoutToDefaultTimeoutWhenParameterIsNull()
        {
            var logFormatter = new Mock<ILogFormatter>().Object;
            var timeout = TimeSpan.FromMilliseconds(1234);

            var fileLogProvider = new RollingFileLogProvider(_logFilePath, logFormatter, timeout: null);

            fileLogProvider.Timeout.Should().Be(RollingFileLogProvider.DefaultTimeout);
        }

        [Fact]
        public void Constructor2SetsMaxFileSizeKilobytes()
        {
            var logFormatter = new Mock<ILogFormatter>().Object;

            var fileLogProvider = new RollingFileLogProvider(_logFilePath, logFormatter, maxFileSizeKilobytes: 1);

            fileLogProvider.MaxFileSizeBytes.Should().Be(1024);
        }

        [Fact]
        public void Constructor2SetsMaxArchiveCount()
        {
            var logFormatter = new Mock<ILogFormatter>().Object;

            var fileLogProvider = new RollingFileLogProvider(_logFilePath, logFormatter, maxArchiveCount: 3);

            fileLogProvider.MaxArchiveCount.Should().Be(3);
        }

        [Fact]
        public void Constructor2SetsRolloverPeriod()
        {
            var logFormatter = new Mock<ILogFormatter>().Object;

            var fileLogProvider = new RollingFileLogProvider(_logFilePath, logFormatter, rolloverPeriod: RolloverPeriod.Hourly);

            fileLogProvider.RolloverPeriod.Should().Be(RolloverPeriod.Hourly);
        }

        [Fact]
        public async Task WriteLineAsyncFormatsTheLogEntryAndWritesItToDisk()
        {
            var rollingFileLogProvider = new RollingFileLogProvider(_logFilePath, "{level}:{message}");

            var logEntry = new LogEntry("Hello, world!", LogLevel.Info);

            await rollingFileLogProvider.WriteAsync(logEntry);

            var output = File.ReadAllText(_logFilePath);

            output.Should().Be($"Info:Hello, world!{Environment.NewLine}");
        }

        [Fact]
        public async Task WriteAsyncCausesTheLogFileToBeArchivedWhenItsSizeGetsTooBig()
        {
            const int maxFileSizeKilobytes = 50;

            var rollingFileLogProvider =
                new RollingFileLogProvider(
                    _logFilePath,
                    new JsonLogFormatter(),
                    maxFileSizeKilobytes: maxFileSizeKilobytes);

            GetFileCount().Should().Be(0);

            await rollingFileLogProvider.WriteAsync(GetLogEntry());
            GetFileCount().Should().Be(1);

            await MakeOneArchiveFile(maxFileSizeKilobytes, rollingFileLogProvider);
            GetFileCount().Should().Be(2);
        }

        [Fact]
        public async Task WriteAsyncPrunesOldArchiveFiles()
        {
            const int maxFileSizeKilobytes = 50;

            var rollingFileLogProvider =
                new RollingFileLogProvider(
                    _logFilePath,
                    new JsonLogFormatter(),
                    maxFileSizeKilobytes: maxFileSizeKilobytes,
                    maxArchiveCount: 2);

            GetFileCount().Should().Be(0);

            await rollingFileLogProvider.WriteAsync(GetLogEntry());
            GetFileCount().Should().Be(1);

            await MakeOneArchiveFile(maxFileSizeKilobytes, rollingFileLogProvider);
            GetFileCount().Should().Be(2);

            await MakeOneArchiveFile(maxFileSizeKilobytes, rollingFileLogProvider);
            GetFileCount().Should().Be(3);

            await MakeOneArchiveFile(maxFileSizeKilobytes, rollingFileLogProvider);
            GetFileCount().Should().Be(3);

            await MakeOneArchiveFile(maxFileSizeKilobytes, rollingFileLogProvider);
            GetFileCount().Should().Be(3);
        }

        [Fact]
        public async Task WriteAsyncArchivesTheLogFileEveryHourOnTheHourWhenRolloverPeriodIsHourly()
        {
            RollingFileLogProvider rollingFileLogProvider = new TestingRollingFileLogProvider(
                _logFilePath,
                RolloverPeriod.Hourly,
                new TimeSet(new DateTime(2014, 10, 14, 11, 30, 0), new DateTime(2014, 10, 15, 13, 15, 0)),  // A) Different date, different hour.
                new TimeSet(new DateTime(2014, 10, 14, 11, 30, 0), new DateTime(2014, 10, 15, 11, 45, 0)),  // B) Different date, same hour.
                new TimeSet(new DateTime(2014, 10, 15, 11, 45, 0), new DateTime(2014, 10, 15, 13, 30, 0)),  // C) Same date, different hour.
                new TimeSet(new DateTime(2014, 10, 15, 13, 15, 0), new DateTime(2014, 10, 15, 13, 30, 0))); // D) Same date, same hour.

            GetFileCount().Should().Be(0);

            await rollingFileLogProvider.WriteAsync(GetLogEntry()); // New file, doesn't consume TimeSet
            GetFileCount().Should().Be(1);

            await rollingFileLogProvider.WriteAsync(GetLogEntry()); // A) Archive: Different date, different hour.
            GetFileCount().Should().Be(2);

            await rollingFileLogProvider.WriteAsync(GetLogEntry()); // B) Archive: Different date, same hour.
            GetFileCount().Should().Be(3);

            await rollingFileLogProvider.WriteAsync(GetLogEntry()); // C) Archive: Same date, different hour.
            GetFileCount().Should().Be(4);

            await rollingFileLogProvider.WriteAsync(GetLogEntry()); // D) No Archive: Same date, same hour.
            GetFileCount().Should().Be(4);
        }

        [Fact]
        public async Task WriteAsyncArchivesTheLogFileEveryDayAtMidnightWhenRolloverPeriodIsDaily()
        {
            RollingFileLogProvider rollingFileLogProvider = new TestingRollingFileLogProvider(
                _logFilePath,
                RolloverPeriod.Daily,
                new TimeSet(new DateTime(2014, 10, 14, 11, 30, 0), new DateTime(2014, 10, 15, 13, 15, 0)),  // A) Different date, different hour.
                new TimeSet(new DateTime(2014, 10, 14, 11, 30, 0), new DateTime(2014, 10, 15, 11, 45, 0)),  // B) Different date, same hour.
                new TimeSet(new DateTime(2014, 10, 15, 11, 45, 0), new DateTime(2014, 10, 15, 13, 30, 0)),  // C) Same date, different hour.
                new TimeSet(new DateTime(2014, 10, 15, 13, 15, 0), new DateTime(2014, 10, 15, 13, 30, 0))); // D) Same date, same hour.

            GetFileCount().Should().Be(0);

            await rollingFileLogProvider.WriteAsync(GetLogEntry()); // New file, doesn't consume TimeSet
            GetFileCount().Should().Be(1);

            await rollingFileLogProvider.WriteAsync(GetLogEntry()); // A) Archive: Different date, different hour.
            GetFileCount().Should().Be(2);

            await rollingFileLogProvider.WriteAsync(GetLogEntry()); // B) Archive: Different date, same hour.
            GetFileCount().Should().Be(3);

            await rollingFileLogProvider.WriteAsync(GetLogEntry()); // C) No Archive: Same date, different hour.
            GetFileCount().Should().Be(3);

            await rollingFileLogProvider.WriteAsync(GetLogEntry()); // D) No Archive: Same date, same hour.
            GetFileCount().Should().Be(3);
        }

        private static int GetFileCount()
        {
            var fileCount = Directory.GetFiles(_logFileDirectory).Length;
            return fileCount;
        }

        private static async Task MakeOneArchiveFile(int maxFileSizeKilobytes, RollingFileLogProvider rollingFileLogProvider)
        {
            var logEntry = GetLogEntry();

            // Write to the log file until it has exceeded the max file size.
            while (IsTooSmallForRollOver(maxFileSizeKilobytes))
            {
                await rollingFileLogProvider.WriteAsync(logEntry);
            }

            // This log entry should cause the existing log file to be archived, and a new one created.
            await rollingFileLogProvider.WriteAsync(logEntry);
        }

        private static LogEntry GetLogEntry()
        {
            try
            {
                throw new Exception("Oh noes!");
            }
            catch (Exception ex)
            {
                return new LogEntry("Hello, world!", ex, LogLevel.Info, new { Foo = "bar", Who = "there" });
            }
        }

        private static bool IsTooSmallForRollOver(double maxFileSizeMegabytes)
        {
            var fileInfo = new FileInfo(_logFilePath);

            return !fileInfo.Exists || (((double)fileInfo.Length) / 1024) < maxFileSizeMegabytes;
        }

        private class TestingRollingFileLogProvider : RollingFileLogProvider
        {
            public TestingRollingFileLogProvider(
                string file,
                RolloverPeriod rolloverPeriod,
                params TimeSet[] timeSets)
                : base(GetCurrentTimeFunc(timeSets), GetFileCreationTimeFunc(timeSets), file, new TemplateLogFormatter("{level}:{message}"), rolloverPeriod: rolloverPeriod)
            {
            }

            private static Func<DateTime> GetCurrentTimeFunc(TimeSet[] timeSets)
            {
                var index = 0;
                return () => timeSets[index++ % timeSets.Length].CurrentTime;
            }

            private static Func<FileInfo, DateTime> GetFileCreationTimeFunc(TimeSet[] timeSets)
            {
                var index = 0;
                return fileInfo => timeSets[index++ % timeSets.Length].CreationTime;
            }
        }

        private struct TimeSet
        {
            public TimeSet(DateTime creationTime, DateTime currentTime)
            {
                CreationTime = creationTime;
                CurrentTime = currentTime;
            }

            public DateTime CreationTime { get; }
            public DateTime CurrentTime { get; }
        }

        private class JsonLogFormatter : ILogFormatter
        {
            public string Format(LogEntry logEntry) => JsonConvert.SerializeObject(logEntry);
        }
    }
}
