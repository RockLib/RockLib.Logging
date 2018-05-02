using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RockLib.Logging
{
    public sealed class Logger : ILogger
    {
        public Logger(bool isLoggingEnabled, LogLevel loggingLevel,
            IReadOnlyCollection<ILogProvider> logProviders)
        {
            IsLoggingEnabled = isLoggingEnabled;
            LoggingLevel = loggingLevel;
            LogProviders = logProviders ?? new ILogProvider[0];
        }

        public bool IsLoggingEnabled { get; }
        public LogLevel LoggingLevel { get; }
        public IReadOnlyCollection<ILogProvider> LogProviders { get; }

        public void Log(LogEntry logEntry,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (!IsLoggingEnabled || logEntry.Level < LoggingLevel)
                return;

            var writeTasks = new List<Task>(LogProviders.Count);

            foreach (var logProvider in LogProviders)
                writeTasks.Add(WriteToLogProvider(logEntry, logProvider));

            //return Task.WhenAll(writeTasks);
        }

        private async Task WriteToLogProvider(LogEntry logEntry, ILogProvider logProvider)
        {
            if (logEntry.Level < logProvider.LoggingLevel)
                return;

            try
            {
                await logProvider.WriteAsync(logEntry).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // TODO: Write to trace
                // TODO: execute retry policy
            }
        }
    }
}
