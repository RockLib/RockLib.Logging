using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RockLib.Logging.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.Tests.DependencyInjection;

public static class LoggerBuilderExtensionsTests
{
    private static readonly IServiceProvider _emptyServiceProvider = new ServiceCollection().BuildServiceProvider();

    [Fact(DisplayName = "AddLogProvider method adds log provider of specified type")]
    public static void AddLogProviderMethod()
    {
        var builder = new TestLoggerBuilder();

        IDependency dependency = new ConcreteDependency();
        var setting = 123;

        var serviceProvider = new ServiceCollection()
            .AddSingleton(dependency)
            .BuildServiceProvider();

        builder.AddLogProvider<TestLogProvider>(setting);

        var registration =
            builder.LogProviderRegistrations.Should().ContainSingle()
            .Subject;

        var logProvider = registration.Invoke(serviceProvider);

        var testLogProvider =
            logProvider.Should().BeOfType<TestLogProvider>()
            .Subject;
        testLogProvider.Setting.Should().Be(setting);
        testLogProvider.Dependency.Should().BeSameAs(dependency);
    }

    [Fact(DisplayName = "AddLogProvider method throws when builder parameter is null")]
    public static void AddLogProviderMethodWhenBuilderIsNull()
    {
        Action act = () => (null as ILoggerBuilder)!.AddLogProvider<TestLogProvider>();

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*builder*");
    }

    [Fact(DisplayName = "AddContextProvider method adds context provider of specified type")]
    public static void AddContextProviderMethod()
    {
        var builder = new TestLoggerBuilder();

        IDependency dependency = new ConcreteDependency();
        var setting = 123;

        var serviceProvider = new ServiceCollection()
            .AddSingleton(dependency)
            .BuildServiceProvider();

        builder.AddContextProvider<TestContextProvider>(setting);

        var registration =
            builder.ContextProviderRegistrations.Should().ContainSingle()
            .Subject;

        var contextProvider = registration.Invoke(serviceProvider);

        var testContextProvider =
            contextProvider.Should().BeOfType<TestContextProvider>()
            .Subject;
        testContextProvider.Setting.Should().Be(setting);
        testContextProvider.Dependency.Should().BeSameAs(dependency);
    }

    [Fact(DisplayName = "AddContextProvider method throws when builder parameter is null")]
    public static void AddContextProviderMethodWhenBuilderIsNull()
    {
        Action act = () => (null as ILoggerBuilder)!.AddContextProvider<TestContextProvider>();

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*builder*");
    }

    #region AddConsoleLogProvider

    [Fact(DisplayName = "AddConsoleLogProvider method 1 adds console log provider with specified template")]
    public static void AddConsoleLogProviderMethod()
    {
        var builder = new TestLoggerBuilder();

        var template = "foobar";

        builder.AddConsoleLogProvider(template, LogLevel.Info);

        var registration =
            builder.LogProviderRegistrations.Should().ContainSingle()
            .Subject;

        var logProvider = registration.Invoke(_emptyServiceProvider);

        var consoleLogProvider =
            logProvider.Should().BeOfType<ConsoleLogProvider>()
            .Subject;

        consoleLogProvider.Level.Should().Be(LogLevel.Info);
        consoleLogProvider.Formatter.Should().BeOfType<TemplateLogFormatter>()
            .Which.Template.Should().Be(template);
    }

    [Fact(DisplayName = "AddConsoleLogProvider method 1 throws when template parameter is null")]
    public static void AddConsoleLogProviderMethodWhenTemplateIsNull()
    {
        var builder = new Mock<ILoggerBuilder>().Object;

        Action act = () => builder.AddConsoleLogProvider((string)null!, LogLevel.Info);

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*template*");
    }

