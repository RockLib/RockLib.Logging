using System;

namespace RockLib.Logging
{
    /// <summary>
    /// Provides data for the <see cref="ILogger.LogProviderError"/> event.
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorEventArgs"/> class.
        /// </summary>
        /// <param name="message">A message the describes the error.</param>
        /// <param name="exception">The exception responsible for the error.</param>
        /// <param name="logProvider">
        /// The log provider that failed to write the log entry.
        /// </param>
        /// <param name="logEntry">The log entry that failed to write.</param>
        /// <param name="failureCount">
        /// The number of times the log provider has failed to write the log entry.
        /// </param>
        public ErrorEventArgs(string message, Exception exception, ILogProvider logProvider, LogEntry logEntry, int failureCount)
        {
            Message = message;
            Exception = exception;
            LogProvider = logProvider;
            LogEntry = logEntry;
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
        public bool IsTimeout => Exception == null;

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
}