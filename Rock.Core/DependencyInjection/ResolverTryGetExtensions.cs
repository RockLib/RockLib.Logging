using System;

namespace Rock.DependencyInjection
{
    public static class ResolverTryGetExtensions
    {
        public static bool TryGet<T>(this IResolver resolver, out T instance)
        {
            if (resolver.CanResolve(typeof(T)))
            {
                try
                {
                    instance = resolver.Get<T>();
                    return true;
                }
                catch (Exception)
                {
                    instance = default(T);
                    return false;
                }
            }

            instance = default(T);
            return false;
        }

        public static bool TryGet(this IResolver resolver, Type type, out object instance)
        {
            if (resolver.CanResolve(type))
            {
                try
                {
                    instance = resolver.Get(type);
                    return true;
                }
                catch (Exception)
                {
                    instance = null;
                    return false;
                }
            }

            instance = null;
            return false;
        }
    }
}