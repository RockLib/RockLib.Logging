namespace RockLib.Logging;

/// <summary>
/// Defines an object that can handle errors that occur when processing log entries.
/// </summary>
public interface IErrorHandler
{
    /// <summary>
    /// Handle the specified error. If an implementation sets the
    /// <see cref="Error.ShouldRetry"/> property to true, then the log provider should
    /// attempt to send the log entry again.
    /// </summary>
    /// <param name="error">An error that has occurred.</param>
#pragma warning disable CA1716 // Identifiers should not match keywords
    void HandleError(Error error);
#pragma warning restore CA1716 // Identifiers should not match keywords
}