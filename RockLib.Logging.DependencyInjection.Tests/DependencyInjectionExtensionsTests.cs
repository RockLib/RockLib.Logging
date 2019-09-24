using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RockLib.Logging.LogProcessing;
using Xunit;

namespace RockLib.Logging.DependencyInjection.Tests
{
    public class DependencyInjectionExtensionsTests
    {
        [Fact]
        public void AddRockLibLoggerTransientThrowsOnNullServiceCollection()
        {
            Action action = () => ((IServiceCollection)null).AddRockLibLoggerTransient();

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: services");
        }

        [Fact]
        public void AddRockLibLoggerTransientThrowsWhenServiceCollectionAlreadyHasILogger()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILogger>(new Logger());

            Action action = () => services.AddRockLibLoggerTransient();

            action.Should().Throw<ArgumentException>().WithMessage("RockLib.Logging.ILogger has already been added to the service collection.\r\nParameter name: services");
        }

        [Fact]
        public void AddRockLibLoggerTransientReturnsTheSameServiceCollection()
        {
            var services = new ServiceCollection();

            var returnedServiceCollection = services.AddRockLibLoggerTransient();

            returnedServiceCollection.Should().BeSameAs(services);
        }

        [Fact]
        public void AddRockLibLoggerTransientAddsLogger()
        {
            var services = new ServiceCollection();

            services.AddRockLibLoggerTransient("SomeRockLibName",
                addLoggerProvider: false, addBackgroundLogProcessor: false, reloadOnConfigChange: false);

            var serviceProvider = services.BuildServiceProvider();

            services.Should().HaveCount(1);

            services[0].ServiceType.Should().Be<ILogger>();
            services[0].Lifetime.Should().Be(ServiceLifetime.Transient);
            services[0].ImplementationFactory.Should().NotBeNull();
            services[0].ImplementationInstance.Should().BeNull();
            services[0].ImplementationType.Should().BeNull();

            var logger = serviceProvider.GetRequiredService<ILogger>();
            logger.Should().BeOfType<Logger>();
            logger.Name.Should().Be("SomeRockLibName");
        }

        [Fact]
        public void AddRockLibLoggerTransientWithAddLoggerProviderTrueAddsLoggerAndLoggerProvider()
        {
            var services = new ServiceCollection();

            services.AddRockLibLoggerTransient("SomeRockLibName",
                addLoggerProvider: true, addBackgroundLogProcessor: false, reloadOnConfigChange: false);

            var serviceProvider = services.BuildServiceProvider();

            services.Should().HaveCount(2);

            services[0].ServiceType.Should().Be<ILogger>();
            services[0].Lifetime.Should().Be(ServiceLifetime.Transient);
            services[0].ImplementationFactory.Should().NotBeNull();
            services[0].ImplementationInstance.Should().BeNull();
            services[0].ImplementationType.Should().BeNull();

            services[1].ServiceType.Should().Be<ILoggerProvider>();
            services[1].Lifetime.Should().Be(ServiceLifetime.Singleton);
            services[1].ImplementationFactory.Should().NotBeNull();
            services[1].ImplementationInstance.Should().BeNull();
            services[1].ImplementationType.Should().BeNull();

            var logger = serviceProvider.GetRequiredService<ILogger>();
            logger.Should().BeOfType<Logger>();
            logger.Name.Should().Be("SomeRockLibName");

            var rockLibLoggerProvider = serviceProvider.GetRequiredService<ILoggerProvider>();
            rockLibLoggerProvider.Should().BeOfType<RockLibLoggerProvider>();
            var microsoftLogger = rockLibLoggerProvider.CreateLogger("SomeCategoryName");
            microsoftLogger.Should().BeOfType<RockLibLogger>();
            var rockLibLogger = (RockLibLogger)microsoftLogger;
            rockLibLogger.Logger.Name.Should().Be("SomeRockLibName");
            rockLibLogger.Logger.Should().NotBeSameAs(logger);
            rockLibLogger.CategoryName.Should().Be("SomeCategoryName");
        }

        [Fact]
        public void AddRockLibLoggerTransientWithAddBackgroundLogProcessorTrueAddsLoggerAndLogProcessor()
        {
            var services = new ServiceCollection();

            services.AddRockLibLoggerTransient("SomeRockLibName",
                addLoggerProvider: false, addBackgroundLogProcessor: true, reloadOnConfigChange: false);

            var serviceProvider = services.BuildServiceProvider();

            services.Should().HaveCount(2);

            services[0].ServiceType.Should().Be<ILogProcessor>();
            services[0].Lifetime.Should().Be(ServiceLifetime.Singleton);
            services[0].ImplementationFactory.Should().BeNull();
            services[0].ImplementationInstance.Should().BeNull();
            services[0].ImplementationType.Should().Be<BackgroundLogProcessor>();

            services[1].ServiceType.Should().Be<ILogger>();
            services[1].Lifetime.Should().Be(ServiceLifetime.Transient);
            services[1].ImplementationFactory.Should().NotBeNull();
            services[1].ImplementationInstance.Should().BeNull();
            services[1].ImplementationType.Should().BeNull();

            var logger = serviceProvider.GetRequiredService<ILogger>();
            logger.Should().BeOfType<Logger>();
            logger.Name.Should().Be("SomeRockLibName");

            var logProcessor = serviceProvider.GetRequiredService<ILogProcessor>();
            logProcessor.Should().BeOfType<BackgroundLogProcessor>();
        }

