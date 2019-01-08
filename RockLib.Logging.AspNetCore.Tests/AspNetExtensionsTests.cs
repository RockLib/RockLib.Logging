using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RockLib.Configuration;
using RockLib.Configuration.ObjectFactory;
using Xunit;

namespace RockLib.Logging.AspNetCore.Tests
{
    public class AspNetExtensionsTests
    {
        [Fact]
        public void UseRockLibLoggingExtension1ThrowsOnNullBuilder()
        {
            Action action = () => ((IWebHostBuilder)null).UseRockLibLogging();

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: builder");
        }

        [Fact]
        public void UseRockLibLoggingExtension1AddsLoggerAndProvider()
        {
            if (!Config.IsLocked)
            {
                var dummy = Config.Root;
            }

            var actualLogger = LoggerFactory.GetCached("SomeRockLibName");

            var serviceDescriptors = new List<ServiceDescriptor>();

            var servicesCollectionMock = new Mock<IServiceCollection>();
            servicesCollectionMock
                .Setup(scm => scm.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(sd => serviceDescriptors.Add(sd));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(m => m.GetService(typeof(ILogger))).Returns(actualLogger);

            var fakeBuilder = new FakeWebHostBuilder()
            {
                ServiceCollection = servicesCollectionMock.Object
            };

            fakeBuilder.UseRockLibLogging("SomeRockLibName");

            servicesCollectionMock.Verify(lfm => lfm.Add(It.IsAny<ServiceDescriptor>()), Times.Exactly(2));

            // The first thing we happen to register is the RockLib.Logging.Logger
            var logger = (ILogger)serviceDescriptors[0].ImplementationFactory.Invoke(null);
            logger.Should().BeSameAs(actualLogger);

            // The second thing we happen to register is the RockLib.Logging.AspNetCore.RockLibLoggerProvider
            var provider = (RockLibLoggerProvider)serviceDescriptors[1].ImplementationFactory.Invoke(serviceProviderMock.Object);
            provider.Logger.Should().BeSameAs(actualLogger);
        }

        [Fact]
        public void UseRockLibLoggingExtension1WithBypassAddsLoggerButNotProvider()
        {
            if (!Config.IsLocked)
            {
                var dummy = Config.Root;
            }

            var actualLogger = LoggerFactory.GetCached("SomeRockLibName");

            var serviceDescriptors = new List<ServiceDescriptor>();

            var servicesCollectionMock = new Mock<IServiceCollection>();
            servicesCollectionMock
                .Setup(scm => scm.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(sd => serviceDescriptors.Add(sd));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(m => m.GetService(typeof(ILogger))).Returns(actualLogger);

            var fakeBuilder = new FakeWebHostBuilder()
            {
                ServiceCollection = servicesCollectionMock.Object
            };

            fakeBuilder.UseRockLibLogging("SomeRockLibName", bypassAspNetCoreLogging: true);

            servicesCollectionMock.Verify(lfm => lfm.Add(It.IsAny<ServiceDescriptor>()), Times.Once);

            var logger = (ILogger)serviceDescriptors[0].ImplementationFactory.Invoke(null);
            logger.Should().BeSameAs(actualLogger);
        }

        [Fact]
        public void UseRockLibLoggingExtension2ThrowsOnNullBuilder()
        {
            var actualLogger = new Mock<ILogger>().Object;

            Action action = () => ((IWebHostBuilder)null).UseRockLibLogging(actualLogger);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: builder");
        }

        [Fact]
        public void UseRockLibLoggingExtension2ThrowsOnNullLogger()
        {
            var webHostBuilder = new Mock<IWebHostBuilder>().Object;

            Action action = () => webHostBuilder.UseRockLibLogging((ILogger)null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: logger");
        }

        [Fact]
        public void UseRockLibLoggingExtension2AddsLoggerAndProvider()
        {
            var actualLogger = new Mock<ILogger>().Object;

            var serviceDescriptors = new List<ServiceDescriptor>();

            var servicesCollectionMock = new Mock<IServiceCollection>();
            servicesCollectionMock
                .Setup(scm => scm.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(sd => serviceDescriptors.Add(sd));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(m => m.GetService(typeof(ILogger))).Returns(actualLogger);

            var fakeBuilder = new FakeWebHostBuilder()
            {
                ServiceCollection = servicesCollectionMock.Object
            };

            fakeBuilder.UseRockLibLogging(actualLogger);

            servicesCollectionMock.Verify(lfm => lfm.Add(It.IsAny<ServiceDescriptor>()), Times.Exactly(2));

            // The first thing we happen to register is the RockLib.Logging.Logger
            var logger = (ILogger)serviceDescriptors[0].ImplementationInstance;
            logger.Should().BeSameAs(actualLogger);

            // The second thing we happen to register is the RockLib.Logging.AspNetCore.RockLibLoggerProvider
            var provider = (RockLibLoggerProvider)serviceDescriptors[1].ImplementationFactory.Invoke(serviceProviderMock.Object);
            provider.Logger.Should().BeSameAs(actualLogger);
        }

        [Fact]
        public void UseRockLibLoggingExtension2WithBypassAddsLoggerButNotProvider()
        {
            var actualLogger = new Mock<ILogger>().Object;

            var serviceDescriptors = new List<ServiceDescriptor>();

            var servicesCollectionMock = new Mock<IServiceCollection>();
            servicesCollectionMock
                .Setup(scm => scm.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(sd => serviceDescriptors.Add(sd));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(m => m.GetService(typeof(ILogger))).Returns(actualLogger);

            var fakeBuilder = new FakeWebHostBuilder()
            {
                ServiceCollection = servicesCollectionMock.Object
            };

            fakeBuilder.UseRockLibLogging(actualLogger, true);

            servicesCollectionMock.Verify(lfm => lfm.Add(It.IsAny<ServiceDescriptor>()), Times.Once);

            var logger = (ILogger)serviceDescriptors[0].ImplementationInstance;
            logger.Should().BeSameAs(actualLogger);
        }

        [Fact]
        public void DefaultTypesFunctionsProperly()
        {
            if (!Config.IsLocked)
            {
                var dummy = Config.Root;
            }

            var defaultTypes = new DefaultTypes
            {
                { typeof(ILogger), typeof(TestLogger) },
                { typeof(ITestDependency), typeof(TestDependencyB) }
            };

            var fakeBuilder = new FakeWebHostBuilder()
            {
                ServiceCollection = new ServiceCollection()
            };

            fakeBuilder.UseRockLibLogging("TestLogger1", defaultTypes: defaultTypes, reloadOnConfigChange: false, bypassAspNetCoreLogging: true);

            var logger = fakeBuilder.ServiceCollection[0].ImplementationFactory.Invoke(null);
            logger.Should().BeOfType<TestLogger>();

            var testLogger = (TestLogger)logger;
            testLogger.Dependency.Should().BeOfType<TestDependencyB>();
        }

        [Fact]
        public void ValueConvertersFunctionsProperly()
        {
            if (!Config.IsLocked)
            {
                var dummy = Config.Root;
            }

            var defaultTypes = new DefaultTypes
            {
                { typeof(ILogger), typeof(TestLogger) }
            };

            Point ParsePoint(string value)
            {
                var split = value.Split(',');
                return new Point(int.Parse(split[0]), int.Parse(split[1]));
            }
            var valueConverters = new ValueConverters
            {
                { typeof(Point), ParsePoint }
            };

            var fakeBuilder = new FakeWebHostBuilder()
            {
                ServiceCollection = new ServiceCollection()
            };

            fakeBuilder.UseRockLibLogging("TestLogger2", defaultTypes: defaultTypes, valueConverters: valueConverters, reloadOnConfigChange: false, bypassAspNetCoreLogging: true);

            var logger = fakeBuilder.ServiceCollection[0].ImplementationFactory.Invoke(null);
            logger.Should().BeOfType<TestLogger>();

            var testLogger = (TestLogger)logger;
            testLogger.Location.X.Should().Be(2);
            testLogger.Location.Y.Should().Be(3);
        }

        [Fact]
        public void ResolverFunctionsProperly()
        {
            if (!Config.IsLocked)
            {
                var dummy = Config.Root;
            }

            var dependency = new TestDependencyA();
            var resolver = new Resolver(t => dependency, t => t == typeof(ITestDependency));

            var defaultTypes = new DefaultTypes
            {
                { typeof(ILogger), typeof(TestLogger) }
            };

            var fakeBuilder = new FakeWebHostBuilder()
            {
                ServiceCollection = new ServiceCollection()
            };

            fakeBuilder.UseRockLibLogging("TestLogger3", defaultTypes: defaultTypes, resolver: resolver, reloadOnConfigChange: false, bypassAspNetCoreLogging: true);

            var logger = fakeBuilder.ServiceCollection[0].ImplementationFactory.Invoke(null);
            logger.Should().BeOfType<TestLogger>();

            var testLogger = (TestLogger)logger;
            testLogger.Dependency.Should().BeSameAs(dependency);
        }

        [Fact]
        public void ReloadOnConfigChangeTrueFunctionsProperly()
        {
            if (!Config.IsLocked)
            {
                var dummy = Config.Root;
            }

            var defaultTypes = new DefaultTypes
            {
                { typeof(ILogger), typeof(TestLogger) },
                { typeof(ITestDependency), typeof(TestDependencyB) }
            };

            var fakeBuilder = new FakeWebHostBuilder()
            {
                ServiceCollection = new ServiceCollection()
            };

            fakeBuilder.UseRockLibLogging("TestLogger4", defaultTypes: defaultTypes, reloadOnConfigChange: true, bypassAspNetCoreLogging: true);

            var logger = fakeBuilder.ServiceCollection[0].ImplementationFactory.Invoke(null);
            logger.Should().BeAssignableTo<ConfigReloadingProxy<ILogger>>();
        }

        [Fact]
        public void ReloadOnConfigChangeFalseFunctionsProperly()
        {
            if (!Config.IsLocked)
            {
                var dummy = Config.Root;
            }

            var defaultTypes = new DefaultTypes
            {
                { typeof(ILogger), typeof(TestLogger) },
                { typeof(ITestDependency), typeof(TestDependencyB) }
            };

            var fakeBuilder = new FakeWebHostBuilder()
            {
                ServiceCollection = new ServiceCollection()
            };

            fakeBuilder.UseRockLibLogging("TestLogger5", defaultTypes: defaultTypes, reloadOnConfigChange: false, bypassAspNetCoreLogging: true);

            var logger = fakeBuilder.ServiceCollection[0].ImplementationFactory.Invoke(null);
            logger.Should().BeOfType<TestLogger>();
        }

        private class FakeWebHostBuilder : IWebHostBuilder
        {
            public IServiceCollection ServiceCollection { get; set; }

            public IWebHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
            {
                configureServices(ServiceCollection);
                return this;
            }

            #region UnusedImplementations
            public IWebHost Build()
            {
                throw new NotImplementedException();
            }

            public IWebHostBuilder ConfigureAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configureDelegate)
            {
                throw new NotImplementedException();
            }

            public IWebHostBuilder ConfigureServices(Action<WebHostBuilderContext, IServiceCollection> configureServices)
            {
                throw new NotImplementedException();
            }

            public string GetSetting(string key)
            {
                throw new NotImplementedException();
            }

            public IWebHostBuilder UseSetting(string key, string value)
            {
                throw new NotImplementedException();
            }
            #endregion
        }

        public class TestLogger : ILogger
        {
            public TestLogger(Point location = default(Point), ITestDependency dependency = null)
            {
                Name = nameof(TestLogger);
                Location = location;
                Dependency = dependency;
            }

            public Point Location { get; }
            public ITestDependency Dependency { get; }

            public string Name { get; }
            public bool IsDisabled { get; }
            public LogLevel Level { get; }
            public IReadOnlyCollection<ILogProvider> Providers { get; }

            public void Log(LogEntry logEntry, string callerMemberName = null, string callerFilePath = null, int callerLineNumber = 0)
            {
                throw new NotImplementedException();
            }

            public void Dispose() { }
        }

        public struct Point
        {
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; }

            public int Y { get; }
        }

        public interface ITestDependency
        {
        }

        public class TestDependencyA : ITestDependency
        {
        }
        public class TestDependencyB : ITestDependency
        {
        }
    }
}
