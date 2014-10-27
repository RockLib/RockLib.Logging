using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Rock.Logging
{
    public static class WithCachingExtension
    {
        private static readonly ConditionalWeakTable<ILoggerFactory, CachedLoggerFactory> _cache = new ConditionalWeakTable<ILoggerFactory, CachedLoggerFactory>();

        public static ILoggerFactory WithCaching(this ILoggerFactory loggerFactory)
        {
            return _cache.GetValue(loggerFactory, factory => new CachedLoggerFactory(factory));
        }

        private class CachedLoggerFactory : ILoggerFactory
        {
            private readonly ConcurrentDictionary<Tuple<string, Type>, ILogger> _cachedLoggers = new ConcurrentDictionary<Tuple<string, Type>, ILogger>();

            private readonly ILoggerFactory _loggerFactory;

            public CachedLoggerFactory(ILoggerFactory loggerFactory)
            {
                _loggerFactory = loggerFactory;
            }

            public TLogger Get<TLogger>(string categoryName = null) where TLogger : ILogger
            {
                return
                    (TLogger)_cachedLoggers.GetOrAdd(
                        Tuple.Create(categoryName, typeof(TLogger)),
                        t => _loggerFactory.Get<TLogger>(categoryName));
            }
        }
    }
}