using Microsoft.Extensions.Configuration;
using RockLib.Configuration;
using RockLib.Configuration.ObjectFactory;
using System;
using System.Collections.Generic;
using Resolver = RockLib.Configuration.ObjectFactory.Resolver;

namespace RockLib.Logging;

/// <summary>
/// A static class that creates and retrieves instances of the <see cref="ILogger"/> interface by name.
/// Loggers returned by this factory are defined by the <see cref="Configuration"/> property.
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

    // We no longer have Semimutable...but we don't need it either,
    // this should be sufficient.
    private static bool _configurationSet;
    private static IConfiguration? _configuration;

    /// <summary>
    /// Sets the instance of <see cref="IConfiguration"/> that defines the loggers that can be created
    /// or retrieved. Note that once the <see cref="Configuration"/> property has been read from, it
    /// cannot be changed.
    /// </summary>
    /// <param name="configuration">
    /// An instance of <see cref="IConfiguration"/> that defines the loggers that can be retrieved. The
    /// configuration can define a single logger object or a list of logger objects.
    /// </param>
    public static void SetConfiguration(IConfiguration configuration)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(configuration);
#else
        if (configuration is null) { throw new ArgumentNullException(nameof(configuration)); }
#endif
        Configuration = configuration;
    }

    /// <summary>
    /// Gets the instance of <see cref="IConfiguration"/> that defines the loggers that can be created
    /// or retrieved.
    /// </summary>
    /// <remarks>
    /// Only the extension methods in this class that do not have an <see cref="IConfiguration"/> parameter
    /// use this property.
    /// </remarks>
    public static IConfiguration Configuration
    {
        get
        {
            if (!_configurationSet)
            {
                _configuration = Config.Root!.GetCompositeSection(AlternateSectionName, SectionName);
                _configurationSet = true;
            }

            return _configuration!;
        }
        private set
        {
            if (_configurationSet)
            {
                throw new InvalidOperationException("Configuration has already been set.");
            }

            _configuration = value;
            _configurationSet = true;
        }
    }

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
        DefaultTypes? defaultTypes = null, ValueConverters? valueConverters = null,
        IResolver? resolver = null, bool reloadOnConfigChange = true) =>
        Configuration.GetCachedLogger(name, defaultTypes, valueConverters, resolver, reloadOnConfigChange);

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
        DefaultTypes? defaultTypes = null, ValueConverters? valueConverters = null,
        IResolver? resolver = null, bool reloadOnConfigChange = true) =>
        Configuration.CreateLogger(name, defaultTypes, valueConverters, resolver, reloadOnConfigChange);
}
