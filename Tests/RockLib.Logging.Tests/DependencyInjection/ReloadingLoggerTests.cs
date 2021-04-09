using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using RockLib.Dynamic;
using RockLib.Logging.DependencyInjection;
using RockLib.Logging.LogProcessing;
using System;
using System.Collections.Generic;
using Xunit;

namespace RockLib.Logging.Tests.DependencyInjection
{
    public class ReloadingLoggerTests
    {
        public static readonly Type ReloadingLogger =
            Type.GetType("RockLib.Logging.DependencyInjection.ReloadingLogger, RockLib.Logging", true);

        [Fact(DisplayName = "Constructor sets fields and properties")]
        public void ConstructorHappyPath()
        {
            var source = new TestConfigurationSource();
            source.Provider.Set("CustomLogger:Level", "Fatal");

            var configuration = new ConfigurationBuilder()
                .Add(source)
                .Build();

            var services = new ServiceCollection();
            services.Configure<LoggerOptions>("MyLogger", configuration.GetSection("CustomLogger"));

            var serviceProvider = services.BuildServiceProvider();

            var logProcessor = new Mock<ILogProcessor>().Object;
            var name = "MyLogger";
            var logProviders = new[] { new Mock<ILogProvider>().Object };
            var contextProviders = new[] { new Mock<IContextProvider>().Object };
            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<LoggerOptions>>();
            var options = optionsMonitor.Get("MyLogger");
            Action<LoggerOptions> configureOptions = opt => { };

            var reloadingLogger = ReloadingLogger.New(logProcessor, name, logProviders, contextProviders, optionsMonitor, options, configureOptions);

            string actualName = reloadingLogger.Name;
            ILogProcessor actualLogProcessor = reloadingLogger._logProcessor;
            IReadOnlyCollection<ILogProvider> actualLogProviders = reloadingLogger._logProviders;
            IReadOnlyCollection<IContextProvider> actualContextProviders = reloadingLogger._contextProviders;
            Action<LoggerOptions> actualConfigureOptions = reloadingLogger._configureOptions;

            actualName.Should().BeSameAs(name);
            actualLogProcessor.Should().BeSameAs(logProcessor);
            actualLogProviders.Should().BeSameAs(logProviders);
            actualContextProviders.Should().BeSameAs(contextProviders);
            actualConfigureOptions.Should().BeSameAs(configureOptions);

            Logger logger = reloadingLogger._logger;
            logger.Name.Should().Be(name);
            logger.Level.Should().Be(LogLevel.Fatal);
            logger.IsDisabled.Should().BeFalse();
            logger.LogProviders.Should().BeSameAs(logProviders);
            logger.ContextProviders.Should().BeSameAs(contextProviders);
            logger.LogProcessor.Should().BeSameAs(logProcessor);
        }

        [Fact(DisplayName = "_logger field is reinstantiated when options monitor changes")]
        public void ReloadHappyPath()
        {
            var configSource = new TestConfigurationSource();
            configSource.Provider.Set("CustomLogger:Level", "Warn");

            var configuration = new ConfigurationBuilder()
                .Add(configSource)
                .Build();

            var services = new ServiceCollection();
            services.Configure<LoggerOptions>("MyLogger", configuration.GetSection("CustomLogger"));

            var serviceProvider = services.BuildServiceProvider();

            var logProcessor = new Mock<ILogProcessor>().Object;
            var name = "MyLogger";
            var logProviders = new[] { new Mock<ILogProvider>().Object };
            var contextProviders = new[] { new Mock<IContextProvider>().Object };
            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<LoggerOptions>>();
            var options = optionsMonitor.Get("MyLogger");
            Action<LoggerOptions> configureOptions = null;

            var reloadingLogger = ReloadingLogger.New(logProcessor, name, logProviders, contextProviders, optionsMonitor, options, configureOptions);

            var errorHandler = new Mock<IErrorHandler>().Object;
            reloadingLogger.ErrorHandler = errorHandler;

            ILogger logger1 = reloadingLogger._logger;
            logger1.Level.Should().Be(LogLevel.Warn);
            logger1.IsDisabled.Should().BeFalse();
            logger1.ErrorHandler.Should().BeSameAs(errorHandler);

            configSource.Provider.Set("CustomLogger:Level", "Debug");
            configSource.Provider.Set("CustomLogger:IsDisabled", "true");
            configSource.Provider.Reload();

            ILogger logger2 = reloadingLogger._logger;
            logger2.Should().NotBeSameAs(logger1);
            logger2.Level.Should().Be(LogLevel.Debug);
            logger2.IsDisabled.Should().BeTrue();
            logger2.ErrorHandler.Should().BeSameAs(errorHandler);
        }
    }
}
