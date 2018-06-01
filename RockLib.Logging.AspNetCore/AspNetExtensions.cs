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
        /// Use RockLib for dependency injected loggers. 
        /// </summary>
        /// <param name="builder">The IWebHostBuilder being extended.</param>
        /// <param name="rockLibLoggerName">The name of the RockLib logger.</param>
        /// <returns>IWebHostBuilder for chaining</returns>
        public static IWebHostBuilder UseRockLib(this IWebHostBuilder builder, string rockLibLoggerName = Logger.DefaultName)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.ConfigureServices(services =>
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

                services.AddSingleton<ILoggerProvider>(serviceProvider =>
                {
                    var logger = serviceProvider.GetRequiredService<ILogger>();
                    return new RockLibLoggerProvider(logger);
                });
            });

            return builder;
        }
    }
}
