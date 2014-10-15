using System.Threading.Tasks;
using Rock.Logging.Defaults.Implementation;

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

        protected FormattableLogProvider(ILogFormatterFactory logFormatterFactory)
        {
            _logFormatter = (logFormatterFactory ?? Default.LogFormatterFactory).GetInstance();
        }

        public async Task WriteAsync(LogEntry entry)
        {
            await WriteAsync(entry, _logFormatter.Format(entry));
        }

        protected abstract Task WriteAsync(LogEntry entry, string formattedLogEntry);
    }
}