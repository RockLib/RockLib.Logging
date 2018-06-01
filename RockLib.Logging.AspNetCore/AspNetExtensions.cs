using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RockLib.Configuration;

namespace RockLib.Logging.AspNetCore
{
    /// <summary>
    /// Extensions for ASP.NET dependency injection
    /// </summary>
    public static class AspNetExtensions
    {
        /// <summary>
        /// Adds an instance of <see cref="Logger"/> retrieved from <see cref="LoggerFactory"/> and an
        /// instance of <see cref="ILoggerProvider"/> that uses that logger to the specified
        /// <see cref="IServiceCollection"/> as singletons.
        /// </summary>
        /// <param name="builder">The IWebHostBuilder being extended.</param>
        /// <param name="rockLibLoggerName">The name of the RockLib logger used for logging.</param>
        /// <returns>IWebHostBuilder for chaining</returns>
        /// <remarks>
        /// This method has a side-effect of calling the <see cref="Config.SetRoot(IConfiguration)"/>
        /// method, passing it the instance of <see cref="IConfiguration"/> obtained from the local
        /// <see cref="IServiceProvider"/>.
        /// </remarks>
        public static IWebHostBuilder UseRockLib(this IWebHostBuilder builder, string rockLibLoggerName = Logger.DefaultName)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.ConfigureServices(services =>
            {
                services.AddLoggerFromLoggerFactory(rockLibLoggerName);
                services.AddRockLibLoggerProvider();
            });

            return builder;
        }

        /// <summary>
        /// Adds the specified instance of <see cref="ILogger"/> and an instance of <see cref="ILoggerProvider"/> that uses
        /// that logger to the specified <see cref="IServiceCollection"/> as singletons.
        /// </summary>
        /// <param name="builder">The IWebHostBuilder being extended.</param>
        /// <param name="logger">The RockLib logger used for logging.</param>
        /// <returns>IWebHostBuilder for chaining</returns>
        /// <remarks>
        /// This extension method, unlike the <see cref="UseRockLib(IWebHostBuilder, string)"/> overload, does not
        /// have any side-effects. As such, applications using this extension method many need to call the
        /// <see cref="Config.SetRoot(IConfiguration)"/> method in the constructor of their <code>Startup</code> class.
        /// </remarks>
        public static IWebHostBuilder UseRockLib(this IWebHostBuilder builder, ILogger logger)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            builder.ConfigureServices(services =>
            {
                services.AddSingleton<ILogger>(logger);
                services.AddRockLibLoggerProvider();
            });

            return builder;
        }

        private static void AddLoggerFromLoggerFactory(this IServiceCollection services, string rockLibLoggerName)
        {
            services.AddSingleton<ILogger>(serviceProvider =>
            {
                if (!Config.IsLocked)
                {
                    var configuration = serviceProvider.GetService<IConfiguration>();
                    Config.SetRoot(configuration);
                }

                return LoggerFactory.GetInstance(rockLibLoggerName);
            });
        }

        private static void AddRockLibLoggerProvider(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerProvider>(serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger>();
                return new RockLibLoggerProvider(logger);
            });
        }
    }
}
