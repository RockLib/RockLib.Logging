using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RockLib.Configuration.ObjectFactory;
using RockLib.Logging.LogProcessing;
using System;
using System.Linq;

namespace RockLib.Logging.DependencyInjection
{
    /// <summary>
    /// Defines extension methods for adding RockLib.Logging to a <see cref="IServiceCollection"/>.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds a transient <see cref="ILogger"/> service, created with <see cref="LoggerFactory"/>, to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="rockLibLoggerName">The name of the RockLib logger used for logging.</param>
        /// <param name="defaultTypes">
        /// An object that defines the default types to be used when a type is not explicitly specified by a
        /// configuration section.
        /// </param>
        /// <param name="valueConverters">
        /// An object that defines custom converter functions that are used to convert string configuration
        /// values to a target type.
        /// </param>
        /// <param name="addLoggerProvider">
        /// Whether to also add a singleton <see cref="ILoggerProvider"/> service with a <see cref="RockLibLoggerProvider"/>
        /// implementation to the service collection.
        /// </param>
        /// <param name="addBackgroundLogProcessor">
        /// Whether to also add a singleton <see cref="ILogProcessor"/> service with a <see cref="BackgroundLogProcessor"/>
        /// implementation to the service collection.
        /// </param>
        /// <param name="reloadOnConfigChange">
        /// Whether instances of <see cref="ILogger"/> created by the service collection will reload when their
        /// configuration changes.
        /// </param>
        /// <returns>The same <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRockLibLoggerTransient(this IServiceCollection services,
            string rockLibLoggerName = Logger.DefaultName, DefaultTypes defaultTypes = null, ValueConverters valueConverters = null,
            bool addLoggerProvider = false, bool addBackgroundLogProcessor = true, bool reloadOnConfigChange = false)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (services.Any(s => s.ServiceType == typeof(ILogger)))
                throw new ArgumentException("RockLib.Logging.ILogger has already been added to the service collection.", nameof(services));

            if (addBackgroundLogProcessor)
                services.AddSingleton<ILogProcessor, BackgroundLogProcessor>();

            services.AddTransient(serviceProvider =>
            {
                var resolver = new Resolver(type => serviceProvider.GetService(type));
                return LoggerFactory.Create(rockLibLoggerName, defaultTypes, valueConverters, resolver, reloadOnConfigChange);
            });

            if (addLoggerProvider)
                services.AddRockLibLoggerProviderSingleton();

            return services;
        }

        /// <summary>
        /// Adds a singleton <see cref="ILogger"/> service, created with <see cref="LoggerFactory"/>, to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="rockLibLoggerName">The name of the RockLib logger used for logging.</param>
        /// <param name="defaultTypes">
        /// An object that defines the default types to be used when a type is not explicitly specified by a
        /// configuration section.
        /// </param>
        /// <param name="valueConverters">
        /// An object that defines custom converter functions that are used to convert string configuration
        /// values to a target type.
        /// </param>
        /// <param name="addLoggerProvider">
        /// Whether to also add a singleton <see cref="ILoggerProvider"/> service with a <see cref="RockLibLoggerProvider"/>
        /// implementation to the service collection.
        /// </param>
        /// <param name="reloadOnConfigChange">
        /// Whether instances of <see cref="ILogger"/> created by the service collection will reload when their
        /// configuration changes.
        /// </param>
        /// <returns>The same <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRockLibLoggerSingleton(this IServiceCollection services, string rockLibLoggerName = Logger.DefaultName,
            DefaultTypes defaultTypes = null, ValueConverters valueConverters = null, bool addLoggerProvider = false, bool reloadOnConfigChange = true)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (services.Any(s => s.ServiceType == typeof(ILogger)))
                throw new ArgumentException("RockLib.Logging.ILogger has already been added to the service collection.", nameof(services));

            services.AddSingleton(serviceProvider =>
            {
                var resolver = new Resolver(type => serviceProvider.GetService(type));
                return LoggerFactory.Create(rockLibLoggerName, defaultTypes, valueConverters, resolver, reloadOnConfigChange);
            });

            if (addLoggerProvider)
                services.AddRockLibLoggerProviderSingleton();

            return services;
        }

        /// <summary>
        /// Adds a singleton <see cref="ILogger"/> service  to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="logger">The instance of <see cref="ILogger"/> to add.</param>
        /// <param name="addLoggerProvider">
        /// Whether to also add a singleton <see cref="ILoggerProvider"/> service with a <see cref="RockLibLoggerProvider"/>
        /// implementation to the service collection.
        /// </param>
        /// <returns>The same <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRockLibLoggerSingleton(this IServiceCollection services, ILogger logger,
            bool addLoggerProvider = false)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (services.Any(s => s.ServiceType == typeof(ILogger)))
                throw new ArgumentException("RockLib.Logging.ILogger has already been added to the service collection.", nameof(services));

            services.AddSingleton(logger);

            if (addLoggerProvider)
                services.AddRockLibLoggerProviderSingleton();

            return services;
        }

        private static void AddRockLibLoggerProviderSingleton(this IServiceCollection services) =>
            services.AddSingleton<ILoggerProvider>(serviceProvider => new RockLibLoggerProvider(() => serviceProvider.GetRequiredService<ILogger>()));
    }
}
