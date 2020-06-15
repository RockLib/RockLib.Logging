#if !NET451
using Microsoft.Extensions.DependencyInjection;
using RockLib.Logging.LogProcessing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RockLib.Logging.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        // TODO: Add remarks stating that if multiple loggers are added, the last one defines the log processor for all.
        public static ILoggerBuilder AddRockLibLogging(this IServiceCollection services,
            ILogProcessor logProcessor,
            string loggerName = Logger.DefaultName,
            Action<LoggerOptions> configureOptions = null,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var builder = new LoggerBuilder(loggerName, configureOptions);

            services.SetLogProcessor(logProcessor);
            services.Add(new ServiceDescriptor(typeof(ILogger), builder.Build, lifetime));
            services.SetLoggerLookupDescriptor();

            return builder;
        }

        // TODO: Add remarks stating that if multiple loggers are added, the last one defines the log processor for all.
        public static ILoggerBuilder AddRockLibLogging(this IServiceCollection services,
            Func<IServiceProvider, ILogProcessor> logProcessorFactory,
            string loggerName = Logger.DefaultName,
            Action<LoggerOptions> configureOptions = null,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var builder = new LoggerBuilder(loggerName, configureOptions);

            services.SetLogProcessor(logProcessorFactory);
            services.Add(new ServiceDescriptor(typeof(ILogger), builder.Build, lifetime));
            services.SetLoggerLookupDescriptor();

            return builder;
        }

        // TODO: Add remarks stating that if multiple loggers are added, the last one defines the log processor for all.
        public static ILoggerBuilder AddRockLibLogging(this IServiceCollection services,
            string loggerName = Logger.DefaultName,
            Action<LoggerOptions> configureOptions = null,
            Logger.ProcessingMode processingMode = Logger.ProcessingMode.Background,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var builder = new LoggerBuilder(loggerName, configureOptions);

            services.SetLogProcessor(processingMode);
            services.Add(new ServiceDescriptor(typeof(ILogger), builder.Build, lifetime));
            services.SetLoggerLookupDescriptor();

            return builder;
        }

        private static void SetLogProcessor(this IServiceCollection services, ILogProcessor logProcessor)
        {
            services.ClearLogProcessor();
            services.AddSingleton(logProcessor);
        }


        private static void SetLogProcessor(this IServiceCollection services, Func<IServiceProvider, ILogProcessor> logProcessorFactory)
        {
            services.ClearLogProcessor();
            services.AddSingleton(logProcessorFactory);
        }

        private static void SetLogProcessor(this IServiceCollection services, Logger.ProcessingMode processingMode)
        {
            ClearLogProcessor(services);
            switch (processingMode)
            {
                case Logger.ProcessingMode.Background:
                    services.AddSingleton<ILogProcessor, BackgroundLogProcessor>();
                    break;
                case Logger.ProcessingMode.Synchronous:
                    services.AddSingleton<ILogProcessor, SynchronousLogProcessor>();
                    break;
                case Logger.ProcessingMode.FireAndForget:
                    services.AddSingleton<ILogProcessor, FireAndForgetLogProcessor>();
                    break;
            }
        }

        private static void ClearLogProcessor(this IServiceCollection services)
        {
            for (int i = 0; i < services.Count; i++)
                if (services[i].ServiceType == typeof(ILogProcessor))
                    services.RemoveAt(i--);
        }

        private static void SetLoggerLookupDescriptor(this IServiceCollection services)
        {
            // Clear the existing LoggerLookup descriptor, if it exists.
            for (int i = 0; i < services.Count; i++)
                if (services[i].ServiceType == typeof(LoggerLookup))
                    services.RemoveAt(i--);

            // Capture which loggers are singleton according to index.
            IReadOnlyList<bool> isSingletonLogger = services.Where(service => service.ServiceType == typeof(ILogger))
                .Select(service => service.Lifetime == ServiceLifetime.Singleton)
                .ToArray();

            LoggerLookup LoggerLookupFactory(IServiceProvider serviceProvider) => name =>
            {
                // Select the first logger that has a matching name.
                var loggers = serviceProvider.GetServices<ILogger>().ToArray();
                var selectedLogger = loggers.FirstOrDefault(logger => NamesEqual(logger.Name, name));

                // Immediately dispose any non-singleton loggers that weren't selected.
                for (int i = 0; i < loggers.Length; i++)
                    if (!isSingletonLogger[i] && !ReferenceEquals(loggers[i], selectedLogger))
                        loggers[i].Dispose();

                return selectedLogger;
            };

            services.AddSingleton(LoggerLookupFactory);
        }

        internal static bool NamesEqual(string loggerName, string lookupName)
        {
            if (string.Equals(loggerName, lookupName, StringComparison.OrdinalIgnoreCase))
                return true;

            if (lookupName is null)
                return string.Equals(loggerName, Logger.DefaultName, StringComparison.OrdinalIgnoreCase);

            return false;
        }
    }
}
#endif
