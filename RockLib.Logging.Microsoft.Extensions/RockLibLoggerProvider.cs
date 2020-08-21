using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;

namespace RockLib.Logging
{
    [ProviderAlias("RockLibLogger")]
    public class RockLibLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, RockLibLogger> _loggers = new ConcurrentDictionary<string, RockLibLogger>();

        private readonly IDisposable _optionsReloadToken;
        private bool _includeScopes;
        private IExternalScopeProvider _scopeProvider;

        public RockLibLoggerProvider(ILogger logger, IOptionsMonitor<RockLibLoggerOptions> options = null)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (options != null)
            {
                _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
                var optionsName = GetOptionsName(logger);
                ReloadLoggerOptions(options.Get(optionsName), optionsName);
            }
        }

        public ILogger Logger { get; }

        public bool IncludeScopes
        {
            get => _includeScopes;
            set
            {
                _includeScopes = value;
                var scopeProvider = ScopeProvider;
                foreach (var logger in _loggers.Values)
                    logger.ScopeProvider = scopeProvider;
            }
        }

        public IExternalScopeProvider ScopeProvider
        {
            get => IncludeScopes
                ? _scopeProvider ?? (_scopeProvider = new LoggerExternalScopeProvider())
                : null;
            set
            {
                _scopeProvider = value;
                if (IncludeScopes)
                    foreach (var logger in _loggers.Values)
                        logger.ScopeProvider = value;
            }
        }

        public RockLibLogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, CreateLoggerInstance);

        Microsoft.Extensions.Logging.ILogger ILoggerProvider.CreateLogger(string categoryName) =>
            CreateLogger(categoryName);

        public void Dispose() =>
            _optionsReloadToken?.Dispose();

        private void ReloadLoggerOptions(RockLibLoggerOptions options, string optionsName)
        {
            if (OptionsNameMatchesLoggerName(optionsName))
            {
                _includeScopes = options.IncludeScopes;
                var scopeProvider = ScopeProvider;
                foreach (var logger in _loggers.Values)
                    logger.ScopeProvider = scopeProvider;
            }
        }

        private RockLibLogger CreateLoggerInstance(string categoryName) =>
            new RockLibLogger(Logger, categoryName, ScopeProvider);

        private bool OptionsNameMatchesLoggerName(string optionsName) =>
            string.Equals(optionsName, Logger.Name, StringComparison.OrdinalIgnoreCase)
                || (string.Equals(optionsName, Options.DefaultName, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(Logger.Name, Logging.Logger.DefaultName, StringComparison.OrdinalIgnoreCase));

        private static string GetOptionsName(ILogger logger) =>
            string.Equals(logger.Name, Logging.Logger.DefaultName, StringComparison.OrdinalIgnoreCase)
                ? Options.DefaultName
                : logger.Name;
    }
}
