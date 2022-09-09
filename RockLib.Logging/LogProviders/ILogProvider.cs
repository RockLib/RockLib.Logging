using System;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.Logging;

/// <summary>
/// Defines a object that writes log entries.
/// </summary>
/// <remarks>
/// Implementations of this interface *must* ensure that their <see cref="WriteAsync"/> method
/// is thread-safe. Multiple threads can invoke the method simultaneously.
/// </remarks>
public interface ILogProvider
{
    /// <summary>
    /// Gets the timeout of the <see cref="ILogProvider"/>.
    /// </summary>
    /// <remarks>
    /// If a task returned by the <see cref="WriteAsync"/> method does not complete
    /// by the value of the <see cref="Timeout"/> property, the task will be cancelled.
    /// </remarks>
    TimeSpan Timeout { get; }

    /// <summary>
    /// Gets the level of the <see cref="ILogProvider"/>.
    /// </summary>
    /// <remarks>
    /// This value is used by the <see cref="Logger"/> class to determine if it should
    /// call this instance's <see cref="WriteAsync"/> method for a given log entry. If
    /// the value of this property is higher than a log entry's level, then this log
    /// provider is skipped.
    /// </remarks>
    LogLevel Level { get; }

    /// <summary>
    /// Writes the specified log entry.
    /// </summary>
    /// <param name="logEntry">The log entry to write.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <returns>A task that completes when the log entry has been written.</returns>
    Task WriteAsync(LogEntry logEntry, CancellationToken cancellationToken);
}