using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;

namespace RockLib.Logging
{
    public class RockLibLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, RockLibLogger> _loggers = new ConcurrentDictionary<string, RockLibLogger>();

        private readonly IDisposable _optionsReloadToken;
        private bool _includeScopes;
        private IExternalScopeProvider _scopeProvider;

        public RockLibLoggerProvider(ILogger logger, IOptionsMonitor<RockLibLoggerOptions> options)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
            ReloadLoggerOptions(options.Get(logger.Name), logger.Name);
        }

        public ILogger Logger { get; }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, CreateLoggerInstance);

        public void Dispose() =>
            _optionsReloadToken.Dispose();

        public void SetScopeProvider(IExternalScopeProvider scopeProvider) =>
            _scopeProvider = scopeProvider;

        private void ReloadLoggerOptions(RockLibLoggerOptions options, string name)
        {
            if (string.Equals(name, Logger.Name, StringComparison.OrdinalIgnoreCase))
            {
                _includeScopes = options.IncludeScopes;
                var scopeProvider = GetScopeProvider();
                foreach (var logger in _loggers.Values)
                    logger.ScopeProvider = scopeProvider;
            }
        }

        private RockLibLogger CreateLoggerInstance(string categoryName) =>
            new RockLibLogger(Logger, categoryName, GetScopeProvider());

        private IExternalScopeProvider GetScopeProvider()
        {
            if (!_includeScopes)
                return null;
            if (_scopeProvider == null)
                _scopeProvider = new LoggerExternalScopeProvider();
            return _scopeProvider;
        }
    }
}
