using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using static RockLib.Logging.DependencyInjection.ServiceCollectionExtensions;

namespace RockLib.Logging.DependencyInjection;

internal class ReloadingLogProvider<TOptions> : ILogProvider
{
    private readonly Func<TOptions, ILogProvider> _createLogProvider;
    private readonly string _name;
    private readonly Action<TOptions> _configureOptions;
    private ILogProvider _logProvider;
    private readonly IDisposable _changeListener;

    public ReloadingLogProvider(IOptionsMonitor<TOptions> optionsMonitor, TOptions options,
        Func<TOptions, ILogProvider> createLogProvider, string name, Action<TOptions> configureOptions)
    {
        _createLogProvider = createLogProvider;
        _name = name;
        _configureOptions = configureOptions;
        _logProvider = _createLogProvider(options);

        _changeListener = optionsMonitor.OnChange(OptionsMonitorChanged);
    }

    public TimeSpan Timeout => _logProvider.Timeout;

    public LogLevel Level => _logProvider.Level;

    public Task WriteAsync(LogEntry logEntry, CancellationToken cancellationToken) =>
        _logProvider.WriteAsync(logEntry, cancellationToken);

    private void OptionsMonitorChanged(TOptions options, string name)
    {
        if (NamesEqual(_name, name))
        {
            _configureOptions?.Invoke(options);
            _logProvider = _createLogProvider(options);
        }
    }

    ~ReloadingLogProvider()
    {
        _changeListener.Dispose();
    }
}