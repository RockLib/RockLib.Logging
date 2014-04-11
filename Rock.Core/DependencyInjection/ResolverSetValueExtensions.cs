using System;

namespace Rock.DependencyInjection
{
    public static class ResolverSetValueExtensions
    {
        /// <summary>
        /// Set <paramref name="fieldOrVariable"/> to what <paramref name="resolver"/> returns from its
        /// <see cref="IResolver.Get{T}()"/> for type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to retrieve from <paramref name="resolver"/>.</typeparam>
        /// <param name="resolver">An object that can retrieve instances of arbitrary types.</param>
        /// <param name="fieldOrVariable">The value to set if <paramref name="resolver"/> can successfully do so.</param>
        public static void SetValueFor<T>(this IResolver resolver, out T fieldOrVariable)
        {
            SetValueFor(resolver, out fieldOrVariable, () => { throw UnableToResolveType<T>(); });
        }

        /// <summary>
        /// Set <paramref name="fieldOrVariable"/> to what <paramref name="resolver"/> returns from its
        /// <see cref="IResolver.Get{T}()"/> for type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to retrieve from <paramref name="resolver"/>.</typeparam>
        /// <param name="resolver">An object that can retrieve instances of arbitrary types.</param>
        /// <param name="fieldOrVariable">The value to set if <paramref name="resolver"/> can successfully do so.</param>
        /// <param name="getDefaultValue">
        /// If <paramref name="resolver"/> is unable to obtain an instance of <typeparamref name="T"/>, this
        /// function will be evaluated in order to obtain a default for <paramref name="fieldOrVariable"/>. If null, then the default value for <typeparamref name="T"/> (e.g. null for reference types) will be used.
        /// </param>
        public static void SetValueFor<T>(this IResolver resolver, out T fieldOrVariable, Func<T> getDefaultValue)
        {
            fieldOrVariable =
                resolver.CanResolve(typeof(T))
                    ? GetValue(resolver, getDefaultValue)
                    : GetValue(getDefaultValue);
        }

        /// <summary>
        /// Tries to get the value from <paramref name="resolver"/>. If it fails, it returns the value
        /// from <paramref name="getValue"/>.
        /// </summary>
        private static T GetValue<T>(IResolver resolver, Func<T> getValue)
        {
            try
            {
                return resolver.Get<T>();
            }
            catch
            {
                return GetValue(getValue);
            }
        }

        /// <summary>
        /// If <paramref name="getValue"/> is not null, return what it returns when evaluated. Otherwise,
        /// if it is null, return the default value of <typeparamref name="T"/> (e.g. null for reference types).
        /// </summary>
        private static T GetValue<T>(Func<T> getValue)
        {
            return
                getValue != null
                    ? getValue()
                    : default(T);
        }

        private static Exception UnableToResolveType<T>(Exception innerException = null)
        {
            return new Exception(string.Format("Unable to resolve type {0}.", typeof(T)), innerException); // TODO: better message
        }
    }
}