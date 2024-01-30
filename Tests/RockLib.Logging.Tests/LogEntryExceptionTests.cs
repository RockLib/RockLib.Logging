using System;
using System.IO;
using Xunit;

namespace RockLib.Logging.Tests;

public static class LogEntryExceptionTests
{
    private static readonly string _file = Path.GetRandomFileName();

    [Fact]
    public static void LogEntryInvokingConstructor1WithUndefinedLogLevelThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new LogEntry("", (LogLevel)(int.MinValue)));

    [Fact]
    public static void LogEntryInvokingConstructor2WithUndefinedLogLevelThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new LogEntry("", new NotSupportedException(), (LogLevel)(int.MinValue)));

    [Fact]
    public static void LogEntryPassingUndefinedLogLevelToLevelPropertyThrowsArgumentException()
    {
        var logEntry = new LogEntry("", LogLevel.Info);
        Assert.Throws<ArgumentException>(() => logEntry.Level = (LogLevel)(int.MinValue));
    }

    [Fact]
    public static void LogEntryInvokingConstructor1WithLogLevelNotSetThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new LogEntry("", LogLevel.NotSet));

    [Fact]
    public static void LogEntryInvokingConstructor2WithLogLevelNotSetThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new LogEntry("", new NotSupportedException(), LogLevel.NotSet));

    [Fact]
    public static void LogEntryPassingLogLevelNotSetToLevelPropertyThrowsArgumentException()
    {
        var logEntry = new LogEntry("", LogLevel.Info);
        Assert.Throws<ArgumentException>(() => logEntry.Level = LogLevel.NotSet);
    }

    [Fact]
    public static void LoggerInvokingConstructorWithUndefinedLogLevelThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Logger(level: (LogLevel)(int.MinValue)));

    [Fact]
    public static void LoggerFactoryCallingSetConfigurationWithNullConfigurationThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => LoggerFactory.SetConfiguration(null!));

    [Fact]
    public static void ConsoleLogProviderInvokingConstructor1WithUndefinedLogLevelThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new ConsoleLogProvider("", (LogLevel)(int.MinValue)));

    [Fact]
    public static void ConsoleLogProviderInvokingConstructor2WithUndefinedLogLevelThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new ConsoleLogProvider(new TemplateLogFormatter(""), (LogLevel)(int.MinValue)));

    [Fact]
    public static void ConsoleLogProviderInvokingConstructor1WithNegativeTimeoutThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new ConsoleLogProvider("", timeout: TimeSpan.FromSeconds(-1)));

    [Fact]
    public static void ConsoleLogProviderInvokingConstructor2WithNegativeTimeoutThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new ConsoleLogProvider(new TemplateLogFormatter(""), timeout: TimeSpan.FromSeconds(-1)));

    [Fact]
    public static void ConsoleLogProviderInvokingConstructor2WithNullLogFormatterThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new ConsoleLogProvider((ILogFormatter)null!));

    [Fact]
    public static void FileLogProviderInvokingConstructor1WithUndefinedLogLevelThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new FileLogProvider(_file, "", (LogLevel)(int.MinValue)));

    [Fact]
    public static void FileLogProviderInvokingConstructor2WithUndefinedLogLevelThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new FileLogProvider(_file, new TemplateLogFormatter(""), (LogLevel)(int.MinValue)));

    [Fact]
    public static void FileLogProviderInvokingConstructor1WithNegativeTimeoutThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new FileLogProvider(_file, "", timeout: TimeSpan.FromSeconds(-1)));

    [Fact]
    public static void FileLogProviderInvokingConstructor2WithNegativeTimeoutThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new FileLogProvider(_file, new TemplateLogFormatter(""), timeout: TimeSpan.FromSeconds(-1)));

    [Fact]
    public static void FileLogProviderInvokingConstructor1WithNullFileThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new FileLogProvider(null!, ""));

    [Fact]
    public static void FileLogProviderInvokingConstructor2WithNullFileThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new FileLogProvider(null!, new TemplateLogFormatter("")));

    [Fact]
    public static void FileLogProviderInvokingConstructor2WithNullLogFormatterThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new FileLogProvider(_file, (ILogFormatter)null!));

    [Fact]
    public static void RollingFileLogProviderInvokingConstructor1WithUndefinedLogLevelThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, "", (LogLevel)(int.MinValue)));

    [Fact]
    public static void RollingFileLogProviderInvokingConstructor2WithUndefinedLogLevelThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, new TemplateLogFormatter(""), (LogLevel)(int.MinValue)));

    [Fact]
    public static void RollingFileLogProviderInvokingConstructor1WithNegativeTimeoutThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, "", timeout: TimeSpan.FromSeconds(-1)));

    [Fact]
    public static void RollingFileLogProviderInvokingConstructor2WithNegativeTimeoutThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, new TemplateLogFormatter(""), timeout: TimeSpan.FromSeconds(-1)));

    [Fact]
    public static void RollingFileLogProviderInvokingConstructor1WithNullFileThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new RollingFileLogProvider(null!, ""));

    [Fact]
    public static void RollingFileLogProviderInvokingConstructor2WithNullFileThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new RollingFileLogProvider(null!, new TemplateLogFormatter("")));

    [Fact]
    public static void RollingFileLogProviderInvokingConstructor2WithNullLogFormatterThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new RollingFileLogProvider(_file, (ILogFormatter)null!));

    [Fact]
    public static void RollingFileLogProviderInvokingConstructor1WithNegativeMaxFileSizeKilobytesThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, "", maxFileSizeKilobytes: -1));

    [Fact]
    public static void RollingFileLogProviderInvokingConstructor2WithNegativeMaxFileSizeKilobytesThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, new TemplateLogFormatter(""), maxFileSizeKilobytes: -1));

    [Fact]
    public static void RollingFileLogProviderInvokingConstructor1WithTooLargeMaxFileSizeKilobytesThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, "", maxFileSizeKilobytes: (int.MaxValue / 1024) + 1));

    [Fact]
    public static void RollingFileLogProviderInvokingConstructor2WithTooLargeMaxFileSizeKilobytesThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, new TemplateLogFormatter(""), maxFileSizeKilobytes: (int.MaxValue / 1024) + 1));

    [Fact]
    public static void RollingFileLogProviderInvokingConstructor1WithNegativeMaxArchiveCountThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, "", maxArchiveCount: -1));

    [Fact]
    public static void RollingFileLogProviderInvokingConstructor2WithNegativeMaxArchiveCountThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, new TemplateLogFormatter(""), maxArchiveCount: -1));

    [Fact]
    public static void RollingFileLogProviderInvokingConstructor1WithUndefinedRolloverPeriodThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, "", rolloverPeriod: (RolloverPeriod)(-1)));

    [Fact]
    public static void RollingFileLogProviderInvokingConstructor2WithUndefinedRolloverPeriodThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new RollingFileLogProvider(_file, new TemplateLogFormatter(""), rolloverPeriod: (RolloverPeriod)(-1)));

    [Fact]
    public static void RollingFileLogProviderInvokingProtectedConstructorWithNullGetCurrentTimeThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new TestingRollingFileLogProvider(null!, f => f.CreationTimeUtc, _file, new TemplateLogFormatter("")));

    [Fact]
    public static void RollingFileLogProviderInvokingProtectedConstructorWithNullGetFileCreationTimeThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new TestingRollingFileLogProvider(() => DateTime.UtcNow, null!, _file, new TemplateLogFormatter("")));

    [Fact]
    public static void TemplateLogFormatterInvokingConstructorWithNullTemplateThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new TemplateLogFormatter(null!));

    private sealed class TestingRollingFileLogProvider : RollingFileLogProvider
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