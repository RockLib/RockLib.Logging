using System;

namespace RockLib.Logging.LogProcessing;

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
    /// <remarks>
    /// Implementations should not call the <see cref="ILogger.Log"/> method
    /// on the <paramref name="logger"/> parameter.
    /// </remarks>
    void ProcessLogEntry(ILogger logger, LogEntry logEntry);
}