    [Fact(DisplayName = "AddConsoleLogProvider method 2 adds console log provider with specified formatter")]
    public static void AddConsoleLogProviderMethodWithSpecifiedFormatter()
    {
        var builder = new TestLoggerBuilder();

        var formatter = new Mock<ILogFormatter>().Object;

        builder.AddConsoleLogProvider(formatter, LogLevel.Info);

        var registration =
            builder.LogProviderRegistrations.Should().ContainSingle()
            .Subject;

        var logProvider = registration.Invoke(_emptyServiceProvider);

        var consoleLogProvider =
            logProvider.Should().BeOfType<ConsoleLogProvider>()
            .Subject;

        consoleLogProvider.Level.Should().Be(LogLevel.Info);
        consoleLogProvider.Formatter.Should().BeSameAs(formatter);
    }

    [Fact(DisplayName = "AddConsoleLogProvider method 2 throws when formatter parameter is null")]
    public static void AddConsoleLogProviderMethodWhenFormatterIsNull()
    {
        var builder = new Mock<ILoggerBuilder>().Object;

        Action act = () => builder.AddConsoleLogProvider((ILogFormatter)null!, LogLevel.Info);

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*formatter*");
    }

    [Fact(DisplayName = "AddConsoleLogProvider method 3 adds console log provider with specified formatter")]
    public static void AddConsoleLogProviderMethodWithTestFormatter()
    {
        var builder = new TestLoggerBuilder();

        IDependency dependency = new ConcreteDependency();
        var setting = 123;

        var serviceProvider = new ServiceCollection()
            .AddSingleton(dependency)
            .BuildServiceProvider();

        builder.AddConsoleLogProvider<TestLogFormatter>(LogLevel.Info, logFormatterParameters: setting);

        var registration =
            builder.LogProviderRegistrations.Should().ContainSingle()
            .Subject;

        var logProvider = registration.Invoke(serviceProvider);

        var consoleLogProvider =
            logProvider.Should().BeOfType<ConsoleLogProvider>()
            .Subject;

        consoleLogProvider.Level.Should().Be(LogLevel.Info);
        
        var formatter =
            consoleLogProvider.Formatter.Should().BeOfType<TestLogFormatter>()
            .Subject;
        
        formatter.Dependency.Should().BeSameAs(dependency);
        formatter.Setting.Should().Be(setting);
    }

    [Fact(DisplayName = "AddConsoleLogProvider method 4 adds console log provider with specified formatter registration")]
    public static void AddConsoleLogProviderMethodWithFormatterRegistrations()
    {
        var builder = new TestLoggerBuilder();

        var formatter = new Mock<ILogFormatter>().Object;
        ILogFormatter FormatterRegistration(IServiceProvider sp) => formatter;

        builder.AddConsoleLogProvider(FormatterRegistration, LogLevel.Info);

        var registration =
            builder.LogProviderRegistrations.Should().ContainSingle()
            .Subject;

        var logProvider = registration.Invoke(_emptyServiceProvider);

        var consoleLogProvider =
            logProvider.Should().BeOfType<ConsoleLogProvider>()
            .Subject;

        consoleLogProvider.Level.Should().Be(LogLevel.Info);
        consoleLogProvider.Formatter.Should().BeSameAs(formatter);
    }

    [Fact(DisplayName = "AddConsoleLogProvider method 4 throws when formatterRegistration parameter is null")]
    public static void AddConsoleLogProviderMethodWithFormatterRegistrationsWhenFormatterIsNull()
    {
        var builder = new Mock<ILoggerBuilder>().Object;

        Action act = () => builder.AddConsoleLogProvider((Func<IServiceProvider, ILogFormatter>)null!, LogLevel.Info);

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*formatterRegistration*");
    }

