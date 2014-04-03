using System;
using System.Collections.Generic;
using System.Linq;

namespace Rock.Framework
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Concat<T>(this T instance, IEnumerable<T> items)
        {
            return Enumerable.Repeat(instance, 1).Concat(items);
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T instance)
        {
            return items.Concat(Enumerable.Repeat(instance, 1));
        }
    }
}