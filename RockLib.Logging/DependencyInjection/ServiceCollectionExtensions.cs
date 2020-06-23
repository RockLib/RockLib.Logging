#if !NET451
using Microsoft.Extensions.DependencyInjection;
using RockLib.Logging.LogProcessing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RockLib.Logging.DependencyInjection
{
    /// <summary>
    /// Defines extension methods for <see cref="IServiceCollection"/> that add loggers to a
    /// service collection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds an <see cref="ILogger"/>, along with its associated services, to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/> to add the logger to.
        /// </param>
        /// <param name="logProcessor">The object that will process log entries on behalf of the logger.</param>
        /// <param name="loggerName">The name of the logger to build.</param>
        /// <param name="configureOptions">
        /// A delegate to configure the <see cref="ILoggerOptions"/> object that is used to configure the
        /// logger.
        /// </param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the service.</param>
        /// <returns>
        /// An <see cref="ILoggerBuilder"/> object for adding log providers and context providers.
        /// </returns>
        /// <remarks>
        /// Note that if this method or any of its overloads are called more than once, then the
        /// last call defines the log processor for all.
        /// </remarks>
        public static ILoggerBuilder AddLogger(this IServiceCollection services,
            ILogProcessor logProcessor,
            string loggerName = Logger.DefaultName,
            Action<ILoggerOptions> configureOptions = null,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));
            if (logProcessor is null)
                throw new ArgumentNullException(nameof(logProcessor));

            var builder = new LoggerBuilder(services, loggerName, configureOptions);

            services.SetLogProcessor(logProcessor);
            services.Add(new ServiceDescriptor(typeof(ILogger), builder.Build, lifetime));
            services.SetLoggerLookupDescriptor();

            return builder;
        }

        /// <summary>
        /// Adds an <see cref="ILogger"/>, along with its associated services, to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/> to add the logger to.
        /// </param>
        /// <param name="logProcessorRegistration">
        /// The method used to create the <see cref="ILogProcessor"/> that will process log entries
        /// on behalf of the logger.
        /// </param>
        /// <param name="loggerName">The name of the logger to build.</param>
        /// <param name="configureOptions">
        /// A delegate to configure the <see cref="ILoggerOptions"/> object that is used to configure the
        /// logger.
        /// </param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the service.</param>
        /// <returns>
        /// An <see cref="ILoggerBuilder"/> object for adding log providers and context providers.
        /// </returns>
        /// <remarks>
        /// Note that if this method or any of its overloads are called more than once, then the
        /// last call defines the log processor for all.
        /// </remarks>
        public static ILoggerBuilder AddLogger(this IServiceCollection services,
            Func<IServiceProvider, ILogProcessor> logProcessorRegistration,
            string loggerName = Logger.DefaultName,
            Action<ILoggerOptions> configureOptions = null,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));
            if (logProcessorRegistration is null)
                throw new ArgumentNullException(nameof(logProcessorRegistration));

            var builder = new LoggerBuilder(services, loggerName, configureOptions);

            services.SetLogProcessor(logProcessorRegistration);
            services.Add(new ServiceDescriptor(typeof(ILogger), builder.Build, lifetime));
            services.SetLoggerLookupDescriptor();

            return builder;
        }

        /// <summary>
        /// Adds an <see cref="ILogger"/>, along with its associated services, to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/> to add the logger to.
        /// </param>
        /// <param name="loggerName">The name of the logger to build.</param>
        /// <param name="configureOptions">
        /// A delegate to configure the <see cref="ILoggerOptions"/> object that is used to configure the
        /// logger.
        /// </param>
        /// <param name="processingMode">A value that indicates how the logger will process logs.</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the service.</param>
        /// <returns>
        /// An <see cref="ILoggerBuilder"/> object for adding log providers and context providers.
        /// </returns>
        /// <remarks>
        /// Note that if this method or any of its overloads are called more than once, then the
        /// last call defines the log processor for all.
        /// </remarks>
        public static ILoggerBuilder AddLogger(this IServiceCollection services,
            string loggerName = Logger.DefaultName,
            Action<ILoggerOptions> configureOptions = null,
            Logger.ProcessingMode processingMode = Logger.ProcessingMode.Background,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            var builder = new LoggerBuilder(services, loggerName, configureOptions);

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

        private static void SetLogProcessor(this IServiceCollection services, Func<IServiceProvider, ILogProcessor> logProcessorRegistration)
        {
            services.ClearLogProcessor();
            services.AddSingleton(logProcessorRegistration);
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(processingMode));
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

            LoggerLookup LoggerLookupRegistration(IServiceProvider serviceProvider) => name =>
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

            services.AddSingleton(LoggerLookupRegistration);
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
