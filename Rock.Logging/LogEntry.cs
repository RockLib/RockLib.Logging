using System;
using System.Collections.Generic;

namespace Rock.Logging
{
    public class LogEntry
    {
        private readonly IDictionary<string, string> _extendedProperties = new Dictionary<string, string>();

        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public IDictionary<string, string> ExtendedProperties { get { return _extendedProperties; } }
    }
}