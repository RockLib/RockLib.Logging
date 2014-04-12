using System;
using System.Threading.Tasks;

namespace Rock.Logging
{
    public abstract class LogProviderBase : ILogProvider
    {
        protected static readonly Task _completedTask = Task.FromResult(0);

        private readonly ILogFormatterFactory _factory;

        protected LogProviderBase(ILogFormatterFactory factory)
        {
            _factory = factory;
        }

        public async Task Write(LogEntry entry)
        {
            var formatter = _factory.GetInstance();
            await Write(formatter.Format(entry));
        }

        protected abstract Task Write(string formattedLogEntry);
    }

    public class ConsoleLogProvider : LogProviderBase
    {
        public ConsoleLogProvider(ILogFormatterFactory factory)
            : base(factory)
        {
        }

        protected override Task Write(string formattedLogEntry)
        {
            Console.WriteLine(formattedLogEntry);
            return _completedTask;
        }
    }

    public interface ILogFormatterFactory
    {
        ILogFormatter GetInstance();
    }

    public class LogFormatterFactory : ILogFormatterFactory
    {
        private readonly ILogFormatterConfiguration _configuration;

        public LogFormatterFactory(ILogFormatterConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ILogFormatter GetInstance()
        {
            return new LogFormatter(_configuration.Template);
        }
    }

    public interface ILogFormatter
    {
        string Format(LogEntry entry);
    }
}