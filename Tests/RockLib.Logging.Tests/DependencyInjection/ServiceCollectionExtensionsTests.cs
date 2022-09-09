﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RockLib.Logging.DependencyInjection;
using RockLib.Logging.LogProcessing;
using System;
using Xunit;

namespace RockLib.Logging.Tests.DependencyInjection;

public class ServiceCollectionExtensionsTests
{
    private readonly IServiceProvider _emptyServiceProvider = new ServiceCollection().BuildServiceProvider();

    [Fact(DisplayName = "AddLogger method 1 adds the correct service descriptors")]
    public void AddLoggerMethod1HappyPath()
    {
        var services = new ServiceCollection();
        var logProcessor = new Mock<ILogProcessor>().Object;

        var builder = services.AddLogger(logProcessor,
            configureOptions: options => options.Level = LogLevel.Info,
            lifetime: ServiceLifetime.Scoped);

        services.Should().HaveCount(3);

        services[0].ServiceType.Should().Be<ILogProcessor>();
        services[0].Lifetime.Should().Be(ServiceLifetime.Singleton);
        services[0].ImplementationInstance.Should().BeSameAs(logProcessor);

        services[1].ServiceType.Should().Be<ILogger>();
        services[1].Lifetime.Should().Be(ServiceLifetime.Scoped);
        services[1].ImplementationFactory.Target.Should().BeSameAs(builder);

        services[2].ServiceType.Should().Be<LoggerLookup>();
        services[2].Lifetime.Should().Be(ServiceLifetime.Scoped);
        services[2].ImplementationFactory.Should().NotBeNull();
    }

    [Fact(DisplayName = "AddLogger method 1 throws when services parameter is null")]
    public void AddLoggerMethod1SadPath1()
    {
        IServiceCollection services = null;
        var logProcessor = new Mock<ILogProcessor>().Object;

        Action act = () => services.AddLogger(logProcessor);

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*services*");
    }

    [Fact(DisplayName = "AddLogger method 1 throws when logProcessor parameter is null")]
    public void AddLoggerMethod1SadPath2()
    {
        var services = new ServiceCollection();
        ILogProcessor logProcessor = null;

        Action act = () => services.AddLogger(logProcessor);

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*logProcessor*");
    }

    [Fact(DisplayName = "AddLogger method 2 adds the correct service descriptors")]
    public void AddLoggerMethod2HappyPath()
    {
        var services = new ServiceCollection();
        var logProcessor = new Mock<ILogProcessor>().Object;
        ILogProcessor LogProcessorRegistration(IServiceProvider serviceProvider) => logProcessor;

        var builder = services.AddLogger(LogProcessorRegistration,
            configureOptions: options => options.Level = LogLevel.Info,
            lifetime: ServiceLifetime.Scoped);

        services.Should().HaveCount(3);

        services[0].ServiceType.Should().Be<ILogProcessor>();
        services[0].Lifetime.Should().Be(ServiceLifetime.Singleton);
        services[0].ImplementationFactory.Invoke(_emptyServiceProvider).Should().BeSameAs(logProcessor);

        services[1].ServiceType.Should().Be<ILogger>();
        services[1].Lifetime.Should().Be(ServiceLifetime.Scoped);
        services[1].ImplementationFactory.Target.Should().BeSameAs(builder);

        services[2].ServiceType.Should().Be<LoggerLookup>();
        services[2].Lifetime.Should().Be(ServiceLifetime.Scoped);
        services[2].ImplementationFactory.Should().NotBeNull();
    }

    [Fact(DisplayName = "AddLogger method 2 throws when services parameter is null")]
    public void AddLoggerMethod2SadPath1()
    {
        IServiceCollection services = null;
        var logProcessor = new Mock<ILogProcessor>().Object;
        ILogProcessor LogProcessorRegistration(IServiceProvider serviceProvider) => logProcessor;

        Action act = () => services.AddLogger(LogProcessorRegistration);

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*services*");
    }

    [Fact(DisplayName = "AddLogger method 2 throws when logProcessorRegistration parameter is null")]
    public void AddLoggerMethod2SadPath2()
    {
        var services = new ServiceCollection();
        Func<IServiceProvider, ILogProcessor> logProcessorRegistration = null;

        Action act = () => services.AddLogger(logProcessorRegistration);

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*logProcessorRegistration*");
    }

