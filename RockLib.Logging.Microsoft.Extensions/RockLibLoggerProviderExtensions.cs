using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RockLib.Logging.DependencyInjection;
using System;

namespace RockLib.Logging;

/// <summary>
/// Defines extension methods for registering <see cref="RockLibLoggerProvider"/> with a
/// <see cref="ILoggingBuilder"/> or <see cref="IServiceCollection"/>.
/// </summary>
public static class RockLibLoggerProviderExtensions
{
    /// <summary>
    /// Adds a RockLib logger named 'RockLibLogger' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configureOptions">
    /// A delegate to configure the <see cref="RockLibLogger"/>.
    /// </param>
    /// <returns>The same <see cref="ILoggingBuilder"/>.</returns>
    public static ILoggingBuilder AddRockLibLoggerProvider(this ILoggingBuilder builder,
        Action<RockLibLoggerOptions> configureOptions) =>
        builder.AddRockLibLoggerProvider(Logger.DefaultName, configureOptions);

    /// <summary>
    /// Adds a RockLib logger named 'RockLibLogger' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="rockLibLoggerName">
    /// The name of the <see cref="ILogger"/> that will ultimately records logs.
    /// </param>
    /// <param name="configureOptions">
    /// A delegate to configure the <see cref="RockLibLogger"/>.
    /// </param>
    /// <returns>The same <see cref="ILoggingBuilder"/>.</returns>
    public static ILoggingBuilder AddRockLibLoggerProvider(this ILoggingBuilder builder,
        string rockLibLoggerName = Logger.DefaultName, Action<RockLibLoggerOptions>? configureOptions = null)
    {
        if (builder is null) { throw new ArgumentNullException(nameof(builder)); }

        builder.Services.AddRockLibLoggerProvider(rockLibLoggerName, configureOptions);

        return builder;
    }

    /// <summary>
    /// Adds a <see cref="RockLibLoggerProvider"/> to the service collection.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the logger provider to.
    /// </param>
    /// <param name="configureOptions">
    /// A delegate to configure the <see cref="RockLibLogger"/>.
    /// </param>
    /// <returns>The same <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddRockLibLoggerProvider(this IServiceCollection services,
        Action<RockLibLoggerOptions> configureOptions) =>
        services.AddRockLibLoggerProvider(Logger.DefaultName, configureOptions);

    /// <summary>
    /// Adds a <see cref="RockLibLoggerProvider"/> to the service collection.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the logger provider to.
    /// </param>
    /// <param name="rockLibLoggerName">
    /// The name of the <see cref="ILogger"/> that will ultimately records logs.
    /// </param>
    /// <param name="configureOptions">
    /// A delegate to configure the <see cref="RockLibLogger"/>.
    /// </param>
    /// <returns>The same <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddRockLibLoggerProvider(this IServiceCollection services,
        string rockLibLoggerName = Logger.DefaultName, Action<RockLibLoggerOptions>? configureOptions = null)
    {
        if (services is null) { throw new ArgumentNullException(nameof(services)); }

        services.Add(ServiceDescriptor.Singleton<ILoggerProvider>(serviceProvider =>
        {
            var lookup = serviceProvider.GetRequiredService<LoggerLookup>();
            var options = serviceProvider.GetService<IOptionsMonitor<RockLibLoggerOptions>>();
            return new RockLibLoggerProvider(lookup(rockLibLoggerName), options);
        }));

        if (configureOptions is not null)
        {
            services.Configure(configureOptions);
        }

        return services;
    }
}
