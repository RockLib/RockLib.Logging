using RockLib.Reflection.Optimized;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RockLib.Logging.SafeLogging
{
    internal static class RedactEngine
    {
        private static readonly ConcurrentDictionary<Type, Func<object, object>> _redactFunctions = new ConcurrentDictionary<Type, Func<object, object>>();

        public static object Redact(object value)
        {
            if (value is null)
                return null;
            var redact = _redactFunctions.GetOrAdd(value.GetType(), GetRedactFunction);
            return redact(value);
        }

        private static Func<object, object> GetRedactFunction(Type type)
        {
            // Unwrap if nullable.
            type = Nullable.GetUnderlyingType(type) ?? type;

            // If it's a "value" type, redact nothing.
            if (IsValueType(type))
                return RedactNothing;

            // If it's a collection, redact the members of each item (includes dictionaries).
            if (typeof(IEnumerable).IsAssignableFrom(type))
                return RedactCollection;

            // If it's a DictionaryEntry (i.e. we're enumerating non-generic dictionary items), redact the value.
            if (type == typeof(DictionaryEntry))
                return RedactDictionaryEntry;

            // If it's a KeyValuePair (i.e. we're enumerating generic dictionary items), redact the value.
            if (IsKeyValuePair(type, out Type keyType, out Type valueType))
            {
                var redactKeyValuePairMethod = typeof(RedactEngine)
                    .GetMethod(nameof(GetRedactKeyValuePairFunction), BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(keyType, valueType);
                return (Func<object, object>)redactKeyValuePairMethod.Invoke(null, null);
            }

            return GetRedactObjectPropertiesFunction(type);
        }

        private static bool IsValueType(Type type) =>
            type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(TimeSpan)
            || type == typeof(DateTimeOffset)
            || type == typeof(Guid)
            || type == typeof(Uri);

        private static bool IsKeyValuePair(Type type, out Type keyType, out Type valueType)
        {
            if (type.IsValueType
                && type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                var genericTypeArguments = type.GetGenericArguments();
                keyType = genericTypeArguments[0];
                valueType = genericTypeArguments[1];
                return true;
            }

            keyType = null;
            valueType = null;
            return false;
        }

        private static object RedactNothing(object value) => value;

        private static object RedactCollection(object value)
        {
            var collection = new List<object>();

            foreach (var item in (IEnumerable)value)
                collection.Add(Redact(item));

            return collection;
        }

        private static object RedactDictionaryEntry(object value)
        {
            var item = (DictionaryEntry)value;
            return new DictionaryEntry(item.Key, Redact(item.Value));
        }

        private static object RedactKeyValuePair<TKey, TValue>(object value)
        {
            var item = (KeyValuePair<TKey, TValue>)value;
            return new KeyValuePair<TKey, object>(item.Key, Redact(item.Value));
        }

        private static Func<object, object> GetRedactKeyValuePairFunction<TKey, TValue>() =>
            RedactKeyValuePair<TKey, TValue>;

        private static Func<object, object> GetRedactObjectPropertiesFunction(Type type)
        {
            var safeProperties = GetSafeProperties(type)
                .Select(p => new { p.Name, GetValue = p.CreateGetter() })
                .ToArray();

            return value =>
            {
                var dictionary = new Dictionary<string, object>();

                foreach (var property in safeProperties)
                    dictionary.Add(property.Name, Redact(property.GetValue(value)));

                return dictionary;
            };
        }

        private static IEnumerable<PropertyInfo> GetSafeProperties(Type type)
        {
            var allProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (Attribute.IsDefined(type, typeof(SafeToLogAttribute), inherit: false))
                return allProperties.Where(property =>
                    !Attribute.IsDefined(property, typeof(NotSafeToLogAttribute), inherit: false));

            return allProperties.Where(property =>
                Attribute.IsDefined(property, typeof(SafeToLogAttribute), inherit: false));
        }
    }
}
