using RockLib.Reflection.Optimized;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RockLib.Logging.SafeLogging;

/// <summary>
/// Defines a method for sanitizing an object.
/// </summary>
public static class SanitizeEngine
{
    private static readonly Type _runtimeTypeType = typeof(SanitizeEngine).GetType();

    private static readonly ConcurrentDictionary<Type, Func<object, object>> _sanitizeFunctions = new();

    internal static readonly HashSet<Type> SafeTypes = new();
    internal static readonly HashSet<PropertyInfo> SafeProperties = new();
    internal static readonly HashSet<PropertyInfo> NotSafeProperties = new();

    /// <summary>
    /// Ensures that any properties not decorated with the [SafeToLog] attribute are excluded
    /// from logs. Note that non-complex types (e.g. primitive types, enums, string) are not
    /// modified.
    /// <para>
    /// A sanitized object is represented by a <c>Dictionary&lt;string, object&gt;</c>
    /// containing only properties decorated with the [SafeToLog] attribute. This
    /// means that a sanitized object essentially loses its type.
    /// </para>
    /// </summary>
    /// <param name="value">The object to sanitize.</param>
    /// <returns>
    /// Either a <c>Dictionary&lt;string, object&gt;</c> or the <paramref name="value"/>
    /// parameter itself, depending on whether the object is a complex type.
    /// </returns>
    public static object? Sanitize(object? value)
    {
        if (value is null)
        {
            return null;
        }

        var sanitize = _sanitizeFunctions.GetOrAdd(value.GetType(), GetSanitizeFunction);
        return sanitize(value);
    }

    /// <summary>
    /// A function that determines whether a given type is safe to log.
    /// <para>Set this value at the "beginning" of your application.</para>
    /// </summary>
    public static Func<Type, bool>? IsTypeSafeToLog { get; set; }

    private static Func<object, object> GetSanitizeFunction(Type runtimeType)
    {
        if (IsSafeToLogType(runtimeType) || IsBasicType(runtimeType))
        {
            return SanitizeNothing;
        }
        else if (IsStringDictionary(runtimeType))
        {
            return SanitizeStringDictionary;
        }
        else if (IsCollection(runtimeType))
        {
            return SanitizeCollection;
        }
        else if (IsDictionaryEntry(runtimeType))
        {
            return SanitizeDictionaryEntry;
        }
        else if (IsKeyValuePair(runtimeType, out var keyType, out var valueType))
        {
            return GetSanitizeKeyValuePairFunction(keyType, valueType);
        }
        else
        {
            return GetSanitizeObjectFunction(runtimeType);
        }
    }

    private static bool IsSafeToLogType(Type runtimeType) =>
        IsTypeSafeToLog?.Invoke(runtimeType) == true;

    private static bool IsBasicType(Type runtimeType) =>
        runtimeType.IsPrimitive
        || runtimeType.IsEnum
        || runtimeType == typeof(string)
        || runtimeType == typeof(decimal)
        || runtimeType == typeof(DateTime)
        || runtimeType == typeof(TimeSpan)
        || runtimeType == typeof(DateTimeOffset)
        || runtimeType == typeof(Guid)
        || runtimeType == typeof(Uri)
        || typeof(Encoding).IsAssignableFrom(runtimeType)
        || runtimeType == _runtimeTypeType;

    private static bool IsCollection(Type runtimeType) =>
        typeof(IEnumerable).IsAssignableFrom(runtimeType);