    [Fact(DisplayName = "AddConsoleLogProvider method 5 adds console log provider configured with configureOptions parameter")]
    public static void AddConsoleLogProviderMethodWithOptions()
    {
        var builder = new TestLoggerBuilder();

        var formatter = new Mock<ILogFormatter>().Object;

        var serviceCollection = new ServiceCollection();
        serviceCollection.Configure<ConsoleLogProviderOptions>(options => options.Level = LogLevel.Info);

        builder.AddConsoleLogProvider(options => options.SetFormatter(formatter));

        var registration =
            builder.LogProviderRegistrations.Should().ContainSingle()
            .Subject;

        var logProvider = registration.Invoke(serviceCollection.BuildServiceProvider());

        var consoleLogProvider =
            logProvider.Should().BeOfType<ConsoleLogProvider>()
            .Subject;

        consoleLogProvider.Level.Should().Be(LogLevel.Info);
        consoleLogProvider.Formatter.Should().BeSameAs(formatter);
    }

    [Fact(DisplayName = "AddConsoleLogProvider method 5 throws when builder parameter is null")]
    public static void AddConsoleLogProviderMethodWithOptionsWhenBuilderIsNull()
    {
        Action act = () => (null as ILoggerBuilder)!.AddConsoleLogProvider();

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*builder*");
    }

    #endregion

    #region AddFileLogProvider

    [Fact(DisplayName = "AddFileLogProvider method 1 adds file log provider with specified template")]
    public static void AddFileLogProviderMethod()
    {
        var builder = new TestLoggerBuilder();

        var template = "foobar";
        var file = "c:\\foobar";

        builder.AddFileLogProvider(template, file, LogLevel.Info);

        var registration =
            builder.LogProviderRegistrations.Should().ContainSingle()
            .Subject;

        var logProvider = registration.Invoke(_emptyServiceProvider);

        var fileLogProvider =
            logProvider.Should().BeOfType<FileLogProvider>()
            .Subject;

        fileLogProvider.Level.Should().Be(LogLevel.Info);
        fileLogProvider.File.Should().Be(file);
        fileLogProvider.Formatter.Should().BeOfType<TemplateLogFormatter>()
            .Which.Template.Should().Be(template);
    }

    [Fact(DisplayName = "AddFileLogProvider method 1 throws when template parameter is null")]
    public static void AddFileLogProviderMethodWhenTemplateIsNull()
    {
        var builder = new Mock<ILoggerBuilder>().Object;

        Action act = () => builder.AddFileLogProvider((string)null!, "c:\\foobar", LogLevel.Info);

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*template*");
    }

    [Fact(DisplayName = "AddFileLogProvider method 2 adds file log provider with specified formatter")]
    public static void AddFileLogProviderMethodWithSpecifiedParameter()
    {
        var builder = new TestLoggerBuilder();

        var formatter = new Mock<ILogFormatter>().Object;
        var file = "c:\\foobar";

        builder.AddFileLogProvider(formatter, file, LogLevel.Info);

        var registration =
            builder.LogProviderRegistrations.Should().ContainSingle()
            .Subject;

        var logProvider = registration.Invoke(_emptyServiceProvider);

        var fileLogProvider =
            logProvider.Should().BeOfType<FileLogProvider>()
            .Subject;

        fileLogProvider.Level.Should().Be(LogLevel.Info);
        fileLogProvider.File.Should().Be(file);
        fileLogProvider.Formatter.Should().BeSameAs(formatter);
    }

    [Fact(DisplayName = "AddFileLogProvider method 2 throws when formatter parameter is null")]
    public static void AddFileLogProviderMethodWithSpecifiedParameterWhenFormatterIsNull()
    {
        var builder = new Mock<ILoggerBuilder>().Object;

        Action act = () => builder.AddFileLogProvider((ILogFormatter)null!, "c:\\foobar", LogLevel.Info);

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*formatter*");
    }

