using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        public void UseRockLibLoggingExtension1WithRegisterAspNetCoreLoggerTrueAddsLoggerAndProvider()
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

            fakeBuilder.UseRockLibLogging("SomeRockLibName", registerAspNetCoreLogger: true);

            servicesCollectionMock.Verify(lfm => lfm.Add(It.IsAny<ServiceDescriptor>()), Times.Exactly(3));

            // The second thing we happen to register is the RockLib.Logging.Logger
            var logger = (ILogger)serviceDescriptors[1].ImplementationFactory.Invoke(serviceProviderMock.Object);
            logger.Should().BeOfType<Logger>();

            // The third thing we happen to register is the RockLib.Logging.AspNetCore.RockLibLoggerProvider
            var loggerProvider = (ILoggerProvider)serviceDescriptors[2].ImplementationFactory.Invoke(serviceProviderMock.Object);
            loggerProvider.Should().BeOfType<RockLibLoggerProvider>();
            var rockLibLogger = (RockLibLogger)loggerProvider.CreateLogger("foo");
            rockLibLogger.CategoryName.Should().Be("foo");
            rockLibLogger.Logger.Should().BeSameAs(actualLogger);
        }

        [Fact]
        public void UseRockLibLoggingExtension1AddsLoggerButNotProvider()
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

            var serviceProvider = new Mock<IServiceProvider>().Object;

            var fakeBuilder = new FakeWebHostBuilder()
            {
                ServiceCollection = servicesCollectionMock.Object
            };

            fakeBuilder.UseRockLibLogging("SomeRockLibName");

            servicesCollectionMock.Verify(lfm => lfm.Add(It.IsAny<ServiceDescriptor>()), Times.Exactly(2));

            var logger = (ILogger)serviceDescriptors[1].ImplementationFactory.Invoke(serviceProvider);
            logger.Should().BeOfType<Logger>();
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
        public void UseRockLibLoggingExtension2WithRegisterAspNetCoreLoggerTrueAddsLoggerAndProvider()
        {
            var actualLogger = new Mock<ILogger>().Object;
            var aDifferentActualLogger = new Mock<ILogger>().Object;

            var serviceDescriptors = new List<ServiceDescriptor>();

            var servicesCollectionMock = new Mock<IServiceCollection>();
            servicesCollectionMock
                .Setup(scm => scm.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(sd => serviceDescriptors.Add(sd));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(m => m.GetService(typeof(ILogger))).Returns(aDifferentActualLogger);

            var fakeBuilder = new FakeWebHostBuilder()
            {
                ServiceCollection = servicesCollectionMock.Object
            };

            fakeBuilder.UseRockLibLogging(actualLogger, registerAspNetCoreLogger: true);

            servicesCollectionMock.Verify(lfm => lfm.Add(It.IsAny<ServiceDescriptor>()), Times.Exactly(2));

            // The first thing we happen to register is the RockLib.Logging.Logger
            var logger = (ILogger)serviceDescriptors[0].ImplementationInstance;
            logger.Should().BeSameAs(actualLogger);

            // The second thing we happen to register is the RockLib.Logging.AspNetCore.RockLibLoggerProvider
            var loggerProvider = (ILoggerProvider)serviceDescriptors[1].ImplementationFactory.Invoke(serviceProviderMock.Object);
            loggerProvider.Should().BeOfType<RockLibLoggerProvider>();
            var rockLibLogger = (RockLibLogger)loggerProvider.CreateLogger("foo");
            rockLibLogger.CategoryName.Should().Be("foo");
            rockLibLogger.Logger.Should().BeSameAs(aDifferentActualLogger);
        }

        [Fact]
        public void UseRockLibLoggingExtension2AddsLoggerButNotProvider()
        {
            var actualLogger = new Mock<ILogger>().Object;

            var serviceDescriptors = new List<ServiceDescriptor>();

            var servicesCollectionMock = new Mock<IServiceCollection>();
            servicesCollectionMock
                .Setup(scm => scm.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(sd => serviceDescriptors.Add(sd));

            var serviceProvider = new Mock<IServiceProvider>().Object;

            var fakeBuilder = new FakeWebHostBuilder()
            {
                ServiceCollection = servicesCollectionMock.Object
            };

            fakeBuilder.UseRockLibLogging(actualLogger);

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

            fakeBuilder.UseRockLibLogging("TestLogger1", defaultTypes: defaultTypes, registerAspNetCoreLogger: true);

            var logger = fakeBuilder.ServiceCollection[1].ImplementationFactory.Invoke(null);
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

            fakeBuilder.UseRockLibLogging("TestLogger2", defaultTypes: defaultTypes, valueConverters: valueConverters, registerAspNetCoreLogger: true);

            var logger = fakeBuilder.ServiceCollection[1].ImplementationFactory.Invoke(null);
            logger.Should().BeOfType<TestLogger>();

            var testLogger = (TestLogger)logger;
            testLogger.Location.X.Should().Be(2);
            testLogger.Location.Y.Should().Be(3);
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

            public IReadOnlyCollection<ILogProvider> LogProviders => throw new NotImplementedException();

            public IReadOnlyCollection<IContextProvider> ContextProviders => throw new NotImplementedException();

            public IErrorHandler ErrorHandler { get; set; }

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
