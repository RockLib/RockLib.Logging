﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RockLib.Logging.DependencyInjection;
using RockLib.Logging.Moq;
using System;
using Xunit;

namespace RockLib.Logging.Microsoft.Extensions.Tests;

public static class RockLibLoggerProviderExtensionsTests
{
    [Fact(DisplayName = "AddRockLibLoggerProvider extension method 1 adds a singleton RockLibLoggerProvider factory")]
    public static void AddRockLibLoggerProviderExtensionMethod1HappyPath1()
    {
        var services = new ServiceCollection();
        var mockLoggerBuilder = new Mock<ILoggingBuilder>();
        mockLoggerBuilder.Setup(m => m.Services).Returns(services);

        mockLoggerBuilder.Object.AddRockLibLoggerProvider(options => { });

        var descriptor =
            services.Should().ContainSingle(d => d.ServiceType == typeof(ILoggerProvider))
                .Subject;

        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        descriptor.ImplementationFactory.Should().NotBeNull();
        descriptor.ImplementationInstance.Should().BeNull();
        descriptor.ImplementationType.Should().BeNull();

        // In order to test the description.ImplementationFactory function, we need be able to
        // provide its required dependencies with its IServiceProvider parameter. We can
        // create such a provider by adding a LoggerLookup delegate to the service collection
        // and building it.
        ILogger? capturedLogger = null;
        LoggerLookup loggerLookup = name => capturedLogger = new MockLogger(name: name).Object;
        services.AddSingleton(loggerLookup);
        var serviceProvider = services.BuildServiceProvider();

        var provider = descriptor!.ImplementationFactory!(serviceProvider);

        provider.Should().BeOfType<RockLibLoggerProvider>()
            .Which.Logger.Should().BeSameAs(capturedLogger).And.NotBeNull();
    }

    [Fact(DisplayName = "AddRockLibLoggerProvider extension method 1 configures the configureOptions parameter")]
    public static void AddRockLibLoggerProviderExtensionMethod1HappyPath2()
    {
        RockLibLoggerOptions? capturedOptions = null;

        var services = new ServiceCollection();
        var mockLoggerBuilder = new Mock<ILoggingBuilder>();
        mockLoggerBuilder.Setup(m => m.Services).Returns(services);

        mockLoggerBuilder.Object.AddRockLibLoggerProvider(opt => capturedOptions = opt);

        var descriptor =
            services.Should().ContainSingle(d => d.ServiceType == typeof(IConfigureOptions<RockLibLoggerOptions>))
                .Subject;

        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        descriptor.ImplementationInstance.Should().NotBeNull();
        descriptor.ImplementationFactory.Should().BeNull();
        descriptor.ImplementationType.Should().BeNull();

        var configureOptions =
            descriptor.ImplementationInstance.Should().BeAssignableTo<IConfigureOptions<RockLibLoggerOptions>>()
                .Subject;

        var options = new RockLibLoggerOptions();

        configureOptions.Configure(options);

        capturedOptions.Should().BeSameAs(options);
    }

    [Fact(DisplayName = "AddRockLibLoggerProvider extension method 1 returns the loggingBuilder parameter")]
    public static void AddRockLibLoggerProviderExtensionMethod1HappyPath3()
    {
        var services = new ServiceCollection();
        var mockLoggerBuilder = new Mock<ILoggingBuilder>();
        mockLoggerBuilder.Setup(m => m.Services).Returns(services);

        var returnedLoggingBuilder = mockLoggerBuilder.Object.AddRockLibLoggerProvider(options => { });

        returnedLoggingBuilder.Should().BeSameAs(mockLoggerBuilder.Object);
    }

    [Fact(DisplayName = "AddRockLibLoggerProvider extension method 1 throws when loggingBuilder parameter is null")]
    public static void AddRockLibLoggerProviderExtensionMethod1SadPath()
    {
        var act = () => (null as ILoggingBuilder)!.AddRockLibLoggerProvider(options => { });

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*builder*");
    }