    [Fact(DisplayName = "AddFileLogProvider method 3 adds file log provider with specified formatter")]
    public static void AddFileLogProviderMethodWithSpecifiedFormatter()
    {
        var builder = new TestLoggerBuilder();

        IDependency dependency = new ConcreteDependency();
        var setting = 123;
        var file = "c:\\foobar";

        var serviceProvider = new ServiceCollection()
            .AddSingleton(dependency)
            .BuildServiceProvider();

        builder.AddFileLogProvider<TestLogFormatter>(file, LogLevel.Info, logFormatterParameters: setting);

        var registration =
            builder.LogProviderRegistrations.Should().ContainSingle()
            .Subject;

        var logProvider = registration.Invoke(serviceProvider);

        var fileLogProvider =
            logProvider.Should().BeOfType<FileLogProvider>()
            .Subject;

        fileLogProvider.Level.Should().Be(LogLevel.Info);
        fileLogProvider.File.Should().Be(file);
        
        var formatter =
            fileLogProvider.Formatter.Should().BeOfType<TestLogFormatter>()
            .Subject;
        
        formatter.Dependency.Should().BeSameAs(dependency);
        formatter.Setting.Should().Be(setting);
    }

    [Fact(DisplayName = "AddFileLogProvider method 4 adds file log provider with specified formatter registration")]
    public static void AddFileLogProviderMethodWithSpecifiedFormatterRegistration()
    {
        var builder = new TestLoggerBuilder();

        var formatter = new Mock<ILogFormatter>().Object;
        ILogFormatter FormatterRegistration(IServiceProvider sp) => formatter;
        var file = "c:\\foobar";

        builder.AddFileLogProvider(FormatterRegistration, file, LogLevel.Info);

        var registration =
            builder.LogProviderRegistrations.Should().ContainSingle()
            .Subject;

        var logProvider = registration.Invoke(_emptyServiceProvider);

        var fileLogProvider =
            logProvider.Should().BeOfType<FileLogProvider>()
            .Subject;

        fileLogProvider.Level.Should().Be(LogLevel.Info);
        fileLogProvider.File.Should().Be(file);
        fileLogProvider.Formatter.Should().BeSameAs(formatter);
    }

    [Fact(DisplayName = "AddFileLogProvider method 4 throws when formatterRegistration parameter is null")]
    public static void AddFileLogProviderMethodWithSpecifiedFormatterRegistrationWhenFormatterIsNull()
    {
        var builder = new Mock<ILoggerBuilder>().Object;

        Action act = () => builder.AddFileLogProvider((Func<IServiceProvider, ILogFormatter>)null!, "c:\\foobar", LogLevel.Info);

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*formatterRegistration*");
    }

    [Fact(DisplayName = "AddFileLogProvider method 5 adds file log provider configured with configureOptions parameter")]
    public static void AddFileLogProviderMethodWithOptions()
    {
        var builder = new TestLoggerBuilder();

        var formatter = new Mock<ILogFormatter>().Object;
        var file = "c:\\foobar";

        var serviceCollection = new ServiceCollection();
        serviceCollection.Configure<FileLogProviderOptions>(options => options.Level = LogLevel.Info);

        builder.AddFileLogProvider(options =>
        {
            options.File = file;
            options.SetFormatter(formatter);
        });

        var registration =
            builder.LogProviderRegistrations.Should().ContainSingle()
            .Subject;

        var logProvider = registration.Invoke(serviceCollection.BuildServiceProvider());

        var fileLogProvider =
            logProvider.Should().BeOfType<FileLogProvider>()
            .Subject;

        fileLogProvider.Level.Should().Be(LogLevel.Info);
        fileLogProvider.File.Should().Be(file);
        fileLogProvider.Formatter.Should().BeSameAs(formatter);
    }

    [Fact(DisplayName = "AddFileLogProvider method 5 throws when builder parameter is null")]
    public static void AddFileLogProviderMethodWithOptionsWhenBuilderIsNull()
    {
        var act = () => (null as ILoggerBuilder)!.AddFileLogProvider();

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*builder*");
    }

    #endregion

    #region AddRollingFileLogProvider

