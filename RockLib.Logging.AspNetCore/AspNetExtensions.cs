using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RockLib.Configuration;
using RockLib.Configuration.AspNetCore;
using RockLib.Configuration.ObjectFactory;
using RockLib.Logging.DependencyInjection;
using RockLib.Logging.LogProcessing;

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
        /// /// <param name="defaultTypes">
        /// An object that defines the default types to be used when a type is not explicitly specified by a
        /// configuration section.
        /// </param>
        /// <param name="valueConverters">
        /// An object that defines custom converter functions that are used to convert string configuration
        /// values to a target type.
        /// </param>
        /// <param name="setConfigRoot">
        /// Whether to set the value of the <see cref="Config.Root"/> property to the <see cref="IConfiguration"/>
        /// containing the merged configuration of the application and the <see cref="IWebHost"/>.
        /// </param>
        /// <param name="registerAspNetCoreLogger">
        /// Whether to register a RockLib <see cref="ILoggerProvider"/> with the DI system.
        /// </param>
        /// <returns>IWebHostBuilder for chaining</returns>
        /// <remarks>
        /// This method has a side-effect of calling the <see cref="Config.SetRoot(IConfiguration)"/>
        /// method, passing it the instance of <see cref="IConfiguration"/> obtained from the local
        /// <see cref="IServiceProvider"/>.
        /// </remarks>
        public static IWebHostBuilder UseRockLibLogging(this IWebHostBuilder builder, string rockLibLoggerName = Logger.DefaultName,
            DefaultTypes defaultTypes = null, ValueConverters valueConverters = null,
            bool setConfigRoot = true, bool registerAspNetCoreLogger = false)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (setConfigRoot)
                builder.SetConfigRoot();

            builder.ConfigureServices(services =>
                services.AddRockLibLoggerTransient(rockLibLoggerName, defaultTypes, valueConverters, registerAspNetCoreLogger));

            return builder;
        }

        /// <summary>
        /// Adds the specified instance of <see cref="ILogger"/> and an instance of <see cref="ILoggerProvider"/> that uses
        /// that logger to the specified <see cref="IServiceCollection"/> as singletons.
        /// </summary>
        /// <param name="builder">The IWebHostBuilder being extended.</param>
        /// <param name="logger">The RockLib logger used for logging.</param>
        /// <param name="registerAspNetCoreLogger">
        /// Whether to register a RockLib <see cref="ILoggerProvider"/> with the DI system.
        /// </param>
        /// <returns>IWebHostBuilder for chaining</returns>
        /// <remarks>
        /// This extension method, unlike the <see cref="UseRockLibLogging(IWebHostBuilder, string, DefaultTypes, ValueConverters, bool, bool)"/> overload, does not
        /// have any side-effects. As such, applications using this extension method many need to call the
        /// <see cref="Config.SetRoot(IConfiguration)"/> method in the constructor of their <code>Startup</code> class.
        /// </remarks>
        public static IWebHostBuilder UseRockLibLogging(this IWebHostBuilder builder, ILogger logger, bool registerAspNetCoreLogger = false)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            builder.ConfigureServices(services => services.AddRockLibLoggerSingleton(logger, registerAspNetCoreLogger));

            return builder;
        }
    }
}