    [Fact(DisplayName = "AddRockLibLoggerProvider extension method 2 adds a singleton RockLibLoggerProvider factory")]
    public static void AddRockLibLoggerProviderExtensionMethod2HappyPath1()
    {
        var services = new ServiceCollection();
        var mockLoggerBuilder = new Mock<ILoggingBuilder>();
        mockLoggerBuilder.Setup(m => m.Services).Returns(services);

        mockLoggerBuilder.Object.AddRockLibLoggerProvider("MyLogger");

        services.Should().ContainSingle();

        var descriptor = services[0];

        descriptor.ServiceType.Should().Be<ILoggerProvider>();
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        descriptor.ImplementationFactory.Should().NotBeNull();
        descriptor.ImplementationInstance.Should().BeNull();
        descriptor.ImplementationType.Should().BeNull();

        // In order to test the description.ImplementationFactory function, we need be able to
        // provide its required dependencies with its IServiceProvider parameter. We can
        // create such a provider by adding a LoggerLookup delegate to the service collection
        // and building it.
        ILogger? capturedLogger = null;
        LoggerLookup loggerLookup = name => capturedLogger = new MockLogger(name: name).Object;
        services.AddSingleton(loggerLookup);
        var serviceProvider = services.BuildServiceProvider();

        var provider = descriptor!.ImplementationFactory!(serviceProvider);

        provider.Should().BeOfType<RockLibLoggerProvider>()
            .Which.Logger.Should().BeSameAs(capturedLogger).And.NotBeNull();
    }

    [Fact(DisplayName = "AddRockLibLoggerProvider extension method 2 configures the configureOptions parameter")]
    public static void AddRockLibLoggerProviderExtensionMethod2HappyPath2()
    {
        RockLibLoggerOptions? capturedOptions = null;

        var services = new ServiceCollection();
        var mockLoggerBuilder = new Mock<ILoggingBuilder>();
        mockLoggerBuilder.Setup(m => m.Services).Returns(services);

        mockLoggerBuilder.Object.AddRockLibLoggerProvider("MyLogger", opt => capturedOptions = opt);

        var descriptor =
            services.Should().ContainSingle(d => d.ServiceType == typeof(IConfigureOptions<RockLibLoggerOptions>))
                .Subject;

        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        descriptor.ImplementationInstance.Should().NotBeNull();
        descriptor.ImplementationFactory.Should().BeNull();
        descriptor.ImplementationType.Should().BeNull();

        var configureOptions =
            descriptor.ImplementationInstance.Should().BeAssignableTo<IConfigureOptions<RockLibLoggerOptions>>()
                .Subject;

        var options = new RockLibLoggerOptions();

        configureOptions.Configure(options);

        capturedOptions.Should().BeSameAs(options);
    }

    [Fact(DisplayName = "AddRockLibLoggerProvider extension method 2 returns the loggingBuilder parameter")]
    public static void AddRockLibLoggerProviderExtensionMethod2HappyPath3()
    {
        var services = new ServiceCollection();
        var mockLoggerBuilder = new Mock<ILoggingBuilder>();
        mockLoggerBuilder.Setup(m => m.Services).Returns(services);

        var returnedLoggingBuilder = mockLoggerBuilder.Object.AddRockLibLoggerProvider("MyLogger");

        returnedLoggingBuilder.Should().BeSameAs(mockLoggerBuilder.Object);
    }

    [Fact(DisplayName = "AddRockLibLoggerProvider extension method 2 throws when loggingBuilder parameter is null")]
    public static void AddRockLibLoggerProviderExtensionMethod2SadPath()
    {
        var act = () => (null as ILoggingBuilder)!.AddRockLibLoggerProvider("MyLogger");

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*builder*");
    }

    [Fact(DisplayName = "AddRockLibLoggerProvider extension method 3 adds a singleton RockLibLoggerProvider factory")]
    public static void AddRockLibLoggerProviderExtensionMethod3HappyPath1()
    {
        var services = new ServiceCollection();

        services.AddRockLibLoggerProvider(options => { });

        var descriptor = 
            services.Should().ContainSingle(d => d.ServiceType == typeof(ILoggerProvider))
            .Subject;

        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        descriptor.ImplementationFactory.Should().NotBeNull();
        descriptor.ImplementationInstance.Should().BeNull();
        descriptor.ImplementationType.Should().BeNull();

        // In order to test the description.ImplementationFactory function, we need be able to
        // provide its required dependencies with its IServiceProvider parameter. We can
        // create such a provider by adding a LoggerLookup delegate to the service collection
        // and building it.
        ILogger? capturedLogger = null;
        LoggerLookup loggerLookup = name => capturedLogger = new MockLogger(name: name).Object;
        services.AddSingleton(loggerLookup);
        var serviceProvider = services.BuildServiceProvider();

        var provider = descriptor.ImplementationFactory!(serviceProvider);

        provider.Should().BeOfType<RockLibLoggerProvider>()
            .Which.Logger.Should().BeSameAs(capturedLogger).And.NotBeNull();
    }

