using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RockLib.Configuration;
using RockLib.Configuration.ObjectFactory;
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
        /// <param name="reloadOnConfigChange">
        /// Whether to create an instance of <see cref="ILogger"/> that automatically reloads itself when its
        /// configuration changes. Default is true.
        /// </param>
        /// <param name="setConfigRoot">
        /// Whether to call <see cref="Config.SetRoot(IConfiguration)"/> prior to calling
        /// <see cref="LoggerFactory.GetCached"/>. This value is true by default, because, by default,
        /// the <see cref="LoggerFactory"/> uses <see cref="Config.Root"/> as the backing data source.
        /// If <see cref="LoggerFactory.SetConfiguration"/> is called directly, this value can be false.
        /// </param>
        /// <param name="registerAspNetCoreLogger">
        /// Whether to bypass registering an <see cref="ILoggerProvider"/> with the DI system.
        /// </param>
        /// <returns>IWebHostBuilder for chaining</returns>
        /// <remarks>
        /// This method has a side-effect of calling the <see cref="Config.SetRoot(IConfiguration)"/>
        /// method, passing it the instance of <see cref="IConfiguration"/> obtained from the local
        /// <see cref="IServiceProvider"/>.
        /// </remarks>
        public static IWebHostBuilder UseRockLibLogging(this IWebHostBuilder builder, string rockLibLoggerName = Logger.DefaultName,
            DefaultTypes defaultTypes = null, ValueConverters valueConverters = null, bool reloadOnConfigChange = true,
            bool setConfigRoot = true, bool registerAspNetCoreLogger = false)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.ConfigureServices(services =>
            {
                services.AddLoggerFromLoggerFactory(rockLibLoggerName, defaultTypes, valueConverters, reloadOnConfigChange, setConfigRoot);
                if (registerAspNetCoreLogger)
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
        /// <param name="registerAspNetCoreLogger">
        /// Whether to bypass registering an <see cref="ILoggerProvider"/> with the DI system.
        /// </param>
        /// <returns>IWebHostBuilder for chaining</returns>
        /// <remarks>
        /// This extension method, unlike the <see cref="UseRockLibLogging(IWebHostBuilder, string, DefaultTypes, ValueConverters, bool, bool, bool)"/> overload, does not
        /// have any side-effects. As such, applications using this extension method many need to call the
        /// <see cref="Config.SetRoot(IConfiguration)"/> method in the constructor of their <code>Startup</code> class.
        /// </remarks>
        public static IWebHostBuilder UseRockLibLogging(this IWebHostBuilder builder, ILogger logger, bool registerAspNetCoreLogger = false)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            builder.ConfigureServices(services =>
            {
                services.AddSingleton(logger);
                if (registerAspNetCoreLogger)
                    services.AddRockLibLoggerProvider();
            });

            return builder;
        }

        private static void AddLoggerFromLoggerFactory(this IServiceCollection services, string rockLibLoggerName, 
            DefaultTypes defaultTypes, ValueConverters valueConverters, bool reloadOnConfigChange, bool setConfigRoot)
        {
            services.AddSingleton<ILogProcessor, BackgroundLogProcessor>();

            services.AddTransient(serviceProvider =>
            {
                if (setConfigRoot && !Config.IsLocked && Config.IsDefault)
                {
                    var configuration = serviceProvider.GetService<IConfiguration>();
                    Config.SetRoot(configuration);
                }

                var resolver = new Resolver(t => serviceProvider.GetService(t));

                return LoggerFactory.Create(rockLibLoggerName, defaultTypes, valueConverters, resolver, reloadOnConfigChange);
            });
        }

        private static void AddRockLibLoggerProvider(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerProvider>(serviceProvider =>
            {
                return new RockLibLoggerProvider(() => serviceProvider.GetRequiredService<ILogger>());
            });
        }
    }
}
