using RockLib.Logging.SafeLogging;
using RockLib.Reflection.Optimized;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RockLib.Logging
{
    /// <summary>
    /// Defines various properties used for logging operations.
    /// </summary>
    public sealed class LogEntry
    {
        /// <summary>The default <see cref="LogLevel"/> of the <see cref="Level"/> property.</summary>
        public const LogLevel DefaultLevel = LogLevel.Error;

        private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;

        private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<Action<LogEntry, object>>> _setExtendedPropertyActionsCache = new ConcurrentDictionary<Type, IReadOnlyCollection<Action<LogEntry, object>>>();
        private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<Action<LogEntry, object>>> _setSafeExtendedPropertyActionsCache = new ConcurrentDictionary<Type, IReadOnlyCollection<Action<LogEntry, object>>>();

        private static readonly ConcurrentDictionary<Type, (Func<object, string> getKey, Func<object, object> getValue)> _stringDictionaryItemAccessors = new ConcurrentDictionary<Type, (Func<object, string>, Func<object, object>)>();

        private LogLevel _level;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class with its <see cref="Level"/> set
        /// to <see cref="DefaultLevel"/>.
        /// </summary>
        public LogEntry()
        {
            _level = DefaultLevel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="level">The logging level of the current logging operation.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to the <see cref="ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to <see cref="ExtendedProperties"/>.
        /// </param>
        public LogEntry(string message, LogLevel level = DefaultLevel, object extendedProperties = null)
            : this(message, null, level, extendedProperties)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the current logging operation.</param>
        /// <param name="level">The logging level of the current logging operation.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to the <see cref="ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to <see cref="ExtendedProperties"/>.
        /// </param>
        public LogEntry(string message, Exception exception, LogLevel level = DefaultLevel, object extendedProperties = null)
        {
            Level = level;
            Message = message;
            Exception = exception;
            SetExtendedProperties(extendedProperties);
        }

        /// <summary>
        /// Gets or sets the unique ID for this log entry. This is initialized to a new guid by default.
        /// </summary>
        public string UniqueId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the time this log entry was created. This is initialized to
        /// <see cref="DateTime.UtcNow"/> by default.
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the <see cref="System.Exception"/> associated with this log entry.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets the extended properties for this log entry. This property enables any kind of data to
        /// be attached to a logging operation by name.
        /// </summary>
        public Dictionary<string, object> ExtendedProperties { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the logging level of the current logging operation.
        /// </summary>
        public LogLevel Level
        {
            get => _level;
            set
            {
                if (!Enum.IsDefined(typeof(LogLevel), value))
                    throw new ArgumentException($"Log level is not defined: {value}.", nameof(value));
                if (value == LogLevel.NotSet)
                    throw new ArgumentException($"Cannot set the level of a log entry to {LogLevel.NotSet}.", nameof(value));

                _level = value;
            }
        }

        /// <summary>
        /// Gets or sets the ID used to corralate a transaction across many service calls for this log entry.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the business process ID.
        /// </summary>
        public string BusinessProcessId { get; set; }

        /// <summary>
        /// Gets or sets the business activity ID.
        /// </summary>
        public string BusinessActivityId { get; set; }

        /// <summary>
        /// Gets or sets the IP addess of the machine where the current logging operation is taking place.
        /// This is set to a default value, detected at runtime.
        /// </summary>
        public string MachineIpAddress { get; set; } = Cached.IpAddress;

        /// <summary>
        /// Gets or sets the machine name where the current logging operation is taking place. This is
        /// set to <see cref="Environment.MachineName"/> by default.
        /// </summary>
        public string MachineName { get; set; } = Cached.MachineName;

        /// <summary>
        /// Gets or sets the message of the log entry.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the caller information of the log entry.
        /// </summary>
        public string CallerInfo { get; set; }

        /// <summary>
        /// Gets or sets the username of the user that is performing the current logging operation.
        /// This is set to <see cref="Environment.UserName"/> by default.
        /// </summary>
        public string UserName { get; set; } = Cached.UserName;

        /// <summary>
        /// Gets a string representation of the <see cref="Exception"/> property, or null
        /// if <see cref="Exception"/> is null.
        /// </summary>
        /// <returns>
        /// A string representation of the <see cref="Exception"/> property, or null if
        /// <see cref="Exception"/> is null.
        /// </returns>
        /// <remarks>
        /// This method is different than calling <see cref="Exception.ToString()"/>. The formatting is
        /// much better, and it displays the names/values of all public properties.
        /// </remarks>
        public string GetExceptionData() => Exception?.FormatToString();

        /// <summary>
        /// Sets values of the <see cref="ExtendedProperties"/> property according to the
        /// <paramref name="extendedProperties"/> parameter.
        /// </summary>
        /// <param name="extendedProperties">
        /// An object whose properties are added to the <see cref="ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to <see cref="ExtendedProperties"/>.
        /// </param>
        public void SetExtendedProperties(object extendedProperties)
        {
            if (extendedProperties == null)
                return;
            if (extendedProperties is IEnumerable<KeyValuePair<string, object>> stringDictionary)
                foreach (var item in stringDictionary)
                    ExtendedProperties[item.Key] = item.Value;
            else if (TryGetStringDictionaryItemAccessors(extendedProperties, out var getKey, out var getValue))
                foreach (object item in (IEnumerable)extendedProperties)
                    ExtendedProperties[getKey(item)] = getValue(item);
            else if (extendedProperties is IDictionary dictionary)
                foreach (var key in dictionary.Keys.OfType<string>())
                    ExtendedProperties[key] = dictionary[key];
            else
                foreach (var setExtendedProperty in GetSetExtendedPropertyActions(extendedProperties.GetType()))
                    setExtendedProperty(this, extendedProperties);
        }

        /// <summary>
        /// Sets values of the <see cref="ExtendedProperties"/> property according to the
        /// <paramref name="extendedProperties"/> parameter. Each extended property value is
        /// sanitized using the <see cref="SanitizeEngine.Sanitize"/> method.
        /// </summary>
        /// <param name="extendedProperties">
        /// An object whose properties are added to the <see cref="ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to <see cref="ExtendedProperties"/>.
        /// </param>
        /// <returns>This log entry.</returns>
        public LogEntry SetSanitizedExtendedProperties(object extendedProperties)
        {
            if (extendedProperties is null)
                return this;
            else if (extendedProperties is IEnumerable<KeyValuePair<string, object>> stringDictionary)
                foreach (var item in stringDictionary)
                    ExtendedProperties[item.Key] = SanitizeEngine.Sanitize(item.Value);
            else if (TryGetStringDictionaryItemAccessors(extendedProperties, out var getKey, out var getValue))
                foreach (object item in (IEnumerable)extendedProperties)
                    ExtendedProperties[getKey(item)] = SanitizeEngine.Sanitize(getValue(item));
            else if (extendedProperties is IDictionary dictionary)
                foreach (var key in dictionary.Keys.OfType<string>())
                    ExtendedProperties[key] = SanitizeEngine.Sanitize(dictionary[key]);
            else
                foreach (var setSafeExtendedProperty in GetSetSafeExtendedPropertyActions(extendedProperties.GetType()))
                    setSafeExtendedProperty(this, extendedProperties);
            return this;
        }

        /// <summary>
        /// Sets the value of an extended property. The <paramref name="value"/> is sanitized using
        /// the <see cref="SanitizeEngine.Sanitize"/> method.
        /// </summary>
        /// <param name="propertyName">The name of the extended property.</param>
        /// <param name="value">
        /// The value of the extended property. This will be sanitized using the  <see cref=
        /// "SanitizeEngine.Sanitize"/> method.
        /// </param>
        /// <returns></returns>
        public LogEntry SetSanitizedExtendedProperty(string propertyName, object value)
        {
            ExtendedProperties[propertyName] = SanitizeEngine.Sanitize(value);
            return this;
        }

        private static bool TryGetStringDictionaryItemAccessors(object extendedProperties, out Func<object, string> getKey, out Func<object, object> getValue)
        {
            (getKey, getValue) = _stringDictionaryItemAccessors.GetOrAdd(extendedProperties.GetType(), CreateStringDictionaryItemAccessors);
            return getKey != null && getValue != null;
        }

        private static (Func<object, string>, Func<object, object>) CreateStringDictionaryItemAccessors(Type type)
        {
            var kvpValueTypes =
               (from typeInterface in type.GetInterfaces()
                where typeInterface.IsGenericType && typeInterface.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                let enumerableType = typeInterface.GetGenericArguments()[0]
                where enumerableType.IsGenericType && enumerableType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)
                let kvpTypeArgs = enumerableType.GetGenericArguments()
                where kvpTypeArgs[0] == typeof(string)
                select kvpTypeArgs[1]).ToList();

            if (kvpValueTypes.Count != 1)
                return (null, null);

            var kvpType = typeof(KeyValuePair<,>).MakeGenericType(typeof(string), kvpValueTypes[0]);

            var keyProperty = kvpType.GetProperty("Key");
            var valueProperty = kvpType.GetProperty("Value");

            return (keyProperty.CreateGetter<string>(), valueProperty.CreateGetter());
        }

        private static IReadOnlyCollection<Action<LogEntry, object>> GetSetExtendedPropertyActions(Type type) =>
            _setExtendedPropertyActionsCache.GetOrAdd(type, t =>
                t.GetProperties(PublicInstance).Select(GetSetExtendedPropertyAction).ToArray());

        private static Action<LogEntry, object> GetSetExtendedPropertyAction(PropertyInfo property)
        {
            var getPropertyValue = property.CreateGetter();
            return (logEntry, extendedPropertiesObject) =>
                logEntry.ExtendedProperties[property.Name] = getPropertyValue(extendedPropertiesObject);
        }

        private static IReadOnlyCollection<Action<LogEntry, object>> GetSetSafeExtendedPropertyActions(Type type) =>
            _setSafeExtendedPropertyActionsCache.GetOrAdd(type, t =>
                t.GetProperties(PublicInstance).Select(GetSetSafeExtendedPropertyAction).ToArray());

        private static Action<LogEntry, object> GetSetSafeExtendedPropertyAction(PropertyInfo property)
        {
            var getPropertyValue = property.CreateGetter();
            return (logEntry, extendedPropertiesObject) =>
                logEntry.ExtendedProperties[property.Name] = SanitizeEngine.Sanitize(getPropertyValue(extendedPropertiesObject));
        }
    }
}