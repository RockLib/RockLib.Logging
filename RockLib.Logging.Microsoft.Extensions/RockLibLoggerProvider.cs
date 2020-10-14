using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;

namespace RockLib.Logging
{
    using static Logger;

    /// <summary>
    /// Represents a type that can create instances of <see cref="RockLibLogger"/>.
    /// </summary>
    [ProviderAlias(nameof(RockLibLogger))]
    public class RockLibLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, RockLibLogger> _loggers = new ConcurrentDictionary<string, RockLibLogger>();

        private readonly IDisposable _optionsReloadToken;
        private bool _includeScopes;
        private IExternalScopeProvider _scopeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RockLibLoggerProvider"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> that will record logs.</param>
        /// <param name="options">
        /// A delegate to configure the <see cref="RockLibLoggerProvider"/>.
        /// </param>
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

        /// <summary>
        /// Gets the <see cref="ILogger"/> that ultimately records logs for the logger provider.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to include scopes when logging.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the external scope information source for the logger provider.
        /// </summary>
        public IExternalScopeProvider ScopeProvider
        {
            get => IncludeScopes
                ? _scopeProvider ?? (_scopeProvider = new LoggerExternalScopeProvider())
                : null;
            set
            {
                _scopeProvider = value ?? throw new ArgumentNullException(nameof(value));
                if (IncludeScopes)
                    foreach (var logger in _loggers.Values)
                        logger.ScopeProvider = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="RockLibLogger"/> for the specified category name.
        /// </summary>
        /// <param name="categoryName">The category name of the logger.</param>
        /// <returns>
        /// The same instance of <see cref="RockLibLogger"/> given the same category name.
        /// </returns>
        public RockLibLogger GetLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName ?? throw new ArgumentNullException(nameof(categoryName)), CreateLoggerInstance);

        Microsoft.Extensions.Logging.ILogger ILoggerProvider.CreateLogger(string categoryName) =>
            GetLogger(categoryName);

        /// <inheritdoc/>
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
                    && string.Equals(Logger.Name, DefaultName, StringComparison.OrdinalIgnoreCase));

        private static string GetOptionsName(ILogger logger) =>
            string.Equals(logger.Name, DefaultName, StringComparison.OrdinalIgnoreCase)
                ? Options.DefaultName
                : logger.Name;
    }
}
