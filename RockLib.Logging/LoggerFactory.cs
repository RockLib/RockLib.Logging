using RockLib.Configuration;
using RockLib.Configuration.ObjectFactory;
using RockLib.Immutable;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RockLib.Logging
{
    /// <summary>
    /// A static class that retrieves instances of the <see cref="Logger"/> class by name. The available
    /// logger instances are set from configuration by default, but can be set programmatically if needed.
    /// </summary>
    /// <remarks>
    /// The following code snippet defines how the default loggers are initialized:
    /// <code>
    /// using RockLib.Configuration;
    /// using RockLib.Configuration.ObjectFactory;
    /// 
    /// Config.Root.GetSection("rocklib.logging").Create&lt;IReadOnlyCollection&lt;Logger&gt;&gt;();
    /// </code>
    /// </remarks>
    public static class LoggerFactory
    {
        private static readonly ConcurrentDictionary<string, Logger> _lookup = new ConcurrentDictionary<string, Logger>(StringComparer.InvariantCultureIgnoreCase);

        private static readonly Semimutable<IReadOnlyCollection<Logger>> _loggers = new Semimutable<IReadOnlyCollection<Logger>>(CreateDefaultLoggers);

        /// <summary>
        /// Gets the loggers that are avilable to the <see cref="GetInstance"/> method to select from.
        /// </summary>
        public static IReadOnlyCollection<Logger> Loggers => _loggers.Value;

        /// <summary>
        /// Sets the loggers that will be avilable to the <see cref="GetInstance"/> method to select from.
        /// </summary>
        /// <param name="loggers">
        /// The loggers that will be avilable to the <see cref="GetInstance"/> method to select from.
        /// </param>
        public static void SetLoggers(IReadOnlyCollection<Logger> loggers) =>
            _loggers.Value = loggers ?? throw new ArgumentNullException(nameof(loggers));

        /// <summary>
        /// Returns the first <see cref="Logger"/> from the <see cref="Loggers"/> property that has a
        /// <see cref="Logger.Name"/> that matches (case-insensitive) the <paramref name="name"/>
        /// parameter.
        /// </summary>
        /// <param name="name">The name of the logger to retrieve.</param>
        /// <returns>A logger with a matching name.</returns>
        /// <exception cref="KeyNotFoundException">
        /// If the <see cref="Loggers"/> property does not contain a logger with the specified name.
        /// </exception>
        public static Logger GetInstance(string name = Logger.DefaultName) => _lookup.GetOrAdd(name ?? Logger.DefaultName, FindLogger);

        /// <summary>
        /// Calls the <see cref="Logger.Dispose"/> method on each logger in the <see cref="Loggers"/> property,
        /// blocking until any of their pending logging operations have completed.
        /// </summary>
        public static void ShutDown()
        {
            foreach (var logger in Loggers)
                logger.Dispose();
        }

        private static Logger FindLogger(string name) =>
            Loggers.FirstOrDefault(logger => logger.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            ?? throw new KeyNotFoundException($"LoggerFactory.Loggers does not contain a Logger with the name '{name}'.");

        /// <summary>
        /// Defines the method used to create the default value of the <see cref="Loggers"/> property.
        /// </summary>
        /// <returns>A collection of new <see cref="Logger"/> objects.</returns>
        public static IReadOnlyCollection<Logger> CreateDefaultLoggers() =>
             Config.Root.GetSection("rocklib.logging").Create<IReadOnlyCollection<Logger>>();
    }
}
