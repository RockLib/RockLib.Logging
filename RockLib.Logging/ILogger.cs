using System;
using System.Collections.Generic;

namespace RockLib.Logging
{
    /// <summary>
    /// Defines an object used for logging.
    /// </summary>
    public interface ILogger : IDisposable
    {
        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the logger is disabled.
        /// </summary>
        bool IsDisabled { get; }

        /// <summary>
        /// Gets the logging level of the logger.
        /// </summary>
        /// <remarks>
        /// Log entries with a level lower than the value of this property should
        /// not be logged by this logger.
        /// </remarks>
        LogLevel Level { get; }

        /// <summary>
        /// Gets the collection of <see cref="ILogProvider"/> objects used by this logger.
        /// </summary>
        IReadOnlyCollection<ILogProvider> LogProviders { get; }

        /// <summary>
        /// Gets the collection of <see cref="IContextProvider"/> objects used by this logger.
        /// </summary>
        IReadOnlyCollection<IContextProvider> ContextProviders { get; }

        /// <summary>
        /// Gets or sets the object that handles errors that occur during log processing.
        /// </summary>
        IErrorHandler ErrorHandler { get; set; }

        /// <summary>
        /// Logs the specified log entry.
        /// </summary>
        /// <param name="logEntry">The log entry to log.</param>
        void Log(LogEntry logEntry);
    }
}