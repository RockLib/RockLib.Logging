using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Rock.Logging.Diagnostics
{
    public class StepLogger : IStepLogger
    {
        private readonly ILogger _logger;
        private readonly LogLevel _logLevel;
        private readonly string _message;
        private readonly List<IStep> _steps;
        private readonly Stopwatch _stopwatch;

        private bool _isDisposed;

        public StepLogger(ILogger logger, LogLevel logLevel, string message)
        {
            _logger = logger;
            _logLevel = logLevel;
            _message = message;
            _steps = new List<IStep>();
            _stopwatch = Stopwatch.StartNew();
        }

        public void AddStep(IStep step)
        {
            _steps.Add(step);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _stopwatch.Stop();
            _isDisposed = true;

            var report = new StringBuilder();
            foreach (var step in _steps)
            {
                step.AddToReport(report.Append("-"));
            }

            report.AppendLine("Total Elapsed: " + _stopwatch.Elapsed);

            var logEntry = new LogEntry
            {
                LogLevel = _logLevel,
                Message = _message ?? "Step Report",
            };

            logEntry.ExtendedProperties.Add("Step Report", report.ToString());

            _logger.Log(logEntry);
        }
    }
}