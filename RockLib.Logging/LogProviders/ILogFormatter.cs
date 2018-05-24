namespace RockLib.Logging
{
    /// <summary>
    /// Defines an object that formats <see cref="LogEntry"/> objects to string
    /// representations.
    /// </summary>
    public interface ILogFormatter
    {
        /// <summary>
        /// Formats the specified log entry as a string.
        /// </summary>
        /// <param name="logEntry">The log entry to format.</param>
        /// <returns>The formatted log entry.</returns>
        string Format(LogEntry logEntry);
    }
}
