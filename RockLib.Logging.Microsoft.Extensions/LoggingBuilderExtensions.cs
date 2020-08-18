using Microsoft.Extensions.DependencyInjection;
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
            Action<ILoggerBuilder> configureLogger = null,
            Action<ILoggerOptions> configureOptions = null,
            string rockLibLoggerName = null)
        {
            var loggerBuilder = new LoggerBuilder(builder.Services, rockLibLoggerName, configureOptions);
            configureLogger?.Invoke(loggerBuilder);

            builder.Services.SetLogProcessor(logProcessor);
            builder.RegisterLoggingServices(loggerBuilder, rockLibLoggerName);

            return builder;
        }

        public static ILoggingBuilder AddRockLibLogger(this ILoggingBuilder builder,
            Func<IServiceProvider, ILogProcessor> logProcessorRegistration,
            Action<ILoggerBuilder> configureLogger = null,
            Action<ILoggerOptions> configureOptions = null,
            string rockLibLoggerName = null)
        {
            var loggerBuilder = new LoggerBuilder(builder.Services, rockLibLoggerName, configureOptions);
            configureLogger?.Invoke(loggerBuilder);

            builder.Services.SetLogProcessor(logProcessorRegistration);
            builder.RegisterLoggingServices(loggerBuilder, rockLibLoggerName);

            return builder;
        }

        public static ILoggingBuilder AddRockLibLogger(this ILoggingBuilder builder,
            Action<ILoggerBuilder> configureLogger = null,
            Action<ILoggerOptions> configureOptions = null,
            string rockLibLoggerName = null,
            Logger.ProcessingMode processingMode = Logger.ProcessingMode.Background)
        {
            var loggerBuilder = new LoggerBuilder(builder.Services, rockLibLoggerName, configureOptions);
            configureLogger?.Invoke(loggerBuilder);

            builder.Services.SetLogProcessor(processingMode);
            builder.RegisterLoggingServices(loggerBuilder, rockLibLoggerName);

            return builder;
        }

        private static void RegisterLoggingServices(this ILoggingBuilder builder, LoggerBuilder loggerBuilder, string rockLibLoggerName)
        {
            builder.Services.Add(ServiceDescriptor.Singleton(loggerBuilder.Build));
            builder.Services.SetLoggerLookup();
            builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider>(serviceProvider =>
            {
                var lookup = serviceProvider.GetRequiredService<LoggerLookup>();
                var options = serviceProvider.GetService<IOptionsMonitor<RockLibLoggerOptions>>();
                return new RockLibLoggerProvider(lookup(rockLibLoggerName), options);
            }));
        }
    }
}