    private static bool IsKeyValuePair(Type runtimeType, out Type? keyType, out Type? valueType)
    {
        if (runtimeType.IsValueType
            && runtimeType.IsGenericType
            && runtimeType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
        {
            var genericTypeArguments = runtimeType.GetGenericArguments();
            keyType = genericTypeArguments[0];
            valueType = genericTypeArguments[1];
            return true;
        }
        keyType = null;
        valueType = null;
        return false;
    }

    private static bool IsDictionaryEntry(Type runtimeType) =>
        runtimeType == typeof(DictionaryEntry);

    private static bool IsStringDictionary(Type runtimeType) =>
        runtimeType.GetInterfaces().Any(IsStringDictionaryInterface);

    private static bool IsStringDictionaryInterface(Type interfaceType)
    {
        if (interfaceType.IsGenericType)
        {
            var genericTypeDefinition = interfaceType.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(IDictionary<,>) || genericTypeDefinition == typeof(IReadOnlyDictionary<,>))
                return interfaceType.GetGenericArguments()[0] == typeof(string);
        }
        return false;
    }

    private static object SanitizeNothing(object value) => value;

    private static object SanitizeCollection(object value)
    {
        var collection = new ArrayList();
        foreach (var item in (IEnumerable)value)
        {
            collection.Add(Sanitize(item));
        }
        return collection;
    }

    private static object SanitizeDictionaryEntry(object value)
    {
        var item = (DictionaryEntry)value;
        return new DictionaryEntry(item.Key, Sanitize(item.Value));
    }

    private static object SanitizeKeyValuePair<TKey, TValue>(object value)
    {
        var item = (KeyValuePair<TKey, TValue>)value;
        return new KeyValuePair<TKey, object>(item.Key, Sanitize(item.Value)!);
    }

    private static object SanitizeStringDictionary(object value) =>
        ((IEnumerable)SanitizeCollection(value))
            .Cast<KeyValuePair<string, object>>()
            .ToDictionary(item => item.Key, item => item.Value);

    private static Func<object, object> GetSanitizeKeyValuePairFunction(Type? keyType, Type? valueType)
    {
        var sanitizeKeyValuePairMethod = typeof(SanitizeEngine)
            .GetMethod(nameof(GetSanitizeKeyValuePairFunction), BindingFlags.NonPublic | BindingFlags.Static, null, Type.EmptyTypes, null)
            .MakeGenericMethod(keyType, valueType);
        return (Func<object, object>)sanitizeKeyValuePairMethod.Invoke(null, null);
    }

    private static Func<object, object> GetSanitizeKeyValuePairFunction<TKey, TValue>() =>
        SanitizeKeyValuePair<TKey, TValue>;

    private static Func<object, object> GetSanitizeObjectFunction(Type type)
    {
        var allProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        if (allProperties.Length == 0)
        {
            return SanitizeNothing;
        }

        var safeProperties = GetSafeProperties(type, allProperties)
            .Select(property => new { property.Name, GetValue = property.CreateGetter() })
            .ToArray();

        if (safeProperties.Length == 0)
        {
            var everythingSanitizedMessage = GetEverythingSanitizedMessage(type);
            return SanitizeEverything;
            object SanitizeEverything(object value) => everythingSanitizedMessage;
        }

        // TODO: Add recursion busting?

        return SanitizeObject;
        object SanitizeObject(object value)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var property in safeProperties)
            {
                dictionary.Add(property.Name, Sanitize(property.GetValue(value))!);
            }
            return dictionary;
        }
    }

    private static string GetEverythingSanitizedMessage(Type type) =>
        $"All properties from the {type.FullName} type have been excluded from the log entry extended properties because none were decorated with the [SafeToLog] attribute.";

    private static IEnumerable<PropertyInfo> GetSafeProperties(Type type, IReadOnlyCollection<PropertyInfo> allProperties)
    {
        if (Attribute.IsDefined(type, typeof(SafeToLogAttribute), inherit: false)
                || SafeTypes.Contains(type))
        {
            return allProperties.Where(property =>
                !Attribute.IsDefined(property, typeof(NotSafeToLogAttribute), inherit: false)
                    && !NotSafeProperties.Contains(property));
        }

        return allProperties.Where(property =>
            Attribute.IsDefined(property, typeof(SafeToLogAttribute), inherit: false)
                || SafeProperties.Contains(property));
    }
}
