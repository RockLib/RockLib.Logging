using Microsoft.Extensions.Options;
using RockLib.Logging.LogProcessing;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using static RockLib.Logging.DependencyInjection.ServiceCollectionExtensions;
using RockLib.Logging.LogProviders;

namespace RockLib.Logging.DependencyInjection;

internal class ReloadingLogger : ILogger
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogProcessor _logProcessor;
    private readonly IReadOnlyCollection<ILogProvider> _logProviders;
    private readonly IReadOnlyCollection<IContextProvider> _contextProviders;
    private readonly Action<LoggerOptions> _configureOptions;
    private Logger _logger;
    private readonly IDisposable _changeListener;

    public ReloadingLogger(IServiceProvider serviceProvider,
        ILogProcessor logProcessor,
        string name,
        IReadOnlyCollection<ILogProvider> logProviders,
        IReadOnlyCollection<IContextProvider> contextProviders,
        IOptionsMonitor<LoggerOptions> optionsMonitor,
        LoggerOptions options,
        Action<LoggerOptions> configureOptions)
    {
        Name = name;
        _serviceProvider = serviceProvider;
        _logProcessor = logProcessor;
        _logProviders = logProviders;
        _contextProviders = contextProviders;
        _configureOptions = configureOptions;
        _logger = CreateLogger(_serviceProvider, options);

        _changeListener = optionsMonitor.OnChange(OptionsMonitorChanged);
    }

    public bool IsDisabled => _logger.IsDisabled;

    public IErrorHandler? ErrorHandler { get => _logger.ErrorHandler; set => _logger.ErrorHandler = value; }

    public string Name { get; }

    public LogLevel Level => _logger.Level;

    public IReadOnlyCollection<ILogProvider> LogProviders => _logger.LogProviders;

    public IReadOnlyCollection<IContextProvider> ContextProviders => _logger.ContextProviders;

    public void Dispose()
    {
        _changeListener.Dispose();
        _logger.Dispose();
    }

    public void Log(LogEntry logEntry, [CallerMemberName] string? callerMemberName = null, [CallerFilePath] string? callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0) => 
        _logger.Log(logEntry, callerMemberName, callerFilePath, callerLineNumber);

    private void OptionsMonitorChanged(LoggerOptions options, string name)
    {
        if (NamesEqual(Name, name))
        {
            _configureOptions?.Invoke(options);

            var oldLogger = _logger;
            var newLogger = CreateLogger(_serviceProvider, options);

            if (oldLogger.ErrorHandler is not null && newLogger.ErrorHandler is null)
            {
                newLogger.ErrorHandler = oldLogger.ErrorHandler;
            }

            _logger = newLogger;
        }
    }

    private Logger CreateLogger(IServiceProvider serviceProvider, LoggerOptions options) => 
        new(_logProcessor, Name, options.Level.GetValueOrDefault(), serviceProvider.GetService<ILogLevelResolver>()!,
            _logProviders, options.IsDisabled.GetValueOrDefault(), _contextProviders);
}