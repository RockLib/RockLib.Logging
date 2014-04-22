using System;
using System.Collections.Generic;

namespace Rock.Logging
{
    public class LogEntry
    {
        private readonly IDictionary<string, string> _extendedProperties = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets an arbitrary "search key" for the log entry. The search key is meant to be
        /// used as an index in a database in order to search for this log entry.
        /// </summary>
        public string SearchKey { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public IDictionary<string, string> ExtendedProperties { get { return _extendedProperties; } }
    }
}