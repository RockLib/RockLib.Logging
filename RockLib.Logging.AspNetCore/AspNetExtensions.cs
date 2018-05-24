using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
        public static IWebHostBuilder UseRockLib(this IWebHostBuilder builder, string rockLibLoggerName = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.ConfigureServices(services =>
            {
                services.AddSingleton<ILoggerProvider>(serviceProvider => new RockLibLoggerProvider(rockLibLoggerName));
            });

            return builder;
        }
    }
}