    [Fact(DisplayName = "AddRollingFileLogProvider method 1 adds rolling file log provider with specified template")]
    public static void AddRollingRollingFileLogProviderMethodWithTemplate()
    {
        var builder = new TestLoggerBuilder();

        var template = "foobar";
        var file = "c:\\foobar";

        builder.AddRollingFileLogProvider(template, file, LogLevel.Info,
            maxFileSizeKilobytes: 123, maxArchiveCount: 456, rolloverPeriod: RolloverPeriod.Hourly);

        var registration =
            builder.LogProviderRegistrations.Should().ContainSingle()
            .Subject;

        var logProvider = registration.Invoke(_emptyServiceProvider);

        var rollingFileLogProvider =
            logProvider.Should().BeOfType<RollingFileLogProvider>()
            .Subject;

        rollingFileLogProvider.Level.Should().Be(LogLevel.Info);
        rollingFileLogProvider.File.Should().Be(file);
        rollingFileLogProvider.MaxFileSizeBytes.Should().Be(123 * 1024);
        rollingFileLogProvider.MaxArchiveCount.Should().Be(456);
        rollingFileLogProvider.RolloverPeriod.Should().Be(RolloverPeriod.Hourly);
        rollingFileLogProvider.Formatter.Should().BeOfType<TemplateLogFormatter>()
            .Which.Template.Should().Be(template);
    }

    [Fact(DisplayName = "AddRollingFileLogProvider method 1 throws when template parameter is null")]
    public static void AddRollingRollingFileLogProviderMethodWithTemplateWhenNameIsNull()
    {
        var builder = new Mock<ILoggerBuilder>().Object;

        Action act = () => builder.AddRollingFileLogProvider((string)null!, "c:\\foobar", LogLevel.Info,
            maxFileSizeKilobytes: 123, maxArchiveCount: 456, rolloverPeriod: RolloverPeriod.Hourly);

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*template*");
    }

    [Fact(DisplayName = "AddRollingFileLogProvider method 2 adds rolling file log provider with specified formatter")]
    public static void AddRollingFileLogProviderMethodWithSpecifiedFormatter()
    {
        var builder = new TestLoggerBuilder();

        var formatter = new Mock<ILogFormatter>().Object;
        var file = "c:\\foobar";

        builder.AddRollingFileLogProvider(formatter, file, LogLevel.Info,
            maxFileSizeKilobytes: 123, maxArchiveCount: 456, rolloverPeriod: RolloverPeriod.Hourly);

        var registration =
            builder.LogProviderRegistrations.Should().ContainSingle()
            .Subject;

        var logProvider = registration.Invoke(_emptyServiceProvider);

        var rollingFileLogProvider =
            logProvider.Should().BeOfType<RollingFileLogProvider>()
            .Subject;

        rollingFileLogProvider.Level.Should().Be(LogLevel.Info);
        rollingFileLogProvider.File.Should().Be(file);
        rollingFileLogProvider.MaxFileSizeBytes.Should().Be(123 * 1024);
        rollingFileLogProvider.MaxArchiveCount.Should().Be(456);
        rollingFileLogProvider.RolloverPeriod.Should().Be(RolloverPeriod.Hourly);
        rollingFileLogProvider.Formatter.Should().BeSameAs(formatter);
    }

    [Fact(DisplayName = "AddRollingFileLogProvider method 2 throws when formatter parameter is null")]
    public static void AddRollingFileLogProviderMethodWithSpecifiedFormatterWhenFormatterIsNull()
    {
        var builder = new Mock<ILoggerBuilder>().Object;

        Action act = () => builder.AddRollingFileLogProvider((ILogFormatter)null!, "c:\\foobar", LogLevel.Info,
            maxFileSizeKilobytes: 123, maxArchiveCount: 456, rolloverPeriod: RolloverPeriod.Hourly);

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*formatter*");
    }

