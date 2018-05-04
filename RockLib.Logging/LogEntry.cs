using System;
using System.Collections.Generic;

namespace RockLib.Logging
{
    public sealed class LogEntry
    {
        public string UniqueId { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public Exception Exception { get; set; }
        public Dictionary<string, string> ExtendedProperties { get; } = new Dictionary<string, string>();
        public LogLevel Level { get; set; }
        public string MachineIPAddress { get; set; } = Cached.IpAddress;
        public string MachineName { get; set; } = Environment.MachineName;
        public string Message { get; set; }
        public string UserName { get; set; } = Environment.UserName;

        public string GetExceptionData() => Exception?.FormatToString();
    }
}