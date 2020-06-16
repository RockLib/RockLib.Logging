#if !NET451
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using static RockLib.Logging.DependencyInjection.ServiceCollectionExtensions;

namespace RockLib.Logging.DependencyInjection
{
    internal class OptionsMonitorReloadingLogProvider<TOptions> : ILogProvider
    {
        private readonly Func<TOptions, ILogProvider> _createLogProvider;
        private readonly string _name;

        private ILogProvider _logProvider;

        public OptionsMonitorReloadingLogProvider(IOptionsMonitor<TOptions> optionsMonitor, TOptions options,
            Func<TOptions, ILogProvider> createLogProvider, string name)
        {
            _createLogProvider = createLogProvider;
            _name = name;
            _logProvider = _createLogProvider(options);

            optionsMonitor.OnChange(OptionsMonitorChanged);
        }

        public TimeSpan Timeout => _logProvider.Timeout;

        public LogLevel Level => _logProvider.Level;

        public Task WriteAsync(LogEntry logEntry, CancellationToken cancellationToken) =>
            _logProvider.WriteAsync(logEntry, cancellationToken);

        private void OptionsMonitorChanged(TOptions options, string name)
        {
            if (NamesEqual(_name, name))
                _logProvider = _createLogProvider(options);
        }
    }
}
#endif
