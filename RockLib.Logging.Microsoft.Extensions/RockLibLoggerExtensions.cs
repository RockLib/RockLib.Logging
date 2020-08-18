using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RockLib.Logging.DependencyInjection;
using System;

namespace RockLib.Logging
{
    public static class RockLibLoggerExtensions
    {
        public static ILoggingBuilder AddRockLibLoggerProvider(this ILoggingBuilder builder, string rockLibLoggerName = Logger.DefaultName)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddRockLibLoggerProvider(rockLibLoggerName);

            return builder;
        }

        public static IServiceCollection AddRockLibLoggerProvider(this IServiceCollection services, string rockLibLoggerName = Logger.DefaultName)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            services.Add(ServiceDescriptor.Singleton<ILoggerProvider>(serviceProvider =>
            {
                var lookup = serviceProvider.GetRequiredService<LoggerLookup>();
                var options = serviceProvider.GetService<IOptionsMonitor<RockLibLoggerOptions>>();
                return new RockLibLoggerProvider(lookup(rockLibLoggerName), options);
            }));

            return services;
        }
    }
}
