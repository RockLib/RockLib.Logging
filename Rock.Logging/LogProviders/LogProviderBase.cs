using System.Threading.Tasks;

namespace Rock.Logging
{
    public abstract class LogProviderBase : ILogProvider
    {
        protected static readonly Task _completedTask = Task.FromResult(0);

        private readonly ILogFormatterFactory _logFormatterFactory;

        protected LogProviderBase(ILogFormatterFactory logFormatterFactory)
        {
            _logFormatterFactory = logFormatterFactory;
        }

        public async Task Write(LogEntry entry)
        {
            var formatter = _logFormatterFactory.GetInstance();
            await Write(formatter.Format(entry));
        }

        protected abstract Task Write(string formattedLogEntry);
    }
}