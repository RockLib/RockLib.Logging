using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RockLib.Logging
{
    /// <summary>
    /// Provides extension methods for the <see cref=" PropertyInfo"/> type, creating
    /// optimized functions that retrieve the value of a <see cref="PropertyInfo"/>.
    /// </summary>
    internal static class GetGetFuncExtension
    {
        /// <summary>
        /// Gets a Func&lt;object, object&gt; that, when invoked, returns the value
        /// of the property represented by <paramref name="propertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">
        /// The <see cref="PropertyInfo"/> that represents the property whose value 
        /// is returned by the return function.
        /// </param>
        /// <returns>
        /// A Func&lt;object, object&gt; that, when invoked, returns the value
        /// of the property represented by <paramref name="propertyInfo"/>.
        /// </returns>
        public static Func<object, object> GetGetFunc(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetGetFunc<object, object>();
        }

        /// <summary>
        /// Gets a <see cref="Func{T, TResult}"/> that, when invoked, returns the value
        /// of the property represented by <paramref name="propertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">
        /// The <see cref="PropertyInfo"/> that represents the property whose value 
        /// is returned by the return function.
        /// </param>
        /// <typeparam name="TInstance">
        /// The type of the input parameter of the resulting <see cref="Func{T, TResult}"/>.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The return type of the resulting <see cref="Func{T, TResult}"/>.
        /// </typeparam>
        /// <returns>
        /// A <see cref="Func{T, TResult}"/> that, when invoked, returns the value
        /// of the property represented by <paramref name="propertyInfo"/>.
        /// </returns>
        public static Func<TInstance, TProperty> GetGetFunc<TInstance, TProperty>(this PropertyInfo propertyInfo)
        {
            var instanceParameter = Expression.Parameter(typeof(TInstance), "instance");

            var body =
                Expression.Property(
                    instanceParameter.EnsureConvertableTo(propertyInfo.ReflectedType),
                    propertyInfo);

            var lambda =
                Expression.Lambda<Func<TInstance, TProperty>>(
                    body.EnsureConvertableTo<TProperty>(),
                    "Get" + propertyInfo.Name,
                    new[] { instanceParameter });

            return lambda.Compile();
        }

        private static Expression EnsureConvertableTo<T>(this Expression expression)
        {
            return expression.EnsureConvertableTo(typeof(T));
        }

        private static Expression EnsureConvertableTo(this Expression expression, Type type)
        {
            if (expression.Type.IsLessSpecificThan(type)
                || expression.Type.RequiresBoxingWhenConvertingTo(type))
            {
                return Expression.Convert(expression, type);
            }

            return expression;
        }

        public static bool RequiresBoxingWhenConvertingTo(this Type fromType, Type toType)
        {
            return fromType.IsValueType && !toType.IsValueType;
        }
    }
}
