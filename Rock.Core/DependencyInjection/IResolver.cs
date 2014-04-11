using System;

namespace Rock.DependencyInjection
{
    public interface IResolver
    {
        bool CanResolve(Type type);
        T Get<T>();
        object Get(Type type);

        IResolver MergeWith(IResolver otheResolver);
    }
}