    [Fact(DisplayName = "AddRockLibLoggerProvider extension method 3 configures the configureOptions parameter")]
    public static void AddRockLibLoggerProviderExtensionMethod3HappyPath2()
    {
        RockLibLoggerOptions? capturedOptions = null;

        var services = new ServiceCollection();

        services.AddRockLibLoggerProvider(opt => capturedOptions = opt);

        var descriptor =
            services.Should().ContainSingle(d => d.ServiceType == typeof(IConfigureOptions<RockLibLoggerOptions>))
                .Subject;

        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        descriptor.ImplementationInstance.Should().NotBeNull();
        descriptor.ImplementationFactory.Should().BeNull();
        descriptor.ImplementationType.Should().BeNull();

        var configureOptions =
            descriptor.ImplementationInstance.Should().BeAssignableTo<IConfigureOptions<RockLibLoggerOptions>>()
                .Subject;

        var options = new RockLibLoggerOptions();

        configureOptions.Configure(options);

        capturedOptions.Should().BeSameAs(options);
    }

    [Fact(DisplayName = "AddRockLibLoggerProvider extension method 3 returns the services parameter")]
    public static void AddRockLibLoggerProviderExtensionMethod3HappyPath3()
    {
        var services = new ServiceCollection();

        var returnedServices = services.AddRockLibLoggerProvider(options => { });

        returnedServices.Should().BeSameAs(services);
    }

    [Fact(DisplayName = "AddRockLibLoggerProvider extension method 3 throws when services parameter is null")]
    public static void AddRockLibLoggerProviderExtensionMethod3SadPath()
    {
        var act = () => (null as IServiceCollection)!.AddRockLibLoggerProvider(options => { });

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*services*");
    }

    [Fact(DisplayName = "AddRockLibLoggerProvider extension method 4 adds a singleton RockLibLoggerProvider factory")]
    public static void AddRockLibLoggerProviderExtensionMethod4HappyPath1()
    {
        var services = new ServiceCollection();

        services.AddRockLibLoggerProvider("MyLogger");

        services.Should().ContainSingle();

        var descriptor = services[0];

        descriptor.ServiceType.Should().Be<ILoggerProvider>();
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        descriptor.ImplementationFactory.Should().NotBeNull();
        descriptor.ImplementationInstance.Should().BeNull();
        descriptor.ImplementationType.Should().BeNull();

        // In order to test the description.ImplementationFactory function, we need be able to
        // provide its required dependencies with its IServiceProvider parameter. We can
        // create such a provider by adding a LoggerLookup delegate to the service collection
        // and building it.
        ILogger? capturedLogger = null;
        LoggerLookup loggerLookup = name => capturedLogger = new MockLogger(name: name).Object;
        services.AddSingleton(loggerLookup);
        var serviceProvider = services.BuildServiceProvider();

        var provider = descriptor.ImplementationFactory!(serviceProvider);

        provider.Should().BeOfType<RockLibLoggerProvider>()
            .Which.Logger.Should().BeSameAs(capturedLogger).And.NotBeNull();
    }

    [Fact(DisplayName = "AddRockLibLoggerProvider extension method 4 configures the configureOptions parameter")]
    public static void AddRockLibLoggerProviderExtensionMethod4HappyPath2()
    {
        RockLibLoggerOptions? capturedOptions = null;

        var services = new ServiceCollection();

        services.AddRockLibLoggerProvider("MyLogger", opt => capturedOptions = opt);

        var descriptor =
            services.Should().ContainSingle(d => d.ServiceType == typeof(IConfigureOptions<RockLibLoggerOptions>))
                .Subject;

        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        descriptor.ImplementationInstance.Should().NotBeNull();
        descriptor.ImplementationFactory.Should().BeNull();
        descriptor.ImplementationType.Should().BeNull();

        var configureOptions =
            descriptor.ImplementationInstance.Should().BeAssignableTo<IConfigureOptions<RockLibLoggerOptions>>()
                .Subject;

        var options = new RockLibLoggerOptions();

        configureOptions.Configure(options);

        capturedOptions.Should().BeSameAs(options);
    }

    [Fact(DisplayName = "AddRockLibLoggerProvider extension method 4 returns the services parameter")]
    public static void AddRockLibLoggerProviderExtensionMethod4HappyPath3()
    {
        var services = new ServiceCollection();

        var returnedServices = services.AddRockLibLoggerProvider("MyLogger");

        returnedServices.Should().BeSameAs(services);
    }

    [Fact(DisplayName = "AddRockLibLoggerProvider extension method 4 throws when services parameter is null")]
    public static void AddRockLibLoggerProviderExtensionMethod4SadPath()
    {
        var act = () => (null as IServiceCollection)!.AddRockLibLoggerProvider("MyLogger");

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*services*");
    }
}
#pragma warning restore IDE0039 // Use local function
