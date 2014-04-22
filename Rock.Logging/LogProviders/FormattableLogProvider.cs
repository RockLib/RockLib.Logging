using System.Threading.Tasks;

namespace Rock.Logging
{
    public abstract class FormattableLogProvider : ILogProvider
    {
        /// <summary>
        /// A completed task. Can be returned from <see cref="Write"/> method if
        /// its implementation is not actually asynchronous.
        /// </summary>
        protected static readonly Task CompletedTask = Task.FromResult(0);

        private readonly ILogFormatterFactory _logFormatterFactory;

        protected FormattableLogProvider(ILogFormatterFactory logFormatterFactory)
        {
            _logFormatterFactory = logFormatterFactory;
        }

        public async Task Write(LogEntry entry)
        {
            var formatter = _logFormatterFactory.GetInstance();
            await Write(entry, formatter.Format(entry));
        }

        protected abstract Task Write(LogEntry entry, string formattedLogEntry);
    }
}