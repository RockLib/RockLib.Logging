namespace Rock.Framework
{
    using Reflectinator;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    public class ReflectinatorObjectToExpandoObjectConverter : IConvertObjectsTo<ExpandoObject>
    {
        private readonly ConcurrentDictionary<Type, Func<object, ExpandoObject>> _createFunctionCache =
            new ConcurrentDictionary<Type, Func<object, ExpandoObject>>();

        public ExpandoObject Convert(object sourceObject)
        {
            if (sourceObject == null)
            {
                return null;
            }

            var createExpandoObject =
                _createFunctionCache.GetOrAdd(
                    sourceObject.GetType(),
                    GetCreateFunction);

            return createExpandoObject(sourceObject);
        }

        private static Func<object, ExpandoObject> GetCreateFunction(Type objectType)
        {
            // TODO: Throw exception if the type is primitive or otherwise unsuitable for representation by an ExpandoObject.

            if (objectType == typeof(ExpandoObject))
            {
                return obj => (ExpandoObject)obj;
            }

            if (objectType
                .GetInterfaces()
                .Any(i => i == typeof(IDictionary<string, object>)))
            {
                return
                    obj =>
                    {
                        var expando = new ExpandoObject();

                        var sourceDictionary = (IDictionary<string, object>)obj;
                        var targetDictionary = (IDictionary<string, object>)expando;

                        foreach (var x in sourceDictionary)
                        {
                            targetDictionary[x.Key] = x.Value;
                        }

                        return expando;
                    };
            }

            Func<Type, bool> isIDictionaryOfString =
                t =>
                    t.IsGenericType
                    && t.GetGenericTypeDefinition() == typeof(IDictionary<,>)
                    && t.GetGenericArguments()[0] == typeof(string);

            if (objectType.GetInterfaces().Any(isIDictionaryOfString))
            {
                // Need a function to get the non-generic enumerator of the source dictionary
                // Need a function to access the Value property (field?) of KeyValuePair<string, ???>

                var invokeGetEnumerator =
                    Method.GetFuncMethod<IEnumerable, IEnumerator>(typeof(IEnumerable).GetMethods().Single())
                    .InvokeDelegate;
                Func<object, IEnumerator> getEnumerator = obj => invokeGetEnumerator((IEnumerable)obj);

                var keyValuePairType =
                    typeof(KeyValuePair<,>).MakeGenericType(
                        typeof(string),
                        objectType
                            .GetInterfaces()
                            .First(isIDictionaryOfString)
                            .GetGenericArguments()[1]);

                var invokeGetKey = Property.Get(keyValuePairType.GetProperty("Key")).GetFunc;
                Func<object, string> getKey = obj => (string)invokeGetKey(obj);
                var getValue = Property.Get(keyValuePairType.GetProperty("Value")).GetFunc;

                return
                    obj =>
                    {
                        var expando = new ExpandoObject();

                        var targetDictionary = (IDictionary<string, object>)expando;

                        var enumerator = getEnumerator(obj);
                        while (enumerator.MoveNext())
                        {
                            var x = enumerator.Current;
                            targetDictionary[getKey(x)] = getValue(x);
                        }

                        return expando;
                    };
            }

            var properties =
                TypeCrawler.Get(objectType)
                    .Properties
                    .Where(p => p.IsPublic && !p.IsStatic && p.CanRead && p.CanWrite) // TODO: support anonymous types
                    .Select(p => new { p.Name, GetValue = p.GetFunc })
                    .ToList();

            return
                obj =>
                {
                    var expando = new ExpandoObject();
                    var dictionary = (IDictionary<string, object>)expando;

                    foreach (var property in properties)
                    {
                        dictionary[property.Name] = property.GetValue(obj);
                    }

                    return expando;
                };
        }
    }
}