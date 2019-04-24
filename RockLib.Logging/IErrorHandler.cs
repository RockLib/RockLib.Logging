namespace RockLib.Logging
{
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
        void HandleError(Error error);
    }
}