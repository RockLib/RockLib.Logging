using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Example.GenericHost
{
    public class ExampleHostedService : IHostedService
    {
        private readonly ILogger<ExampleHostedService> _logger;
        private readonly Timer _timer;

        public ExampleHostedService(ILogger<ExampleHostedService> logger)
        {
            _logger = logger;
            _timer = new Timer(TimerElapsed);
        }

        private void TimerElapsed(object state)
        {
            _logger.LogInformation("Service timer elapsed at {Timestamp:O}", DateTime.UtcNow);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting service...");
            _timer.Change(0, 5000);
            _logger.LogInformation("Service started.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping service...");
            _timer.Change(-1, -1);
            _timer.Dispose();
            _logger.LogInformation("Service stopped.");
            return Task.CompletedTask;
        }
    }
}
