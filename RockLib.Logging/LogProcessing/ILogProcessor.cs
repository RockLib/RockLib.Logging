using System;

namespace RockLib.Logging.LogProcessing
{
    /// <summary>
    /// Defines an object that processes log entries on behalf of a logger.
    /// </summary>
    public interface ILogProcessor : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the log processor has been disposed.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Processes the log entry on behalf of the logger.
        /// </summary>
        /// <param name="logger">
        /// The logger that the log entry is processed on behalf of. Its log
        /// providers and context providers define how the log entry is processed.
        /// </param>
        /// <param name="logEntry">The log entry to process.</param>
        /// <param name="errorHandler">
        /// An optional delegate to invoke if there is an error. If the
        /// <see cref="ErrorEventArgs.ShouldRetry"/> property of the delegate's
        /// <see cref="ErrorEventArgs"/> parameter is set to <see langword="true"/>,
        /// then the log entry will be retried.
        /// </param>
        /// <remarks>
        /// Implementations should not call the <see cref="ILogger.Log"/> method
        /// on the <paramref name="logger"/> parameter.
        /// </remarks>
        void ProcessLogEntry(ILogger logger, LogEntry logEntry, Action<ErrorEventArgs> errorHandler);
    }
}
