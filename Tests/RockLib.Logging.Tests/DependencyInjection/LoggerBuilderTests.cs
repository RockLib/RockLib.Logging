using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using RockLib.Logging.DependencyInjection;
using RockLib.Logging.LogProcessing;
using RockLib.Logging.LogProviders;
using System;
using System.Collections.Generic;
using Xunit;

namespace RockLib.Logging.Tests.DependencyInjection
{
    public class LoggerBuilderTests
    {
        [Fact(DisplayName = "Constructor sets properties")]
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

        [Fact(DisplayName = "Constructor sets LoggerName to Logger.DefaultName if name parameter is null")]
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

        [Fact(DisplayName = "Constructor throws if services parameter is null")]
        public void ConstructorSadPath()
        {
            IServiceCollection services = null;
            var name = "foo";
            Action<ILoggerOptions> configureOptions = o => o.Level = LogLevel.Info;

            Action act = () => new LoggerBuilder(services, name, configureOptions);

            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*services*");
        }

        [Fact(DisplayName = "AddLogProvider method adds logProviderRegistration parameter to configured LoggerOptions")]
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

        [Fact(DisplayName = "AddLogProvider method throws if logProviderRegistration parameter is null")]
        public void AddLogProviderMethodSadPath()
        {
            var builder = new LoggerBuilder(new ServiceCollection(), Logger.DefaultName, null);

            Func<IServiceProvider, ILogProvider> logProviderRegistration = null;

            Action act = () => builder.AddLogProvider(logProviderRegistration);

            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*logProviderRegistration*");
        }

        [Fact(DisplayName = "AddContextProvider method adds contextProviderRegistration parameter to configured LoggerOptions")]
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

        [Fact(DisplayName = "AddContextProvider method throws if contextProviderRegistration parameter is null")]
        public void AddContextProviderMethodSadPath()
        {
            var builder = new LoggerBuilder(new ServiceCollection(), Logger.DefaultName, null);

            Func<IServiceProvider, IContextProvider> contextProviderRegistration = null;

            Action act = () => builder.AddContextProvider(contextProviderRegistration);

            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*contextProviderRegistration*");
        }

        [Theory(DisplayName = "Build method returns Logger instance given non-empty options")]
        [InlineData(true, LogLevel.Error), InlineData(false, LogLevel.Info)]
        public void BuildMethodHappyPath1(bool customLogLevelResolver, LogLevel expectedLogLevel)
        {
            var services = new ServiceCollection();

            var logProcessor = new Mock<ILogProcessor>().Object;
            services.AddSingleton(logProcessor);

            var logLevelResolver = new Mock<ILogLevelResolver>();
            logLevelResolver.Setup(i => i.GetLogLevel()).Returns(LogLevel.Error);
            if (customLogLevelResolver)
            {
                services.AddSingleton(logLevelResolver.Object);
            }

            var builder = new LoggerBuilder(services, Logger.DefaultName, options => options.Level = LogLevel.Info);

            var logProvider = new Mock<ILogProvider>().Object;
            builder.AddLogProvider(sp => logProvider);

            var contextProvider = new Mock<IContextProvider>().Object;
            builder.AddContextProvider(sp => contextProvider);

            var serviceProvider = services.BuildServiceProvider();

            var logger = builder.Build(serviceProvider);

            logger.Should().BeOfType<Logger>()
                .Which.LogProcessor.Should().BeSameAs(logProcessor);
            logger.Level.Should().Be(expectedLogLevel);
            logger.LogProviders.Should().ContainSingle()
                .Which.Should().BeSameAs(logProvider);
            logger.ContextProviders.Should().ContainSingle()
                .Which.Should().BeSameAs(contextProvider);
        }

        [Theory(DisplayName = "Build method returns ReloadingLogger instance given non-empty options where ReloadOnChange is true")]
        [InlineData(true, LogLevel.Error), InlineData(false, LogLevel.Info)]
        public void BuildMethodHappyPath2(bool customLogLevelResolver, LogLevel expectedLogLevel)
        {
            var services = new ServiceCollection();

            var logProcessor = new Mock<ILogProcessor>().Object;
            services.AddSingleton(logProcessor);

            var logLevelResolver = new Mock<ILogLevelResolver>();
            logLevelResolver.Setup(i => i.GetLogLevel()).Returns(LogLevel.Error);
            if (customLogLevelResolver)
            {
                services.AddSingleton(logLevelResolver.Object);
            }

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
            logger.Level.Should().Be(expectedLogLevel);
            logger.LogProviders.Should().ContainSingle()
                .Which.Should().BeSameAs(logProvider);
            logger.ContextProviders.Should().ContainSingle()
                .Which.Should().BeSameAs(contextProvider);
        }

        [Theory(DisplayName = "Build method returns logger created from configuration given empty options")]
        [InlineData(true, LogLevel.Error), InlineData(false, LogLevel.Warn)]
        public void BuildMethodHappyPath3(bool customLogLevelResolver, LogLevel expectedLogLevel)
        {
            var services = new ServiceCollection();

            var logProcessor = new Mock<ILogProcessor>().Object;
            services.AddSingleton(logProcessor);

            var logLevelResolver = new Mock<ILogLevelResolver>();
            logLevelResolver.Setup(i => i.GetLogLevel()).Returns(LogLevel.Error);
            if (customLogLevelResolver)
            {
                services.AddSingleton(logLevelResolver.Object);
            }

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
            logger.Level.Should().Be(expectedLogLevel);
            logger.LogProviders.Should().ContainSingle()
                .Which.Should().BeOfType<ConsoleLogProvider>()
                .Which.Formatter.Should().BeOfType<TemplateLogFormatter>()
                .Which.Template.Should().Be("foobar");
        }
    }
}
