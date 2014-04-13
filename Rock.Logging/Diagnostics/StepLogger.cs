using System.Collections.Generic;
using System.Text;

namespace Rock.Logging.Diagnostics
{
    public class StepLogger : IStepLogger
    {
        private readonly ILogger _logger;
        private readonly LogLevel _logLevel;
        private readonly string _message;
        private readonly System.Diagnostics.Stopwatch _stopwatch;

        private bool _isDisposed;

        private readonly List<IStep> _steps = new List<IStep>(); 

        public StepLogger(ILogger logger, LogLevel logLevel, string message)
        {
            _logger = logger;
            _logLevel = logLevel;
            _message = message;
            _stopwatch = System.Diagnostics.Stopwatch.StartNew();
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
                step.AddToReport(report);
            }

            report.AppendLine().AppendLine("Total Elapsed Time: " + _stopwatch.Elapsed);

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