        [Fact]
        public void AddRockLibLoggerTransientWithReloadOnConfigChangeTrueAddsReloadingLogger()
        {
            var services = new ServiceCollection();

            services.AddRockLibLoggerTransient("SomeRockLibName",
                addLoggerProvider: false, addBackgroundLogProcessor: false, reloadOnConfigChange: true);

            var serviceProvider = services.BuildServiceProvider();

            services.Should().HaveCount(1);

            services[0].ServiceType.Should().Be<ILogger>();
            services[0].Lifetime.Should().Be(ServiceLifetime.Transient);
            services[0].ImplementationFactory.Should().NotBeNull();
            services[0].ImplementationInstance.Should().BeNull();
            services[0].ImplementationType.Should().BeNull();

            var logger = serviceProvider.GetRequiredService<ILogger>();
            logger.Should().NotBeOfType<Logger>();
            logger.GetType().Name.Should().Be("<ILogger>c__RockLibConfigReloadingProxyClass");
            logger.Name.Should().Be("SomeRockLibName");
        }

        [Fact]
        public void AddRockLibLoggerSingleton1ThrowsOnNullServiceCollection()
        {
            Action action = () => ((IServiceCollection)null).AddRockLibLoggerSingleton();

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: services");
        }

        [Fact]
        public void AddRockLibLoggerSingleton1ThrowsWhenServiceCollectionAlreadyHasILogger()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILogger>(new Logger());

            Action action = () => services.AddRockLibLoggerSingleton();

