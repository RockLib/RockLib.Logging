namespace RockLib.Logging;

/// <summary>
/// Defines an object that adds custom context to <see cref="LogEntry"/> objects.
/// </summary>
public interface IContextProvider
{
    /// <summary>
    /// Add custom context to the <see cref="LogEntry"/> object.
    /// </summary>
    /// <param name="logEntry">The log entry to add custom context to.</param>
    void AddContext(LogEntry logEntry);
}
