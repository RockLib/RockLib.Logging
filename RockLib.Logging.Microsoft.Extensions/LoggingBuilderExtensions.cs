using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RockLib.Logging.DependencyInjection;
using RockLib.Logging.DependencyInjection.Internal;
using RockLib.Logging.LogProcessing;
using System;

namespace RockLib.Logging
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddRockLibLogger(this ILoggingBuilder builder,
            ILogProcessor logProcessor,
            string name = null,
            Action<ILoggerBuilder> configureLogger = null,
            Action<ILoggerOptions> configureOptions = null)
        {
            var loggerBuilder = new LoggerBuilder(builder.Services, name, configureOptions);
            configureLogger?.Invoke(loggerBuilder);

            builder.Services.SetLogProcessor(logProcessor);
            RegisterLoggingServices(builder, loggerBuilder, name);

            return builder;
        }

        public static ILoggingBuilder AddRockLibLogger(this ILoggingBuilder builder,
            Func<IServiceProvider, ILogProcessor> logProcessorRegistration,
            string name = null,
            Action<ILoggerBuilder> configureLogger = null,
            Action<ILoggerOptions> configureOptions = null)
        {
            var loggerBuilder = new LoggerBuilder(builder.Services, name, configureOptions);
            configureLogger?.Invoke(loggerBuilder);

            builder.Services.SetLogProcessor(logProcessorRegistration);
            RegisterLoggingServices(builder, loggerBuilder, name);

            return builder;
        }

        public static ILoggingBuilder AddRockLibLogger(this ILoggingBuilder builder,
            string name = null,
            Action<ILoggerBuilder> configureLogger = null,
            Action<ILoggerOptions> configureOptions = null,
            Logger.ProcessingMode processingMode = Logger.ProcessingMode.Background)
        {
            var loggerBuilder = new LoggerBuilder(builder.Services, name, configureOptions);
            configureLogger?.Invoke(loggerBuilder);

            builder.Services.SetLogProcessor(processingMode);
            RegisterLoggingServices(builder, loggerBuilder, name);

            return builder;
        }

        private static void RegisterLoggingServices(ILoggingBuilder builder, LoggerBuilder loggerBuilder, string name)
        {
            builder.Services.Add(ServiceDescriptor.Singleton(loggerBuilder.Build));
            builder.Services.SetLoggerLookup();
            builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider>(serviceProvider =>
            {
                var lookup = serviceProvider.GetRequiredService<LoggerLookup>();
                var options = serviceProvider.GetRequiredService<IOptionsMonitor<RockLibLoggerOptions>>();
                return new RockLibLoggerProvider(lookup(name), options);
            }));
        }
    }
}
