using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RockLib.Configuration;
using RockLib.Configuration.ObjectFactory;
using RockLib.Logging.DependencyInjection;
using RockLib.Logging.LogProcessing;
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
            var fakeBuilder = new FakeWebHostBuilder();

            fakeBuilder.UseRockLibLogging("SomeRockLibName", registerAspNetCoreLogger: true);

            fakeBuilder.ServiceCollection.Should().HaveCount(3);

            fakeBuilder.ServiceCollection[0].ImplementationFactory.Should().BeNull();
            fakeBuilder.ServiceCollection[0].ImplementationInstance.Should().BeNull();
            fakeBuilder.ServiceCollection[0].ImplementationType.Should().Be<BackgroundLogProcessor>();
            fakeBuilder.ServiceCollection[0].Lifetime.Should().Be(ServiceLifetime.Singleton);
            fakeBuilder.ServiceCollection[0].ServiceType.Should().Be<ILogProcessor>();

            fakeBuilder.ServiceCollection[1].ImplementationFactory.Should().NotBeNull();
            fakeBuilder.ServiceCollection[1].ImplementationInstance.Should().BeNull();
            fakeBuilder.ServiceCollection[1].ImplementationType.Should().BeNull();
            fakeBuilder.ServiceCollection[1].Lifetime.Should().Be(ServiceLifetime.Transient);
            fakeBuilder.ServiceCollection[1].ServiceType.Should().Be<ILogger>();

            fakeBuilder.ServiceCollection[2].ImplementationFactory.Should().NotBeNull();
            fakeBuilder.ServiceCollection[2].ImplementationInstance.Should().BeNull();
            fakeBuilder.ServiceCollection[2].ImplementationType.Should().BeNull();
            fakeBuilder.ServiceCollection[2].Lifetime.Should().Be(ServiceLifetime.Singleton);
            fakeBuilder.ServiceCollection[2].ServiceType.Should().Be<ILoggerProvider>();

            var serviceProvider = fakeBuilder.ServiceCollection.BuildServiceProvider();

            var logger = (Logger)fakeBuilder.ServiceCollection[1].ImplementationFactory.Invoke(serviceProvider);
            logger.Name.Should().Be("SomeRockLibName");

            var loggerProvider = (RockLibLoggerProvider)fakeBuilder.ServiceCollection[2].ImplementationFactory.Invoke(serviceProvider);
            var rockLibLogger = (RockLibLogger)loggerProvider.CreateLogger("foo");
            rockLibLogger.CategoryName.Should().Be("foo");
            rockLibLogger.Logger.Name.Should().Be("SomeRockLibName");
        }

        [Fact]
        public void UseRockLibLoggingExtension1AddsLoggerButNotProvider()
        {
            var fakeBuilder = new FakeWebHostBuilder();

            fakeBuilder.UseRockLibLogging("SomeRockLibName");

            fakeBuilder.ServiceCollection.Should().HaveCount(2);

            fakeBuilder.ServiceCollection[0].ImplementationFactory.Should().BeNull();
            fakeBuilder.ServiceCollection[0].ImplementationInstance.Should().BeNull();
            fakeBuilder.ServiceCollection[0].ImplementationType.Should().Be<BackgroundLogProcessor>();
            fakeBuilder.ServiceCollection[0].Lifetime.Should().Be(ServiceLifetime.Singleton);
            fakeBuilder.ServiceCollection[0].ServiceType.Should().Be<ILogProcessor>();

            fakeBuilder.ServiceCollection[1].ImplementationFactory.Should().NotBeNull();
            fakeBuilder.ServiceCollection[1].ImplementationInstance.Should().BeNull();
            fakeBuilder.ServiceCollection[1].ImplementationType.Should().BeNull();
            fakeBuilder.ServiceCollection[1].Lifetime.Should().Be(ServiceLifetime.Transient);
            fakeBuilder.ServiceCollection[1].ServiceType.Should().Be<ILogger>();

            var serviceProvider = fakeBuilder.ServiceCollection.BuildServiceProvider();

            var logger = (Logger)fakeBuilder.ServiceCollection[1].ImplementationFactory.Invoke(serviceProvider);
            logger.Name.Should().Be("SomeRockLibName");
        }

        [Fact]
        public void UseRockLibLoggingExtension2ThrowsOnNullBuilder()
        {
            var actualLogger = new Logger();

            Action action = () => ((IWebHostBuilder)null).UseRockLibLogging(actualLogger);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: builder");
        }

        [Fact]
        public void UseRockLibLoggingExtension2ThrowsOnNullLogger()
        {
            var webHostBuilder = new FakeWebHostBuilder();

            Action action = () => webHostBuilder.UseRockLibLogging((ILogger)null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: logger");
        }

        [Fact]
        public void UseRockLibLoggingExtension2WithRegisterAspNetCoreLoggerTrueAddsLoggerAndProvider()
        {
            var actualLogger = new Logger();

            var fakeBuilder = new FakeWebHostBuilder();

            fakeBuilder.UseRockLibLogging(actualLogger, registerAspNetCoreLogger: true);

            fakeBuilder.ServiceCollection.Should().HaveCount(2);

            fakeBuilder.ServiceCollection[0].ImplementationFactory.Should().BeNull();
            fakeBuilder.ServiceCollection[0].ImplementationInstance.Should().BeSameAs(actualLogger);
            fakeBuilder.ServiceCollection[0].ImplementationType.Should().BeNull();
            fakeBuilder.ServiceCollection[0].Lifetime.Should().Be(ServiceLifetime.Singleton);
            fakeBuilder.ServiceCollection[0].ServiceType.Should().Be<ILogger>();

            fakeBuilder.ServiceCollection[1].ImplementationFactory.Should().NotBeNull();
            fakeBuilder.ServiceCollection[1].ImplementationInstance.Should().BeNull();
            fakeBuilder.ServiceCollection[1].ImplementationType.Should().BeNull();
            fakeBuilder.ServiceCollection[1].Lifetime.Should().Be(ServiceLifetime.Singleton);
            fakeBuilder.ServiceCollection[1].ServiceType.Should().Be<ILoggerProvider>();

            var serviceProvider = fakeBuilder.ServiceCollection.BuildServiceProvider();

            var loggerProvider = (RockLibLoggerProvider)fakeBuilder.ServiceCollection[1].ImplementationFactory.Invoke(serviceProvider);
            var rockLibLogger = (RockLibLogger)loggerProvider.CreateLogger("foo");
            rockLibLogger.CategoryName.Should().Be("foo");
            rockLibLogger.Logger.Should().BeSameAs(actualLogger);
        }

        [Fact]
        public void UseRockLibLoggingExtension2AddsLoggerButNotProvider()
        {
            var actualLogger = new Logger();

            var fakeBuilder = new FakeWebHostBuilder();

            fakeBuilder.UseRockLibLogging(actualLogger);

            fakeBuilder.ServiceCollection.Should().HaveCount(1);

            fakeBuilder.ServiceCollection[0].ImplementationFactory.Should().BeNull();
            fakeBuilder.ServiceCollection[0].ImplementationInstance.Should().BeSameAs(actualLogger);
            fakeBuilder.ServiceCollection[0].ImplementationType.Should().BeNull();
            fakeBuilder.ServiceCollection[0].Lifetime.Should().Be(ServiceLifetime.Singleton);
            fakeBuilder.ServiceCollection[0].ServiceType.Should().Be<ILogger>();
        }

        [Fact]
        public void DefaultTypesFunctionsProperly()
        {
            var defaultTypes = new DefaultTypes
            {
                { typeof(ILogger), typeof(TestLogger) },
                { typeof(ITestDependency), typeof(TestDependencyB) }
            };

            var fakeBuilder = new FakeWebHostBuilder();

            fakeBuilder.UseRockLibLogging("TestLogger1", defaultTypes: defaultTypes, registerAspNetCoreLogger: true);

            var logger = fakeBuilder.ServiceCollection[1].ImplementationFactory.Invoke(null);
            logger.Should().BeOfType<TestLogger>();

            var testLogger = (TestLogger)logger;
            testLogger.Dependency.Should().BeOfType<TestDependencyB>();
        }

        [Fact]
        public void ValueConvertersFunctionsProperly()
        {
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

            var fakeBuilder = new FakeWebHostBuilder();

            fakeBuilder.UseRockLibLogging("TestLogger2", defaultTypes: defaultTypes, valueConverters: valueConverters, registerAspNetCoreLogger: true);

            var logger = fakeBuilder.ServiceCollection[1].ImplementationFactory.Invoke(null);
            logger.Should().BeOfType<TestLogger>();

            var testLogger = (TestLogger)logger;
            testLogger.Location.X.Should().Be(2);
            testLogger.Location.Y.Should().Be(3);
        }

        private class FakeWebHostBuilder : IWebHostBuilder
        {
            public IServiceCollection ServiceCollection { get; } = new ServiceCollection();
            public IConfiguration Configuration { get; } = Config.Root;

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
                configureServices(new WebHostBuilderContext { Configuration = Configuration }, ServiceCollection);
                return this;
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
