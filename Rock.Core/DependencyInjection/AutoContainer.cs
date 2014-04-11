using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Rock.DependencyInjection
{
    public class AutoContainer : IResolver
    {
        private readonly IDictionary<Type, Func<object>> _bindings;

        private AutoContainer(IDictionary<Type, Func<object>> bindings)
        {
            _bindings = bindings;
        }

        public AutoContainer(params object[] instances)
            : this(new Dictionary<Type, Func<object>>())
        {
            foreach (var instance in instances.Where(x => x != null))
            {
                BindConstant(instance.GetType(), instance);
            }
        }

        public virtual bool CanResolve(Type type)
        {
            return _bindings.ContainsKey(type) || CanCreateInstance(type);
        }

        public T Get<T>()
        {
            var instance = Get(typeof(T));

            if (instance == null)
            {
                return default(T);
            }

            return (T)instance;
        }

        public virtual object Get(Type type)
        {
            Func<object> getInstance;

            if (!_bindings.TryGetValue(type, out getInstance))
            {
                if (CanCreateInstance(type))
                {
                    getInstance = GetCreateInstanceFunc(type);
                    _bindings[type] = getInstance;
                }
                else
                {
                    return null;
                }
            }

            if (getInstance == null)
            {
                return null;
            }

            return getInstance();
        }

        public AutoContainer MergeWith(IResolver otherContainer)
        {
            return new MergedAutoContainer(_bindings, otherContainer);
        }

        IResolver IResolver.MergeWith(IResolver otherContainer)
        {
            return MergeWith(otherContainer);
        }

        private Func<object> GetCreateInstanceFunc(Type type)
        {
            var ctor =
                type.GetConstructors()
                    .Where(c => c.GetParameters().All(p => CanResolve(p.ParameterType)))
                    .GroupBy(c => c.GetParameters().Length)
                    .OrderByDescending(g => g.Key)
                    .Where(g => g.Count() == 1)
                    .Select(g => g.Single())
                    .FirstOrDefault();

            if (ctor == null)
            {
                return null;
            }

            var getMethodOpenGeneric = GetType().GetMethod("Get", Type.EmptyTypes);
            var thisExpression = Expression.Constant(this);

            var createInstanceExpression =
                Expression.Lambda<Func<object>>(
                    Expression.New(
                        ctor,
                        ctor.GetParameters()
                            .Select(
                                p =>
                                Expression.Call(
                                    thisExpression,
                                    getMethodOpenGeneric.MakeGenericMethod(p.ParameterType)))));

            return createInstanceExpression.Compile();
        }

        public AutoContainer BindInstance<TContract, TImplementation>()
        {
            return BindInstance(typeof(TContract), typeof(TImplementation));
        }

        public AutoContainer BindInstance<TContract>(Func<TContract> createInstance)
        {
            return BindInstance(typeof(TContract), () => createInstance());
        }

        public AutoContainer BindInstance(Type contractType, Type implementationType)
        {
            if (!contractType.IsAssignableFrom(implementationType))
            {
                return this;
            }

            if (!CanCreateInstance(implementationType))
            {
                return this;
            }

            var createInstance = GetCreateInstanceFunc(implementationType);
            return BindInstance(contractType, createInstance);
        }

        public AutoContainer BindInstance(Type contractType, Func<object> createInstance)
        {
            foreach (var type in GetTypeHierarchy(contractType))
            {
                AddInstanceMapping(type, createInstance);
            }

            return this;
        }

        private void AddInstanceMapping(Type contractType, Func<object> createInstance)
        {
            _bindings[contractType] = createInstance;
        }

        public AutoContainer BindSingleton<TContract, TImplementation>()
        {
            return BindSingleton(typeof(TContract), typeof(TImplementation));
        }

        public AutoContainer BindSingleton<TContract>(Func<TContract> createInstance)
        {
            return BindSingleton(typeof(TContract), () => createInstance());
        }

        public AutoContainer BindSingleton(Type contractType, Type implementationType)
        {
            if (!contractType.IsAssignableFrom(implementationType))
            {
                return this;
            }

            if (!CanCreateInstance(implementationType))
            {
                return this;
            }

            var createInstance = GetCreateInstanceFunc(implementationType);
            return BindSingleton(contractType, createInstance);
        }

        public AutoContainer BindSingleton(Type contractType, Func<object> createInstance)
        {
            var lazyInstance = new Lazy<object>(createInstance);

            foreach (var type in GetTypeHierarchy(contractType))
            {
                AddSingletonMapping(type, lazyInstance);
            }

            return this;
        }

        private void AddSingletonMapping(Type contractType, Lazy<object> lazyInstance)
        {
            _bindings[contractType] = () => lazyInstance.Value;
        }

        public AutoContainer BindConstant<TContract>(TContract instance)
        {
            return BindConstant(typeof(TContract), instance);
        }

        public AutoContainer BindConstant(Type contractType, object instance)
        {
            if (instance == null)
            {
                return this;
            }

            foreach (var type in GetTypeHierarchy(contractType))
            {
                AddConstantMapping(type, instance);
            }

            return this;
        }

        private bool CanCreateInstance(Type type)
        {
            return
                !type.IsAbstract
                && type.GetConstructors().Any(c => c.GetParameters().All(p => CanResolve(p.ParameterType)));
        }

        private void AddConstantMapping(Type type, object instance)
        {
            _bindings[type] = () => instance;
        }

        private static IEnumerable<Type> GetTypeHierarchy(Type type)
        {
            var types = Enumerable.Repeat(type, 1);

            if (type == typeof(object))
            {
                return types;
            }

            if (!type.IsInterface)
            {
                types = types.Concat(GetTypeHierarchy(type.BaseType));
            }

            types = type.GetInterfaces().Aggregate(types, (typesSoFar, @interface) => typesSoFar.Concat(GetTypeHierarchy(@interface)));

            return types.Distinct();
        }

        private class MergedAutoContainer : AutoContainer
        {
            private readonly IResolver _otherContainer;

            public MergedAutoContainer(IDictionary<Type, Func<object>> bindings, IResolver otherContainer)
                : base(bindings)
            {
                _otherContainer = otherContainer;
            }

            public override bool CanResolve(Type type)
            {
                return base.CanResolve(type) || _otherContainer.CanResolve(type);
            }

            public override object Get(Type type)
            {
                if (base.CanResolve(type))
                {
                    return base.Get(type);
                }

                return _otherContainer.Get((type));
            }
        }
    }
}