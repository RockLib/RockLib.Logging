using RockLib.Configuration;
using RockLib.Configuration.ObjectFactory;
using RockLib.Immutable;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RockLib.Logging
{
    public static class LoggerFactory
    {
        private static readonly ConcurrentDictionary<string, Logger> _lookup = new ConcurrentDictionary<string, Logger>(StringComparer.InvariantCultureIgnoreCase);

        private static readonly Semimutable<IReadOnlyCollection<Logger>> _loggers = new Semimutable<IReadOnlyCollection<Logger>>(GetDefaultLoggers);

        public static IReadOnlyCollection<Logger> Loggers => _loggers.Value;

        public static void SetLoggers(IReadOnlyCollection<Logger> loggers) => _loggers.Value = loggers;

        public static Logger GetInstance(string name = Logger.DefaultName) => _lookup.GetOrAdd(name ?? Logger.DefaultName, FindLogger);

        private static Logger FindLogger(string name) =>
            Loggers.FirstOrDefault(logger => logger.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            ?? throw new KeyNotFoundException(); // TODO: Add exception message

        private static IReadOnlyCollection<Logger> GetDefaultLoggers() =>
             Config.Root.GetSection("rocklib.logging").Create<IReadOnlyCollection<Logger>>();
    }
}
