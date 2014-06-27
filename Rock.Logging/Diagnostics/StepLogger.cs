using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Rock.Logging.Defaults.Implementation;

namespace Rock.Logging.Diagnostics
{
    public class StepLogger : IStepLogger
    {
        private readonly ILogger _logger;
        private readonly LogLevel _logLevel;
        private readonly string _message;

        private readonly string _callerMemberName;
        private readonly string _callerFilePath;
        private readonly int _callerLineNumber;

        private readonly List<IStep> _steps;
        private readonly Stopwatch _stopwatch;

        private bool _isDisposed;

        public StepLogger(
            ILogger logger,
            LogLevel logLevel,
            string message,
            string callerMemberName,
            string callerFilePath,
            int callerLineNumber)
        {
            _logger = logger;
            _logLevel = logLevel;
            _message = message;

            _callerMemberName = callerMemberName;
            _callerFilePath = callerFilePath;
            _callerLineNumber = callerLineNumber;

            _steps = new List<IStep>();
            _stopwatch = Stopwatch.StartNew();
        }

        public void AddStep(IStep step)
        {
            _steps.Add(step);
        }

        public void Flush()
        {
            Dispose();
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

            var logEntry = Default.LogEntryFactory.CreateLogEntry();

            logEntry.LogLevel = _logLevel;
            logEntry.Message = _message ?? "Step Report";
            logEntry.ExtendedProperties.Add("Step Report", report.ToString());

            // ReSharper disable ExplicitCallerInfoArgument
            _logger.LogAsync(logEntry, _callerMemberName, _callerFilePath, _callerLineNumber);
            // ReSharper restore ExplicitCallerInfoArgument
        }
    }
}