using System.Threading.Tasks;

namespace Rock.Logging
{
    public abstract class FormattableLogProvider : ILogProvider
    {
        /// <summary>
        /// A completed task. Can be returned from the <see cref="WriteAsync"/> method
        /// if its implementation is not actually asynchronous.
        /// </summary>
        protected static readonly Task CompletedTask = Task.FromResult(0);

        private readonly ILogFormatter _logFormatter;

        protected FormattableLogProvider(ILogFormatterFactory logFormatterFactory)
        {
            _logFormatter = logFormatterFactory.GetInstance();
        }

        public async Task WriteAsync(LogEntry entry)
        {
            await WriteAsync(entry, _logFormatter.Format(entry));
        }

        protected abstract Task WriteAsync(LogEntry entry, string formattedLogEntry);
    }
}