using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RockLib.Logging
{
    public sealed class LogEntry
    {
        private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;

        private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<Action<LogEntry, object>>> _setExtendedPropertyActionsCache = new ConcurrentDictionary<Type, IReadOnlyCollection<Action<LogEntry, object>>>();

        public LogEntry(LogLevel level, string message, object extendedProperties = null)
            : this(level, message, null, extendedProperties)
        {
        }

        public LogEntry(LogLevel level, string message, Exception exception, object extendedProperties = null)
        {
            Level = level;
            Message = message;
            Exception = exception;
            SetExtendedProperties(extendedProperties);
        }

        public string UniqueId { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public Exception Exception { get; set; }
        public Dictionary<string, object> ExtendedProperties { get; } = new Dictionary<string, object>();
        public LogLevel Level { get; set; }
        public string MachineIpAddress { get; set; } = Cached.IpAddress;
        public string MachineName { get; set; } = Environment.MachineName;
        public string Message { get; set; }
        public string UserName { get; set; } = Environment.UserName;

        public string GetExceptionData() => Exception?.FormatToString();

        public void SetExtendedProperties(object extendedProperties)
        {
            if (extendedProperties != null)
                foreach (var setExtendedProperty in GetSetExtendedPropertyActions(extendedProperties.GetType()))
                    setExtendedProperty(this, extendedProperties);
        }

        private static IReadOnlyCollection<Action<LogEntry, object>> GetSetExtendedPropertyActions(Type type) =>
            _setExtendedPropertyActionsCache.GetOrAdd(type, t =>
                t.GetProperties(PublicInstance).Select(GetSetExtendedPropertyAction).ToArray());

        private static Action<LogEntry, object> GetSetExtendedPropertyAction(PropertyInfo property)
        {
            var getPropertyValue = property.GetGetFunc();
            return (logEntry, extendedPropertiesObject) =>
                logEntry.ExtendedProperties[property.Name] = getPropertyValue(extendedPropertiesObject);
        }
    }
}