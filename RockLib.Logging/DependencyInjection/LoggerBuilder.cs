#if !NET451
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RockLib.Configuration;
using RockLib.Configuration.ObjectFactory;
using RockLib.Logging.LogProcessing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RockLib.Logging.DependencyInjection
{
    public class LoggerBuilder : ILoggerBuilder
    {
        public LoggerBuilder(string loggerName, Action<LoggerOptions> configureOptions)
        {
            LoggerName = loggerName ?? Logger.DefaultName;
            ConfigureOptions = configureOptions;
        }

        public string LoggerName { get; }

        public Action<LoggerOptions> ConfigureOptions { get; }

        public IList<LogProviderRegistration> LogProviders { get; } = new List<LogProviderRegistration>();

        public IList<ContextProviderRegistration> ContextProviders { get; } = new List<ContextProviderRegistration>();

        public ILoggerBuilder AddLogProvider(LogProviderRegistration registration)
        {
            LogProviders.Add(registration);
            return this;
        }

        public ILoggerBuilder AddContextProvider(ContextProviderRegistration registration)
        {
            ContextProviders.Add(registration);
            return this;
        }

        public ILogger Build(IServiceProvider serviceProvider)
        {
            var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<LoggerOptions>>();
            var options = optionsMonitor?.Get(LoggerName) ?? new LoggerOptions();            
            ConfigureOptions?.Invoke(options);

            if (IsEmpty(options))
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var resolver = new Resolver(serviceProvider.GetService);

                return configuration.GetCompositeSection(LoggerFactory.AlternateSectionName, LoggerFactory.SectionName)
                    .CreateLogger(name: LoggerName, resolver: resolver, reloadOnConfigChange: options.ReloadOnChange);
            }

            var logProcessor = serviceProvider.GetRequiredService<ILogProcessor>();
            var logProviders = LogProviders.Select(createLogProvider => createLogProvider(serviceProvider)).ToArray();
            var contextProviders = ContextProviders.Select(createContextProvider => createContextProvider(serviceProvider)).ToArray();

            if (optionsMonitor != null && options.ReloadOnChange)
                return new OptionsMonitorReloadingLogger(logProcessor, LoggerName, logProviders, contextProviders, optionsMonitor, options);
            
            return new Logger(logProcessor, LoggerName, logProviders: logProviders, contextProviders: contextProviders);
        }

        private bool IsEmpty(LoggerOptions options) =>
            LogProviders.Count == 0 && ContextProviders.Count == 0 && options.Level == null && options.IsDisabled == null;
    }
}
#endif
