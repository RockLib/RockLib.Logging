using System;

namespace Rock.Logging
{
    public interface IThrottlingRuleConfiguration
    {
        string Name { get; }

        /// <summary>
        /// Gets the minimum time interval between logging the same <see cref="Rock.Logging.LogEntry"/>.
        /// </summary>
        /// <value>The min interval.</value>
        TimeSpan MinInterval { get; }

        /// <summary>
        /// Gets the min event threshold before firing an equivalent <see cref="Rock.Logging.LogEntry"/>. 
        /// For example if this value is set to 3 then every third event will be logged.
        /// If both MinInterval and MinEventThreshold are set then a OR logical operation is performed to see if a log entry will be logged.
        /// If the log entry passes at least one of the rule then it will be logged.
        /// Setting this value to 0 or 1 has no effect on throttling. If set to -1 and MinInterval is set to a non-zero value then
        /// the first message in a series will be skipped.
        /// </summary>
        /// <value>The min event threshold.</value>
        int MinEventThreshold { get; }
    }
}