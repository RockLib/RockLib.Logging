#if !NET451
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RockLib.Configuration;
using RockLib.Configuration.ObjectFactory;
using RockLib.Logging.LogProcessing;
using System;
using System.Linq;

namespace RockLib.Logging.DependencyInjection
{
    /// <summary>
    /// The default implementation of the <see cref="ILoggerBuilder"/> interface.
    /// </summary>
    public class LoggerBuilder : ILoggerBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerBuilder"/> class.
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/> that is configured when the <see cref="AddLogProvider"/> and
        /// <see cref="AddContextProvider"/> methods are called.
        /// </param>
        /// <param name="loggerName">The name of the logger to build.</param>
        /// <param name="configureOptions">
        /// A delegate to configure the <see cref="ILoggerOptions"/> object that is used to configure the
        /// logger.
        /// </param>
        public LoggerBuilder(IServiceCollection services, string loggerName, Action<ILoggerOptions> configureOptions)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            LoggerName = loggerName ?? Logger.DefaultName;
            ConfigureOptions = configureOptions;
        }

        /// <summary>
        /// The <see cref="IServiceCollection"/> that is configured when the <see cref="AddLogProvider"/> and
        /// <see cref="AddContextProvider"/> methods are called.
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// The name of the logger to build.
        /// </summary>
        public string LoggerName { get; }

        /// <summary>
        /// The delegate to configure the <see cref="ILoggerOptions"/> object that is used to configure the
        /// logger.
        /// </summary>
        public Action<ILoggerOptions> ConfigureOptions { get; }

        /// <summary>
        /// Adds an <see cref="ILogProvider"/> to the logger.
        /// </summary>
        /// <param name="logProviderRegistration">A method that creates the <see cref="ILogProvider"/>.</param>
        /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
        public ILoggerBuilder AddLogProvider(Func<IServiceProvider, ILogProvider> logProviderRegistration)
        {
            if (logProviderRegistration is null)
                throw new ArgumentNullException(nameof(logProviderRegistration));

            Services.Configure<LoggerOptions>(LoggerName, options => options.LogProviderRegistrations.Add(logProviderRegistration));
            return this;
        }

        /// <summary>
        /// Adds an <see cref="IContextProvider"/> to the logger.
        /// </summary>
        /// <param name="contextProviderRegistration">A method that creates the <see cref="IContextProvider"/>.</param>
        /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
        public ILoggerBuilder AddContextProvider(Func<IServiceProvider, IContextProvider> contextProviderRegistration)
        {
            if (contextProviderRegistration is null)
                throw new ArgumentNullException(nameof(contextProviderRegistration));

            Services.Configure<LoggerOptions>(LoggerName, options => options.ContextProviderRegistrations.Add(contextProviderRegistration));
            return this;
        }

        /// <summary>
        /// Creates an instance of <see cref="ILogger"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// The <see cref="IServiceProvider"/> that retrieves the services required to create the <see cref="ILogger"/>.
        /// </param>
        /// <returns>An instance of <see cref="ILogger"/>.</returns>
        public ILogger Build(IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
                throw new ArgumentNullException(nameof(serviceProvider));

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
            var logProviders = options.LogProviderRegistrations.Select(createLogProvider => createLogProvider(serviceProvider)).ToArray();
            var contextProviders = options.ContextProviderRegistrations.Select(createContextProvider => createContextProvider(serviceProvider)).ToArray();

            if (optionsMonitor != null && options.ReloadOnChange)
                return new ReloadingLogger(logProcessor, LoggerName, logProviders, contextProviders, optionsMonitor, options);
            
            return new Logger(logProcessor, LoggerName, options.Level.GetValueOrDefault(),
                logProviders, options.IsDisabled.GetValueOrDefault(), contextProviders);
        }

        private bool IsEmpty(LoggerOptions options) =>
            options.LogProviderRegistrations.Count == 0 && options.ContextProviderRegistrations.Count == 0
                && options.Level == null && options.IsDisabled == null;
    }
}
#endif
