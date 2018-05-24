namespace Rock.Logging
{
    /// <summary>
    /// Evaluates wheter a log entry should be logged or not
    /// </summary>
    public interface IThrottlingRuleEvaluator
    {
        /// <summary>
        /// Determines whether the specified log entry should be logged.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <returns>
        /// <c>true</c> if the specified log entry should be logged; otherwise, <c>false</c>.
        /// </returns>
        bool ShouldLog(ILogEntry logEntry);
    }
}