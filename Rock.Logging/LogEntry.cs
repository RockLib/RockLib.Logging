using System;
using System.Collections.Generic;

namespace Rock.Logging
{
    public class LogEntry
    {
        private readonly IDictionary<string, string> _extendedProperties = new Dictionary<string, string>();

        public string ApplicationId { get; set; }
        public DateTime CreateTime { get; set; }
        public string Environment { get; set; }
        public Exception Exception { get; set; }
        public LogLevel Level { get; set; }
        public string MachineIPAddress { get; set; }
        public string MachineName { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }

        public IDictionary<string, string> ExtendedProperties { get { return _extendedProperties; } }

        /// <summary>
        /// Gets or sets an arbitrary "search key" for the log entry. The search key is meant to be
        /// used as an index in a database in order to search for this log entry.
        /// </summary>
        public string SearchKey { get; set; }
    }
}