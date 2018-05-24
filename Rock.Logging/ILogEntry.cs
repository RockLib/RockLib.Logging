using System;

namespace Rock.Logging
{
    /// <summary>
    /// Defines the interface for a single logging data point.
    /// </summary>
    public interface ILogEntry
    {
        /// <summary>
        /// Gets or sets the message that needs to be logged.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        string ApplicationId { get; set; }

        /// <summary>
        /// The ID of the account that is running the application. By default, this is set to <see cref="System.Environment.UserName"/>.
        /// </summary>
        string ApplicationUserId { get; set; }

        /// <summary>
        /// Gets or sets the time when the entry was created.
        /// </summary>
        DateTime CreateTime { get; set; }

        /// <summary>
        /// Gets or sets the environment (e.g. Test or Prod) in which the log entry was created.
        /// </summary>
        string Environment { get; set; }

        /// <summary>
        /// Gets or sets the name of the machine name the log entry was created on.
        /// </summary>
        string MachineName { get; set; }

        /// <summary>
        /// Gets or sets the log level of the log entry (e.g. Debug or Error).
        /// </summary>
        LogLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the details of an exception.
        /// </summary>
        string ExceptionDetails { get; set; }

        /// <summary>
        /// Gets or sets arbitrary contextual information related to a thrown exception. This value
        /// should give a developer additional information to help debug or fix the issue.
        /// </summary>
        string ExceptionContext { get; set; }

        /// <summary>
        /// Gets or sets the type of the exception.
        /// </summary>
        string ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets the extended properties. This property is used to add any additional 
        /// information into the log entry.
        /// </summary>
        LogEntryExtendedProperties ExtendedProperties { get; set; }

        /// <summary>
        /// Gets or sets an arbitrary unique identifier for the log entry. This value allows a log
        /// entry to be identified on the client-side. For example, a link to this log entry can be
        /// generated, client-side, before the log entry is added to a database. It is assumed that
        /// a database will index this value.
        /// </summary>
        string UniqueId { get; set; }

        /// <summary>
        /// Gets a hash of this instance of <see cref="ILogEntry"/> for the purpose of
        /// throttling log entries. If the value returned from this log entry is equal
        /// to the value from another log entry, then they are considered duplicates.
        /// Depending on the throttling configuration of a logger, duplicate log etries
        /// may or may not be sent its log providers.
        /// </summary>
        /// <returns>A hash code to be used for throttling purposes.</returns>
        int GetThrottlingHashCode();
    }
}