using Microsoft.Extensions.Configuration;
using RockLib.Configuration;
using RockLib.Configuration.ObjectFactory;
using RockLib.Immutable;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Resolver = RockLib.Configuration.ObjectFactory.Resolver;

namespace RockLib.Logging
{
    /// <summary>
    /// A static class that creates and retrieves instances of the <see cref="ILogger"/> interface by name.
    /// Loggers returned by this factory are defined by instances of the <see cref="IConfiguration"/> interface.
    /// </summary>
    public static class LoggerFactory
    {
        /// <summary>
        /// The section name, relative to <see cref="Config.Root"/>, from which to retrieve logging settings.
        /// </summary>
        public const string SectionName = "RockLib.Logging";

        /// <summary>
        /// The alternate section name, relative to <see cref="Config.Root"/>, from which to retrieve logging settings.
        /// </summary>
        public const string AlternateSectionName = "RockLib_Logging";

        private static readonly ConditionalWeakTable<IConfiguration, ConcurrentDictionary<string, ILogger>> _cache = new ConditionalWeakTable<IConfiguration, ConcurrentDictionary<string, ILogger>>();

        private static readonly Semimutable<IConfiguration> _configuration =
            new Semimutable<IConfiguration>(() => Config.Root.GetCompositeSection(AlternateSectionName, SectionName));

        /// <summary>
        /// Sets the instance of <see cref="IConfiguration"/> that defines the loggers that can be created
        /// or retrieved. Note that once the <see cref="Configuration"/> property has been read from, it
        /// cannot be changed.
        /// </summary>
        /// <param name="configuration">
        /// An instance of <see cref="IConfiguration"/> that defines the loggers that can be retrieved. The
        /// configuration can define a single logger object or a list of logger objects.
        /// </param>
        public static void SetConfiguration(IConfiguration configuration) => _configuration.Value = configuration ?? throw new ArgumentNullException(nameof(configuration));

        /// <summary>
        /// Gets the instance of <see cref="IConfiguration"/> that defines the loggers that can be created
        /// or retrieved.
        /// </summary>
        /// <remarks>
        /// Only the extension methods in this class that do not have an <see cref="IConfiguration"/> parameter
        /// use this property.
        /// </remarks>
        public static IConfiguration Configuration => _configuration.Value;

        /// <summary>
        /// Gets a cached instance of <see cref="ILogger"/> with a name matching the <paramref name="name"/>
        /// parameter that is backed by the value of the <see cref="Configuration"/> property.
        /// </summary>
        /// <param name="name">The name of the logger to retrieve.</param>
        /// <param name="defaultTypes">
        /// An object that defines the default types to be used when a type is not explicitly specified by a
        /// configuration section.
        /// </param>
        /// <param name="valueConverters">
        /// An object that defines custom converter functions that are used to convert string configuration
        /// values to a target type.
        /// </param>
        /// <param name="resolver">
        /// An object that can retrieve constructor parameter values that are not found in configuration. This
        /// object is an adapter for dependency injection containers, such as Ninject, Unity, Autofac, or
        /// StructureMap. Consider using the <see cref="Resolver"/> class for this parameter, as it supports
        /// most depenedency injection containers.
        /// </param>
        /// <param name="reloadOnConfigChange">
        /// Whether to create an instance of <see cref="ILogger"/> that automatically reloads itself when its
        /// configuration changes. Default is true.
        /// </param>
        /// <returns>A logger with a matching name.</returns>
        /// <exception cref="KeyNotFoundException">
        /// If a logger with a name specified by the <paramref name="name"/> parameter is not defined in
        /// the value of the <see cref="Configuration"/> property.
        /// </exception>
        public static ILogger GetCached(string name = Logger.DefaultName,
            DefaultTypes defaultTypes = null, ValueConverters valueConverters = null,
            IResolver resolver = null, bool reloadOnConfigChange = true) => 
            Configuration.GetCachedLogger(name, defaultTypes, valueConverters, resolver, reloadOnConfigChange);

        /// <summary>
        /// Gets a cached instance of <see cref="ILogger"/> with a name matching the <paramref name="name"/>
        /// parameter that is backed by the value of the <paramref name="configuration"/> parameter.
        /// </summary>
        /// <param name="configuration">
        /// An instance of <see cref="IConfiguration"/> that defines the loggers that can be retrieved. The
        /// configuration can define a single logger object or a list of logger objects.
        /// </param>
        /// <param name="name">The name of the logger to retrieve.</param>
        /// <param name="defaultTypes">
        /// An object that defines the default types to be used when a type is not explicitly specified by a
        /// configuration section.
        /// </param>
        /// <param name="valueConverters">
        /// An object that defines custom converter functions that are used to convert string configuration
        /// values to a target type.
        /// </param>
        /// <param name="resolver">
        /// An object that can retrieve constructor parameter values that are not found in configuration. This
        /// object is an adapter for dependency injection containers, such as Ninject, Unity, Autofac, or
        /// StructureMap. Consider using the <see cref="Resolver"/> class for this parameter, as it supports
        /// most depenedency injection containers.
        /// </param>
        /// <param name="reloadOnConfigChange">
        /// Whether to create an instance of <see cref="ILogger"/> that automatically reloads itself when its
        /// configuration changes. Default is true.
        /// </param>
        /// <returns>A logger with a matching name.</returns>
        /// <exception cref="KeyNotFoundException">
        /// If a logger with a name specified by the <paramref name="name"/> parameter is not defined in
        /// the value of the <paramref name="configuration"/> parameter.
        /// </exception>
        public static ILogger GetCachedLogger(this IConfiguration configuration, string name = Logger.DefaultName,
            DefaultTypes defaultTypes = null, ValueConverters valueConverters = null,
            IResolver resolver = null, bool reloadOnConfigChange = true)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (name == null) throw new ArgumentNullException(nameof(name));