    [Fact(DisplayName = "AddLogger method 3 adds the correct service descriptors for ProcessingMode.Background")]
    public void AddLoggerMethod3HappyPath1()
    {
        var services = new ServiceCollection();

        var builder = services.AddLogger(configureOptions: options => options.Level = LogLevel.Info,
            processingMode: Logger.ProcessingMode.Background,
            lifetime: ServiceLifetime.Scoped);

        services.Should().HaveCount(3);

        services[0].ServiceType.Should().Be<ILogProcessor>();
        services[0].Lifetime.Should().Be(ServiceLifetime.Singleton);
        services[0].ImplementationType.Should().Be<BackgroundLogProcessor>();

        services[1].ServiceType.Should().Be<ILogger>();
        services[1].Lifetime.Should().Be(ServiceLifetime.Scoped);
        services[1].ImplementationFactory.Target.Should().BeSameAs(builder);

        services[2].ServiceType.Should().Be<LoggerLookup>();
        services[2].Lifetime.Should().Be(ServiceLifetime.Scoped);
        services[2].ImplementationFactory.Should().NotBeNull();
    }

    [Fact(DisplayName = "AddLogger method 3 adds the correct service descriptors for ProcessingMode.FireAndForget")]
    public void AddLoggerMethod3HappyPath2()
    {
        var services = new ServiceCollection();

        var builder = services.AddLogger(configureOptions: options => options.Level = LogLevel.Info,
            processingMode: Logger.ProcessingMode.FireAndForget,
            lifetime: ServiceLifetime.Scoped);

        services.Should().HaveCount(3);

        services[0].ServiceType.Should().Be<ILogProcessor>();
        services[0].Lifetime.Should().Be(ServiceLifetime.Singleton);
        services[0].ImplementationType.Should().Be<FireAndForgetLogProcessor>();

        services[1].ServiceType.Should().Be<ILogger>();
        services[1].Lifetime.Should().Be(ServiceLifetime.Scoped);
        services[1].ImplementationFactory.Target.Should().BeSameAs(builder);

        services[2].ServiceType.Should().Be<LoggerLookup>();
        services[2].Lifetime.Should().Be(ServiceLifetime.Scoped);
        services[2].ImplementationFactory.Should().NotBeNull();
    }

    [Fact(DisplayName = "AddLogger method 3 adds the correct service descriptors for ProcessingMode.Synchronous")]
    public void AddLoggerMethod3HappyPath3()
    {
        var services = new ServiceCollection();

        var builder = services.AddLogger(configureOptions: options => options.Level = LogLevel.Info,
            processingMode: Logger.ProcessingMode.Synchronous,
            lifetime: ServiceLifetime.Scoped);

        services.Should().HaveCount(3);

        services[0].ServiceType.Should().Be<ILogProcessor>();
        services[0].Lifetime.Should().Be(ServiceLifetime.Singleton);
        services[0].ImplementationType.Should().Be<SynchronousLogProcessor>();

        services[1].ServiceType.Should().Be<ILogger>();
        services[1].Lifetime.Should().Be(ServiceLifetime.Scoped);
        services[1].ImplementationFactory.Target.Should().BeSameAs(builder);

        services[2].ServiceType.Should().Be<LoggerLookup>();
        services[2].Lifetime.Should().Be(ServiceLifetime.Scoped);
        services[2].ImplementationFactory.Should().NotBeNull();
    }

    [Fact(DisplayName = "AddLogger method 3 throws when services parameter is null")]
    public void AddLoggerMethod3SadPath1()
    {
        IServiceCollection services = null;
        var processingMode = Logger.ProcessingMode.Background;

        Action act = () => services.AddLogger(processingMode: processingMode);

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*services*");
    }

    [Fact(DisplayName = "AddLogger method 3 throws when processingMode parameter is out of range")]
    public void AddLoggerMethod3SadPath2()
    {
        var services = new ServiceCollection();
        var processingMode = (Logger.ProcessingMode)123;
     
        Action act = () => services.AddLogger(processingMode: processingMode);

        act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithMessage("*processingMode*");
    }
}
