using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RockLib.Logging.DependencyInjection;
using System;

namespace RockLib.Logging
{
    public static class RockLibLoggerExtensions
    {
        public static ILoggingBuilder AddRockLibLoggerProvider(this ILoggingBuilder builder,
            Action<RockLibLoggerOptions> configureOptions) =>
            builder.AddRockLibLoggerProvider(Logger.DefaultName, configureOptions);

        public static ILoggingBuilder AddRockLibLoggerProvider(this ILoggingBuilder builder,
            string rockLibLoggerName = Logger.DefaultName, Action<RockLibLoggerOptions> configureOptions = null)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddRockLibLoggerProvider(rockLibLoggerName, configureOptions);

            return builder;
        }

        public static IServiceCollection AddRockLibLoggerProvider(this IServiceCollection services,
            Action<RockLibLoggerOptions> configureOptions) =>
            services.AddRockLibLoggerProvider(Logger.DefaultName, configureOptions);

        public static IServiceCollection AddRockLibLoggerProvider(this IServiceCollection services,
            string rockLibLoggerName = Logger.DefaultName, Action<RockLibLoggerOptions> configureOptions = null)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            services.Add(ServiceDescriptor.Singleton<ILoggerProvider>(serviceProvider =>
            {
                var lookup = serviceProvider.GetRequiredService<LoggerLookup>();
                var options = serviceProvider.GetService<IOptionsMonitor<RockLibLoggerOptions>>();
                return new RockLibLoggerProvider(lookup(rockLibLoggerName), options);
            }));

            if (configureOptions != null)
                services.Configure(configureOptions);

            return services;
        }
    }
}
