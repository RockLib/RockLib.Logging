using System;
using System.IO;
using Xunit;

namespace RockLib.Logging.Tests
{
    public class ArgumentSadPathTests
    {
        public class LogEntry_
        {
            [Fact]
            public void InvokingConstructor1WithUndefinedLogLevelThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new LogEntry((LogLevel)(int.MinValue), ""));
            }

            [Fact]
            public void InvokingConstructor2WithUndefinedLogLevelThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new LogEntry((LogLevel)(int.MinValue), "", new Exception()));
            }

            [Fact]
            public void PassingUndefinedLogLevelToLevelPropertyThrowsArgumentException()
            {
                var logEntry = new LogEntry(LogLevel.Info, "");
                Assert.Throws<ArgumentException>(() => logEntry.Level = (LogLevel)(int.MinValue));
            }

            [Fact]
            public void InvokingConstructor1WithLogLevelNotSetThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new LogEntry(LogLevel.NotSet, ""));
            }

            [Fact]
            public void InvokingConstructor2WithLogLevelNotSetThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new LogEntry(LogLevel.NotSet, "", new Exception()));
            }

            [Fact]
            public void PassingLogLevelNotSetToLevelPropertyThrowsArgumentException()
            {
                var logEntry = new LogEntry(LogLevel.Info, "");
                Assert.Throws<ArgumentException>(() => logEntry.Level = LogLevel.NotSet);
            }
        }

        public class Logger_
        {
            [Fact]
            public void InvokingConstructorWithUndefinedLogLevelThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new Logger(level:(LogLevel)(int.MinValue)));
            }
        }

        public class LoggerFactory_
        {
            [Fact]
            public void CallingSetLoggersWithNullLoggersThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => LoggerFactory.SetLoggers(null));
            }
        }

        public class ConsoleLogProvider_
        {
            [Fact]
            public void InvokingConstructor1WithUndefinedLogLevelThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new ConsoleLogProvider("", (LogLevel)(int.MinValue)));
            }

            [Fact]
            public void InvokingConstructor2WithUndefinedLogLevelThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new ConsoleLogProvider(new TemplateLogFormatter(""), (LogLevel)(int.MinValue)));
            }

            [Fact]
            public void InvokingConstructor1WithNegativeTimeoutThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new ConsoleLogProvider("", timeout:TimeSpan.FromSeconds(-1)));
            }

            [Fact]
            public void InvokingConstructor2WithNegativeTimeoutThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new ConsoleLogProvider(new TemplateLogFormatter(""), timeout:TimeSpan.FromSeconds(-1)));
            }

            [Fact]
            public void InvokingConstructor2WithNullLogFormatterThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new ConsoleLogProvider((ILogFormatter)null));
            }
        }

        public class FileLogProvider_ : IDisposable
        {
            private readonly string _file = Path.GetRandomFileName();

            public void Dispose()
            {
                File.Delete(_file);
            }

            [Fact]
            public void InvokingConstructor1WithUndefinedLogLevelThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new FileLogProvider(_file, "", (LogLevel)(int.MinValue)));
            }

            [Fact]
            public void InvokingConstructor2WithUndefinedLogLevelThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new FileLogProvider(_file, new TemplateLogFormatter(""), (LogLevel)(int.MinValue)));
            }

            [Fact]
            public void InvokingConstructor1WithNegativeTimeoutThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new FileLogProvider(_file, "", timeout: TimeSpan.FromSeconds(-1)));
            }

            [Fact]
            public void InvokingConstructor2WithNegativeTimeoutThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new FileLogProvider(_file, new TemplateLogFormatter(""), timeout: TimeSpan.FromSeconds(-1)));
            }

            [Fact]
            public void InvokingConstructor1WithNullFileThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new FileLogProvider(null, ""));
            }

            [Fact]
            public void InvokingConstructor2WithNullFileThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new FileLogProvider(null, new TemplateLogFormatter("")));
            }

            [Fact]
            public void InvokingConstructor2WithNullLogFormatterThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new FileLogProvider(_file, (ILogFormatter)null));
            }
        }

        public class RollingFileLogProvider_ : IDisposable
        {
            private readonly string _file = Path.GetRandomFileName();

            public void Dispose()
            {
                File.Delete(_file);
            }

            [Fact]
            public void InvokingConstructor1WithUndefinedLogLevelThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, "", (LogLevel)(int.MinValue)));
            }

            [Fact]
            public void InvokingConstructor2WithUndefinedLogLevelThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, new TemplateLogFormatter(""), (LogLevel)(int.MinValue)));
            }

            [Fact]
            public void InvokingConstructor1WithNegativeTimeoutThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, "", timeout: TimeSpan.FromSeconds(-1)));
            }

            [Fact]
            public void InvokingConstructor2WithNegativeTimeoutThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, new TemplateLogFormatter(""), timeout: TimeSpan.FromSeconds(-1)));
            }

            [Fact]
            public void InvokingConstructor1WithNullFileThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new RollingFileLogProvider(null, ""));
            }

            [Fact]
            public void InvokingConstructor2WithNullFileThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new RollingFileLogProvider(null, new TemplateLogFormatter("")));
            }

            [Fact]
            public void InvokingConstructor2WithNullLogFormatterThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new RollingFileLogProvider(_file, (ILogFormatter)null));
            }

            [Fact]
            public void InvokingConstructor1WithNegativeMaxFileSizeKilobytesThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, "", maxFileSizeKilobytes:-1));
            }

            [Fact]
            public void InvokingConstructor2WithNegativeMaxFileSizeKilobytesThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, new TemplateLogFormatter(""), maxFileSizeKilobytes: -1));
            }

            [Fact]
            public void InvokingConstructor1WithTooLargeMaxFileSizeKilobytesThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, "", maxFileSizeKilobytes: (int.MaxValue / 1024) + 1));
            }

            [Fact]
            public void InvokingConstructor2WithTooLargeMaxFileSizeKilobytesThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, new TemplateLogFormatter(""), maxFileSizeKilobytes: (int.MaxValue / 1024) + 1));
            }

            [Fact]
            public void InvokingConstructor1WithNegativeMaxArchiveCountThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, "", maxArchiveCount: -1));
            }

            [Fact]
            public void InvokingConstructor2WithNegativeMaxArchiveCountThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, new TemplateLogFormatter(""), maxArchiveCount: -1));
            }

            [Fact]
            public void InvokingConstructor1WithUndefinedRolloverPeriodThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, "", rolloverPeriod: (RolloverPeriod)(-1)));
            }

            [Fact]
            public void InvokingConstructor2WithUndefinedRolloverPeriodThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, new TemplateLogFormatter(""), rolloverPeriod: (RolloverPeriod)(-1)));
            }

            [Fact]
            public void InvokingProtectedConstructorWithNullGetCurrentTimeThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new TestingRollingFileLogProvider(null, f => f.CreationTimeUtc, _file, new TemplateLogFormatter("")));
            }

            [Fact]
            public void InvokingProtectedConstructorWithNullGetFileCreationTimeThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new TestingRollingFileLogProvider(() => DateTime.UtcNow, null, _file, new TemplateLogFormatter("")));
            }

            private class TestingRollingFileLogProvider : RollingFileLogProvider
            {
                public TestingRollingFileLogProvider(
                    Func<DateTime> getCurrentTime,
                    Func<FileInfo, DateTime> getFileCreationTime,
                    string file,
                    ILogFormatter formatter)
                    : base(getCurrentTime, getFileCreationTime, file, formatter)
                {
                }
            }
        }

        public class TemplateLogFormatter_
        {
            [Fact]
            public void InvokingConstructorWithNullTemplateThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new TemplateLogFormatter(null));
            }
        }
    }
}