    [Fact(DisplayName = "AddRollingFileLogProvider method 3 adds rolling file log provider with specified formatter")]
    public static void AddRollingFileLogProviderMethodWithFormatter()
    {
        var builder = new TestLoggerBuilder();

        IDependency dependency = new ConcreteDependency();
        var setting = 123;
        var file = "c:\\foobar";

        var serviceProvider = new ServiceCollection()
            .AddSingleton(dependency)
            .BuildServiceProvider();

        builder.AddRollingFileLogProvider<TestLogFormatter>(file, LogLevel.Info,
            maxFileSizeKilobytes: 123, maxArchiveCount: 456, rolloverPeriod: RolloverPeriod.Hourly,
            logFormatterParameters: setting);

        var registration =
            builder.LogProviderRegistrations.Should().ContainSingle()
            .Subject;

        var logProvider = registration.Invoke(serviceProvider);

        var rollingFileLogProvider =
            logProvider.Should().BeOfType<RollingFileLogProvider>()
            .Subject;

        rollingFileLogProvider.Level.Should().Be(LogLevel.Info);
        rollingFileLogProvider.File.Should().Be(file);
        rollingFileLogProvider.MaxFileSizeBytes.Should().Be(123 * 1024);
        rollingFileLogProvider.MaxArchiveCount.Should().Be(456);
        rollingFileLogProvider.RolloverPeriod.Should().Be(RolloverPeriod.Hourly);
        
        var formatter =
            rollingFileLogProvider.Formatter.Should().BeOfType<TestLogFormatter>()
            .Subject;
        
        formatter.Dependency.Should().BeSameAs(dependency);
        formatter.Setting.Should().Be(setting);
    }

    [Fact(DisplayName = "AddRollingFileLogProvider method 4 adds rolling file log provider with specified formatter registration")]
    public static void AddRollingFileLogProviderMethodWithFormatterRegistration()
    {
        var builder = new TestLoggerBuilder();

        var formatter = new Mock<ILogFormatter>().Object;
        ILogFormatter FormatterRegistration(IServiceProvider sp) => formatter;
        var file = "c:\\foobar";

        builder.AddRollingFileLogProvider(FormatterRegistration, file, LogLevel.Info,
            maxFileSizeKilobytes: 123, maxArchiveCount: 456, rolloverPeriod: RolloverPeriod.Hourly);

        var registration =
            builder.LogProviderRegistrations.Should().ContainSingle()
            .Subject;

        var logProvider = registration.Invoke(_emptyServiceProvider);

        var rollingFileLogProvider =
            logProvider.Should().BeOfType<RollingFileLogProvider>()
            .Subject;

        rollingFileLogProvider.Level.Should().Be(LogLevel.Info);
        rollingFileLogProvider.File.Should().Be(file);
        rollingFileLogProvider.MaxFileSizeBytes.Should().Be(123 * 1024);
        rollingFileLogProvider.MaxArchiveCount.Should().Be(456);
        rollingFileLogProvider.RolloverPeriod.Should().Be(RolloverPeriod.Hourly);
        rollingFileLogProvider.Formatter.Should().BeSameAs(formatter);
    }

    [Fact(DisplayName = "AddRollingFileLogProvider method 4 throws when formatterRegistration parameter is null")]
    public static void AddRollingFileLogProviderMethodWithFormatterRegistrationWhenRegistrationIsNull()
    {
        var builder = new Mock<ILoggerBuilder>().Object;

        Action act = () => builder.AddRollingFileLogProvider((Func<IServiceProvider, ILogFormatter>)null!, "c:\\foobar", LogLevel.Info,
            maxFileSizeKilobytes: 123, maxArchiveCount: 456, rolloverPeriod: RolloverPeriod.Hourly);

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*formatterRegistration*");
    }