            var configCache = _cache.GetValue(configuration, c => new ConcurrentDictionary<string, ILogger>());
            return configCache.GetOrAdd(name, n => configuration.CreateLogger(n, defaultTypes, valueConverters, resolver, reloadOnConfigChange));
        }

        /// <summary>
        /// Creates a new instance of <see cref="ILogger"/> with a name matching the <paramref name="name"/>
        /// parameter that is backed by the value of the <see cref="Configuration"/> property.
        /// </summary>
        /// <param name="name">The name of the logger to create.</param>
        /// <param name="defaultTypes">
        /// An object that defines the default types to be used when a type is not explicitly specified by a
        /// configuration section.
        /// </param>
        /// <param name="valueConverters">
        /// An object that defines custom converter functions that are used to convert string configuration
        /// values to a target type.
        /// </param>
        /// <param name="resolver">
        /// An object that can retrieve constructor parameter values that are not found in configuration. This
        /// object is an adapter for dependency injection containers, such as Ninject, Unity, Autofac, or
        /// StructureMap. Consider using the <see cref="Resolver"/> class for this parameter, as it supports
        /// most depenedency injection containers.
        /// </param>
        /// <param name="reloadOnConfigChange">
        /// Whether to create an instance of <see cref="ILogger"/> that automatically reloads itself when its
        /// configuration changes. Default is true.
        /// </param>
        /// <returns>A new logger with a matching name.</returns>
        /// <exception cref="KeyNotFoundException">
        /// If a logger with a name specified by the <paramref name="name"/> parameter is not defined in
        /// the value of the <see cref="Configuration"/> property.
        /// </exception>
        public static ILogger Create(string name = Logger.DefaultName,
            DefaultTypes defaultTypes = null, ValueConverters valueConverters = null,
            IResolver resolver = null, bool reloadOnConfigChange = true) =>
            Configuration.CreateLogger(name, defaultTypes, valueConverters, resolver, reloadOnConfigChange);

        /// <summary>
        /// Creates a new instance of <see cref="ILogger"/> with a name matching the <paramref name="name"/>
        /// parameter that is backed by the value of the <paramref name="configuration"/> parameter.
        /// </summary>
        /// <param name="configuration">
        /// An instance of <see cref="IConfiguration"/> that defines the loggers that can be retrieved. The
        /// configuration can define a single logger object or a list of logger objects.
        /// </param>
        /// <param name="name">The name of the logger to create.</param>
        /// <param name="defaultTypes">
        /// An object that defines the default types to be used when a type is not explicitly specified by a
        /// configuration section.
        /// </param>
        /// <param name="valueConverters">
        /// An object that defines custom converter functions that are used to convert string configuration
        /// values to a target type.
        /// </param>
        /// <param name="resolver">
        /// An object that can retrieve constructor parameter values that are not found in configuration. This
        /// object is an adapter for dependency injection containers, such as Ninject, Unity, Autofac, or
        /// StructureMap. Consider using the <see cref="Resolver"/> class for this parameter, as it supports
        /// most depenedency injection containers.
        /// </param>
        /// <param name="reloadOnConfigChange">
        /// Whether to create an instance of <see cref="ILogger"/> that automatically reloads itself when its
        /// configuration changes. Default is true.
        /// </param>
        /// <returns>A new logger with a matching name.</returns>
        /// <exception cref="KeyNotFoundException">
        /// If a logger with a name specified by the <paramref name="name"/> parameter is not defined in
        /// the value of the <paramref name="configuration"/> parameter.
        /// </exception>
        public static ILogger CreateLogger(this IConfiguration configuration, string name = Logger.DefaultName,
            DefaultTypes defaultTypes = null, ValueConverters valueConverters = null,
            IResolver resolver = null, bool reloadOnConfigChange = true)
        {
            if (defaultTypes == null)
                defaultTypes = new DefaultTypes();
            if (!defaultTypes.TryGet(typeof(ILogger), out var dummy))
                defaultTypes.Add(typeof(ILogger), typeof(Logger));

            if (configuration.IsList())
            {
                foreach (var child in configuration.GetChildren())
                    if (name.Equals(child.GetSectionName(), StringComparison.OrdinalIgnoreCase))
                        return reloadOnConfigChange
                            ? child.CreateReloadingProxy<ILogger>(defaultTypes, valueConverters, resolver)
                            : child.Create<ILogger>(defaultTypes, valueConverters, resolver);
            }
            else if (name.Equals(configuration.GetSectionName(), StringComparison.OrdinalIgnoreCase))
                return reloadOnConfigChange
                    ? configuration.CreateReloadingProxy<ILogger>(defaultTypes, valueConverters, resolver)
                    : configuration.Create<ILogger>(defaultTypes, valueConverters, resolver);

            throw new KeyNotFoundException($"No loggers were found matching the name '{name}'.");
        }

        private static bool IsEmpty(this IConfiguration configuration)
        {
            switch (configuration)
            {
                case IConfigurationSection section:
                    return section.Value == null && !section.GetChildren().Any();
                default:
                    return !configuration.GetChildren().Any();
            }
        }

        private static bool IsList(this IConfiguration configuration)
        {
            if (configuration is IConfigurationSection section && section.Value != null)
                return false;
            int i = 0;
            foreach (var child in configuration.GetChildren())
                if (child.Key != i++.ToString())
                    return false;
            return i > 0;
        }

        private static string GetSectionName(this IConfiguration configuration)
        {
            var section = configuration;

            if (configuration["type"] != null && !configuration.GetSection("value").IsEmpty())
                section = configuration.GetSection("value");

            return section["name"] ?? Logger.DefaultName;
        }
    }
}
