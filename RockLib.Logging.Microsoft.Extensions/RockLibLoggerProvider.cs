using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace RockLib.Logging;

using static Logger;

/// <summary>
/// Represents a type that can create instances of <see cref="RockLibLogger"/>.
/// </summary>
[ProviderAlias(nameof(RockLibLogger))]
public class RockLibLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, RockLibLogger> _loggers = new();

    private readonly IDisposable? _optionsReloadToken;
    private bool _includeScopes;
    private IExternalScopeProvider? _scopeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="RockLibLoggerProvider"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> that will record logs.</param>
    /// <param name="options">
    /// A delegate to configure the <see cref="RockLibLoggerProvider"/>.
    /// </param>
    public RockLibLoggerProvider(ILogger logger, IOptionsMonitor<RockLibLoggerOptions>? options = null)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (options is not null)
        {
            _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
            var optionsName = GetOptionsName(logger);
            ReloadLoggerOptions(options.Get(optionsName), optionsName);
        }
    }

    /// <summary>
    /// Gets the <see cref="ILogger"/> that ultimately records logs for the logger provider.
    /// </summary>
#pragma warning disable CA1721 // Property names should not match get methods
    public ILogger Logger { get; }
#pragma warning restore CA1721 // Property names should not match get methods

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
            {
                logger.ScopeProvider = scopeProvider;
            }
        }
    }

    /// <summary>
    /// Gets or sets the external scope information source for the logger provider.
    /// Note that this could return <c>null</c> if <see cref="IncludeScopes"/> is <c>false</c>.
    /// </summary>
    [DisallowNull]
    public IExternalScopeProvider ScopeProvider
    {
#pragma warning disable CS8603 // Possible null reference return.
        get => IncludeScopes
            ? (_scopeProvider ??= new LoggerExternalScopeProvider()) : null;
#pragma warning restore CS8603 // Possible null reference return.
        set
        {
            _scopeProvider = value ?? throw new ArgumentNullException(nameof(value));
            if (IncludeScopes)
            {
                foreach (var logger in _loggers.Values)
                {
                    logger.ScopeProvider = value;
                }
            }
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

#pragma warning disable CA1033 // Interface methods should be callable by child types
    Microsoft.Extensions.Logging.ILogger ILoggerProvider.CreateLogger(string categoryName) =>
        GetLogger(categoryName);
#pragma warning restore CA1033 // Interface methods should be callable by child types

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes reload token
    /// </summary>
    /// <param name="isDisposing">Set to <c>true</c> if invoked from <see cref="Dispose()"/>.</param>
    protected virtual void Dispose(bool isDisposing)
    {
        if(isDisposing)
        {
            _optionsReloadToken?.Dispose();
        }
    }

    /// <summary>
    /// Disposes any unmanaged resources.
    /// </summary>
    ~RockLibLoggerProvider() => Dispose(false);

    private void ReloadLoggerOptions(RockLibLoggerOptions options, string? optionsName)
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
        new(Logger, categoryName, ScopeProvider);

#pragma warning disable CA1820 // Test for empty strings using string length
    private bool OptionsNameMatchesLoggerName(string? optionsName) =>
        string.Equals(optionsName, Logger.Name, StringComparison.OrdinalIgnoreCase)
            || (string.Equals(optionsName, Options.DefaultName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(Logger.Name, DefaultName, StringComparison.OrdinalIgnoreCase));

    private static string GetOptionsName(ILogger logger) =>
        string.Equals(logger.Name, DefaultName, StringComparison.OrdinalIgnoreCase)
            ? Options.DefaultName
            : logger.Name;
#pragma warning restore CA1820 // Test for empty strings using string length
}