    [Fact(DisplayName = "AddRollingFileLogProvider method 5 adds rolling file log provider configured with configureOptions parameter")]
    public static void AddRollingFileLogProviderMethodWithOptions()
    {
        var builder = new TestLoggerBuilder();

        var formatter = new Mock<ILogFormatter>().Object;
        var file = "c:\\foobar";

        var serviceCollection = new ServiceCollection();
        serviceCollection.Configure<RollingFileLogProviderOptions>(options => options.Level = LogLevel.Info);

        builder.AddRollingFileLogProvider(options =>
        {
            options.File = file;
            options.MaxFileSizeKilobytes = 123;
            options.MaxArchiveCount = 456;
            options.RolloverPeriod = RolloverPeriod.Hourly;
            options.SetFormatter(formatter);
        });

        var registration =
            builder.LogProviderRegistrations.Should().ContainSingle()
            .Subject;

        var logProvider = registration.Invoke(serviceCollection.BuildServiceProvider());

        var rollingFileLogProvider =
            logProvider.Should().BeOfType<RollingFileLogProvider>()
            .Subject;

        rollingFileLogProvider.Level.Should().Be(LogLevel.Info);
        rollingFileLogProvider.File.Should().Be(file);
        rollingFileLogProvider.MaxFileSizeBytes.Should().Be(123 * 1024);
        rollingFileLogProvider.MaxArchiveCount.Should().Be(456);
        rollingFileLogProvider.RolloverPeriod.Should().Be(RolloverPeriod.Hourly);
        rollingFileLogProvider.Formatter.Should().BeSameAs(formatter);
    }

    [Fact(DisplayName = "AddRollingFileLogProvider method 5 throws when builder parameter is null")]
    public static void AddRollingFileLogProviderMethodWithOptionsWhenBuilderIsNull()
    {
        var act = () => (null as ILoggerBuilder)!.AddRollingFileLogProvider();

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*builder*");
    }

    #endregion

    private sealed class TestLoggerBuilder : ILoggerBuilder
    {
        public string LoggerName => Logger.DefaultName;

        public List<Func<IServiceProvider, ILogProvider>> LogProviderRegistrations { get; } = new List<Func<IServiceProvider, ILogProvider>>();

        public List<Func<IServiceProvider, IContextProvider>> ContextProviderRegistrations { get; } = new List<Func<IServiceProvider, IContextProvider>>();

        public ILoggerBuilder AddLogProvider(Func<IServiceProvider, ILogProvider> logProviderRegistration)
        {
            LogProviderRegistrations.Add(logProviderRegistration);
            return this;
        }

        public ILoggerBuilder AddContextProvider(Func<IServiceProvider, IContextProvider> contextProviderRegistration)
        {
            ContextProviderRegistrations.Add(contextProviderRegistration);
            return this;
        }
    }

#pragma warning disable CA1812
    private sealed class TestLogProvider : ILogProvider
    {
        public TestLogProvider(IDependency dependency, int setting) => (Dependency, Setting) = (dependency, setting);
        public IDependency Dependency { get; }
        public int Setting { get; }
        public TimeSpan Timeout => throw new NotImplementedException();
        public LogLevel Level => throw new NotImplementedException();
        public Task WriteAsync(LogEntry logEntry, CancellationToken cancellationToken) => throw new NotImplementedException();
    }

    private sealed class TestContextProvider : IContextProvider
    {
        public TestContextProvider(IDependency dependency, int setting) => (Dependency, Setting) = (dependency, setting);
        public IDependency Dependency { get; }
        public int Setting { get; }
        public void AddContext(LogEntry logEntry) => throw new NotImplementedException();
    }

    private sealed class TestLogFormatter : ILogFormatter
    {
        public TestLogFormatter(IDependency dependency, int setting) => (Dependency, Setting) = (dependency, setting);
        public IDependency Dependency { get; }
        public int Setting { get; }
        public string Format(LogEntry logEntry) => throw new NotImplementedException();
    }
#pragma warning restore CA1812

    private interface IDependency
    {
    }

    private sealed class ConcreteDependency : IDependency
    {
    }
}
