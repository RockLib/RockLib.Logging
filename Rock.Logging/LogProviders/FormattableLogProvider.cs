using System;
using System.Threading.Tasks;
using Rock.Immutable;

namespace Rock.Logging
{
    public abstract class FormattableLogProvider : ILogProvider
    {
        /// <summary>
        /// A completed task. Can be returned from methods that return a <see cref="Task"/>
        /// but are not really asynchronous.
        /// </summary>
        protected static readonly Task _completedTask = Task.FromResult(0);

        private readonly ILogFormatter _logFormatter;
        private readonly Semimutable<LogLevel> _loggingLevel;

        protected FormattableLogProvider(ILogFormatter logFormatter)
        {
            if (logFormatter == null)
            {
                throw new ArgumentNullException("logFormatter");
            }

            _logFormatter = logFormatter;
            _loggingLevel = new Semimutable<LogLevel>(LogLevel.NotSet);
        }

        public LogLevel LoggingLevel
        {
            get { return _loggingLevel.Value; }
            set { _loggingLevel.Value = value; }
        }

        public async Task WriteAsync(LogEntry entry)
        {
            await WriteAsync(entry, _logFormatter.Format(entry));
        }

        protected abstract Task WriteAsync(LogEntry entry, string formattedLogEntry);
    }
}