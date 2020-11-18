using System;
using System.Collections.Concurrent;

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
            // If it's a "value" type, string, dictionary, list, or array, return identity function.
            return obj => obj;
        }
    }
}