            action.Should().Throw<ArgumentException>().WithMessage("RockLib.Logging.ILogger has already been added to the service collection.\r\nParameter name: services");
        }

        [Fact]
        public void AddRockLibLoggerSingleton1ReturnsTheSameServiceCollection()
        {
            var services = new ServiceCollection();

            var returnedServiceCollection = services.AddRockLibLoggerSingleton();

            returnedServiceCollection.Should().BeSameAs(services);
        }

        [Fact]
        public void AddRockLibLoggerSingleton1AddsLogger()
        {
            var services = new ServiceCollection();

            services.AddRockLibLoggerSingleton("SomeRockLibName",
                addLoggerProvider: false, reloadOnConfigChange: false);

            var serviceProvider = services.BuildServiceProvider();

            services.Should().HaveCount(1);

            services[0].ServiceType.Should().Be<ILogger>();
            services[0].Lifetime.Should().Be(ServiceLifetime.Singleton);
            services[0].ImplementationFactory.Should().NotBeNull();
            services[0].ImplementationInstance.Should().BeNull();
            services[0].ImplementationType.Should().BeNull();

            var logger = serviceProvider.GetRequiredService<ILogger>();
            logger.Should().BeOfType<Logger>();
            logger.Name.Should().Be("SomeRockLibName");
        }

        [Fact]
        public void AddRockLibLoggerSingleton1WithAddLoggerProviderTrueAddsLoggerAndLoggerProvider()
        {
            var services = new ServiceCollection();

            services.AddRockLibLoggerSingleton("SomeRockLibName",
                addLoggerProvider: true, reloadOnConfigChange: false);

            var serviceProvider = services.BuildServiceProvider();

            services.Should().HaveCount(2);

            services[0].ServiceType.Should().Be<ILogger>();
            services[0].Lifetime.Should().Be(ServiceLifetime.Singleton);
            services[0].ImplementationFactory.Should().NotBeNull();
            services[0].ImplementationInstance.Should().BeNull();
            services[0].ImplementationType.Should().BeNull();

            services[1].ServiceType.Should().Be<ILoggerProvider>();
            services[1].Lifetime.Should().Be(ServiceLifetime.Singleton);
            services[1].ImplementationFactory.Should().NotBeNull();
            services[1].ImplementationInstance.Should().BeNull();
            services[1].ImplementationType.Should().BeNull();

            var logger = serviceProvider.GetRequiredService<ILogger>();
            logger.Should().BeOfType<Logger>();
            logger.Name.Should().Be("SomeRockLibName");

            var rockLibLoggerProvider = serviceProvider.GetRequiredService<ILoggerProvider>();
            rockLibLoggerProvider.Should().BeOfType<RockLibLoggerProvider>();
            var microsoftLogger = rockLibLoggerProvider.CreateLogger("SomeCategoryName");
            microsoftLogger.Should().BeOfType<RockLibLogger>();
            var rockLibLogger = (RockLibLogger)microsoftLogger;
            rockLibLogger.Logger.Name.Should().Be("SomeRockLibName");
            rockLibLogger.Logger.Should().BeSameAs(logger);
            rockLibLogger.CategoryName.Should().Be("SomeCategoryName");
        }

        [Fact]
        public void AddRockLibLoggerSingleton1WithReloadOnConfigChangeTrueAddsReloadingLogger()
        {
            var services = new ServiceCollection();

            services.AddRockLibLoggerSingleton("SomeRockLibName",
                addLoggerProvider: false, reloadOnConfigChange: true);

            var serviceProvider = services.BuildServiceProvider();

            services.Should().HaveCount(1);

            services[0].ServiceType.Should().Be<ILogger>();
            services[0].Lifetime.Should().Be(ServiceLifetime.Singleton);
            services[0].ImplementationFactory.Should().NotBeNull();
            services[0].ImplementationInstance.Should().BeNull();
            services[0].ImplementationType.Should().BeNull();

            var logger = serviceProvider.GetRequiredService<ILogger>();
            logger.Should().NotBeOfType<Logger>();
            logger.GetType().Name.Should().Be("<ILogger>c__RockLibConfigReloadingProxyClass");
            logger.Name.Should().Be("SomeRockLibName");
        }

        [Fact]
        public void AddRockLibLoggerSingleton2ThrowsOnNullServiceCollection()
        {
            var logger = new Logger();

            Action action = () => ((IServiceCollection)null).AddRockLibLoggerSingleton(logger);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: services");
        }

        [Fact]
        public void AddRockLibLoggerSingleton2ThrowsOnNullLogger()
        {
            ILogger logger = null;
            var services = new ServiceCollection();

            Action action = () => services.AddRockLibLoggerSingleton(logger);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: logger");
        }

        [Fact]
        public void AddRockLibLoggerSingleton2ThrowsWhenServiceCollectionAlreadyHasILogger()
        {
            var logger = new Logger();
            var services = new ServiceCollection();

            services.AddSingleton<ILogger>(new Logger());

            Action action = () => services.AddRockLibLoggerSingleton(logger);

            action.Should().Throw<ArgumentException>().WithMessage("RockLib.Logging.ILogger has already been added to the service collection.\r\nParameter name: services");
        }

        [Fact]
        public void AddRockLibLoggerSingleton2ReturnsTheSameServiceCollection()
        {
            var logger = new Logger();
            var services = new ServiceCollection();

            var returnedServiceCollection = services.AddRockLibLoggerSingleton(logger);

            returnedServiceCollection.Should().BeSameAs(services);
        }

        [Fact]
        public void AddRockLibLoggerSingleton2AddsLogger()
        {
            var logger = new Logger();
            var services = new ServiceCollection();

            services.AddRockLibLoggerSingleton(logger, addLoggerProvider: false);

            var serviceProvider = services.BuildServiceProvider();

            services.Should().HaveCount(1);

            services[0].ServiceType.Should().Be<ILogger>();
            services[0].Lifetime.Should().Be(ServiceLifetime.Singleton);
            services[0].ImplementationFactory.Should().BeNull();
            services[0].ImplementationInstance.Should().BeSameAs(logger);
            services[0].ImplementationType.Should().BeNull();

            var retrievedLogger = serviceProvider.GetRequiredService<ILogger>();
            retrievedLogger.Should().BeOfType<Logger>();
            retrievedLogger.Should().BeSameAs(logger);
        }

        [Fact]
        public void AddRockLibLoggerSingleton2WithAddLoggerProviderTrueAddsLoggerAndLoggerProvider()
        {
            var logger = new Logger();
            var services = new ServiceCollection();

            services.AddRockLibLoggerSingleton(logger, addLoggerProvider: true);

            var serviceProvider = services.BuildServiceProvider();

            services.Should().HaveCount(2);

            services[0].ServiceType.Should().Be<ILogger>();
            services[0].Lifetime.Should().Be(ServiceLifetime.Singleton);
            services[0].ImplementationFactory.Should().BeNull();
            services[0].ImplementationInstance.Should().BeSameAs(logger);
            services[0].ImplementationType.Should().BeNull();

            services[1].ServiceType.Should().Be<ILoggerProvider>();
            services[1].Lifetime.Should().Be(ServiceLifetime.Singleton);
            services[1].ImplementationFactory.Should().NotBeNull();
            services[1].ImplementationInstance.Should().BeNull();
            services[1].ImplementationType.Should().BeNull();

            var retrievedLogger = serviceProvider.GetRequiredService<ILogger>();
            retrievedLogger.Should().BeOfType<Logger>();
            retrievedLogger.Should().BeSameAs(logger);

            var rockLibLoggerProvider = serviceProvider.GetRequiredService<ILoggerProvider>();
            rockLibLoggerProvider.Should().BeOfType<RockLibLoggerProvider>();
            var microsoftLogger = rockLibLoggerProvider.CreateLogger("SomeCategoryName");
            microsoftLogger.Should().BeOfType<RockLibLogger>();
            var rockLibLogger = (RockLibLogger)microsoftLogger;
            rockLibLogger.Logger.Should().BeSameAs(retrievedLogger);
            rockLibLogger.CategoryName.Should().Be("SomeCategoryName");
        }
    }
}
