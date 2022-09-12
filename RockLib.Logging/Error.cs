using System;

namespace RockLib.Logging;

/// <summary>
/// Defines an error to be handled by the <see cref="IErrorHandler"/> interface.
/// </summary>
#pragma warning disable CA1716 // Identifiers should not match keywords
public sealed class Error
#pragma warning restore CA1716 // Identifiers should not match keywords
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    /// <param name="message">A message the describes the error.</param>
    /// <param name="exception">The exception responsible for the error, or null to indicate a timeout.</param>
    /// <param name="logProvider">
    /// The log provider that failed to write the log entry.
    /// </param>
    /// <param name="logEntry">The log entry that failed to write.</param>
    /// <param name="failureCount">
    /// The number of times the log provider has failed to write the log entry.
    /// </param>
    public Error(string message, Exception exception, ILogProvider logProvider, LogEntry logEntry, int failureCount)
    {
        if (failureCount < 0)
        {
            throw new ArgumentException("Cannot be less than zero.", nameof(failureCount));
        }

        Message = message ?? throw new ArgumentNullException(nameof(message));
        Exception = exception;
        LogProvider = logProvider ?? throw new ArgumentNullException(nameof(logProvider));
        LogEntry = logEntry ?? throw new ArgumentNullException(nameof(logEntry));
        FailureCount = failureCount;
    }

    /// <summary>
    /// Gets the message that describes the error.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the exception responsible for the error.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// Gets the log provider that failed to write the log entry.
    /// </summary>
    public ILogProvider LogProvider { get; }

    /// <summary>
    /// Gets the log entry that failed to write.
    /// </summary>
    public LogEntry LogEntry { get; }

    /// <summary>
    /// Gets the number of times the log provider has failed to write the log entry.
    /// </summary>
    public int FailureCount { get; }

    /// <summary>
    /// Gets a value indicating whether the error was a result of timing out.
    /// </summary>
    public bool IsTimeout => Exception is null;

    /// <summary>
    /// Gets the time that the error event occurred.
    /// </summary>
    public DateTime Timestamp { get; } = DateTime.Now;

    /// <summary>
    /// Gets or sets a value indicating whether the log provider should attempt to send
    /// the log entry again.
    /// </summary>
    public bool ShouldRetry { get; set; }
}