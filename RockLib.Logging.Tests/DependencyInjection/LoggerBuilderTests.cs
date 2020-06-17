using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using RockLib.Logging.DependencyInjection;
using RockLib.Logging.LogProcessing;
using System;
using System.Collections.Generic;
using Xunit;

namespace RockLib.Logging.Tests.DependencyInjection
{
    public class LoggerBuilderTests
    {
        [Fact]
        public void ConstructorHappyPath1()
        {
            var services = new ServiceCollection();
            var name = "foo";
            Action<ILoggerOptions> configureOptions = o => o.Level = LogLevel.Info;

            var builder = new LoggerBuilder(services, name, configureOptions);

            builder.Services.Should().BeSameAs(services);
            builder.LoggerName.Should().BeSameAs(name);
            builder.ConfigureOptions.Should().BeSameAs(configureOptions);
        }

        [Fact]
        public void ConstructorHappyPath2()
        {
            var services = new ServiceCollection();
            string name = null;
            Action<ILoggerOptions> configureOptions = o => o.Level = LogLevel.Info;

            var builder = new LoggerBuilder(services, name, configureOptions);

            builder.Services.Should().BeSameAs(services);
            builder.LoggerName.Should().BeSameAs(Logger.DefaultName);
            builder.ConfigureOptions.Should().BeSameAs(configureOptions);
        }

        [Fact]
        public void ConstructorSadPath()
        {
            IServiceCollection services = null;
            var name = "foo";
            Action<ILoggerOptions> configureOptions = o => o.Level = LogLevel.Info;

            Action act = () => new LoggerBuilder(services, name, configureOptions);

            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*services*");
        }

        [Fact]
        public void AddLogProviderMethodHappyPath()
        {
            var services = new ServiceCollection();
            var builder = new LoggerBuilder(services, Logger.DefaultName, null);

            var logProvider = new Mock<ILogProvider>().Object;

            Func<IServiceProvider, ILogProvider> logProviderRegistration = sp => logProvider;

            builder.AddLogProvider(logProviderRegistration);

            var serviceProvider = services.BuildServiceProvider();

            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<LoggerOptions>>();
            var options = optionsMonitor.Get(Logger.DefaultName);

            options.LogProviderRegistrations.Should().ContainSingle()
                .Which.Should().BeSameAs(logProviderRegistration);
        }

        [Fact]
        public void AddLogProviderMethodSadPath()
        {
            var builder = new LoggerBuilder(new ServiceCollection(), Logger.DefaultName, null);

            Func<IServiceProvider, ILogProvider> logProviderRegistration = null;

            Action act = () => builder.AddLogProvider(logProviderRegistration);

            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*logProviderRegistration*");
        }

        [Fact]
        public void AddContextProviderMethodHappyPath()
        {
            var services = new ServiceCollection();
            var builder = new LoggerBuilder(services, Logger.DefaultName, null);

            var contextProvider = new Mock<IContextProvider>().Object;

            Func<IServiceProvider, IContextProvider> contextProviderRegistration = sp => contextProvider;

            builder.AddContextProvider(contextProviderRegistration);

            var serviceProvider = services.BuildServiceProvider();

            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<LoggerOptions>>();
            var options = optionsMonitor.Get(Logger.DefaultName);

            options.ContextProviderRegistrations.Should().ContainSingle()
                .Which.Should().BeSameAs(contextProviderRegistration);
        }

        [Fact]
        public void AddContextProviderMethodSadPath()
        {
            var builder = new LoggerBuilder(new ServiceCollection(), Logger.DefaultName, null);

            Func<IServiceProvider, IContextProvider> contextProviderRegistration = null;

            Action act = () => builder.AddContextProvider(contextProviderRegistration);

            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*contextProviderRegistration*");
        }

        [Fact]
        public void BuildMethodHappyPath1()
        {
            // IsEmpty(options) returns false
            // ReloadOnChange is false

            var services = new ServiceCollection();

            var logProcessor = new Mock<ILogProcessor>().Object;
            services.AddSingleton(logProcessor);

            var builder = new LoggerBuilder(services, Logger.DefaultName, options => options.Level = LogLevel.Info);

            var logProvider = new Mock<ILogProvider>().Object;
            builder.AddLogProvider(sp => logProvider);

            var contextProvider = new Mock<IContextProvider>().Object;
            builder.AddContextProvider(sp => contextProvider);

            var serviceProvider = services.BuildServiceProvider();

            var logger = builder.Build(serviceProvider);

            logger.Should().BeOfType<Logger>()
                .Which.LogProcessor.Should().BeSameAs(logProcessor);
            logger.Level.Should().Be(LogLevel.Info);
            logger.LogProviders.Should().ContainSingle()
                .Which.Should().BeSameAs(logProvider);
            logger.ContextProviders.Should().ContainSingle()
                .Which.Should().BeSameAs(contextProvider);
        }

        [Fact]
        public void BuildMethodHappyPath2()
        {
            // IsEmpty(options) returns false
            // ReloadOnChange is true

            var services = new ServiceCollection();

            var logProcessor = new Mock<ILogProcessor>().Object;
            services.AddSingleton(logProcessor);

            var builder = new LoggerBuilder(services, Logger.DefaultName, options =>
            {
                options.Level = LogLevel.Info;
                options.ReloadOnChange = true;
            });

            var logProvider = new Mock<ILogProvider>().Object;
            builder.AddLogProvider(sp => logProvider);

            var contextProvider = new Mock<IContextProvider>().Object;
            builder.AddContextProvider(sp => contextProvider);

            var serviceProvider = services.BuildServiceProvider();

            var logger = builder.Build(serviceProvider);

            logger.Should().BeOfType(ReloadingLoggerTests.ReloadingLogger);
            logger.Level.Should().Be(LogLevel.Info);
            logger.LogProviders.Should().ContainSingle()
                .Which.Should().BeSameAs(logProvider);
            logger.ContextProviders.Should().ContainSingle()
                .Which.Should().BeSameAs(contextProvider);
        }

        [Fact]
        public void BuildMethodHappyPath3()
        {
            // IsEmpty(options) returns true

            var services = new ServiceCollection();

            var logProcessor = new Mock<ILogProcessor>().Object;
            services.AddSingleton(logProcessor);

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "RockLib.Logging:Level", "Warn" },
                    { "RockLib.Logging:LogProviders:Type", "RockLib.Logging.ConsoleLogProvider, RockLib.Logging" },
                    { "RockLib.Logging:LogProviders:Value:Template", "foobar" }
                })
                .Build();
            services.AddSingleton<IConfiguration>(configuration);

            var builder = new LoggerBuilder(services, Logger.DefaultName, null);

            var serviceProvider = services.BuildServiceProvider();

            var logger = builder.Build(serviceProvider);

            logger.Should().BeOfType<Logger>()
                .Which.LogProcessor.Should().BeSameAs(logProcessor);
            logger.Level.Should().Be(LogLevel.Warn);
            logger.LogProviders.Should().ContainSingle()
                .Which.Should().BeOfType<ConsoleLogProvider>()
                .Which.Formatter.Should().BeOfType<TemplateLogFormatter>()
                .Which.Template.Should().Be("foobar");
        }
    }
}
