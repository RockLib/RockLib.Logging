using System;
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

        private readonly List<IStepSnapshot> _snapshots = new List<IStepSnapshot>(); 

        public StepLogger(ILogger logger, LogLevel logLevel, string message)
        {
            _logger = logger;
            _logLevel = logLevel;
            _message = message;
            _stopwatch = System.Diagnostics.Stopwatch.StartNew();
        }

        public void AddStep(IStep step)
        {
            _snapshots.Add(step.GetSnapshot());
        }

        public void Flush()
        {
            _stopwatch.Stop();

            var sb = new StringBuilder();
            foreach (var snapshot in _snapshots)
            {
                snapshot.AddToReport(sb);
            }

            sb.AppendLine().AppendLine("Total Elapsed Time: " + _stopwatch.Elapsed);

            var logEntry = new LogEntry
            {
                LogLevel = _logLevel,
                Message = "Step Report" + (String.IsNullOrWhiteSpace(_message) ? "" : (": " + _message)),
            };

            logEntry.ExtendedProperties.Add("Step Report", sb.ToString());

            _logger.Log(logEntry);
        }

        public void Dispose()
        {
            Flush();
        }
    }
}