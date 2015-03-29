using System;
using System.Threading.Tasks;

namespace Rock.Logging
{
    public class ConsoleLogProvider : FormattableLogProvider
    {
        public ConsoleLogProvider()
            : this(null)
        {
        }

        public ConsoleLogProvider(ILogFormatter logFormatter = null)
            : base(logFormatter ?? DefaultFileLogFormatter.Current)
        {
        }

        protected override Task WriteAsync(LogEntry entry, string formattedLogEntry)
        {
            Console.WriteLine(formattedLogEntry);
            return _completedTask;
        }
    }
}