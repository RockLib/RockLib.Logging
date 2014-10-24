using System;
using System.Threading.Tasks;
using Rock.Logging.Defaults.Implementation;

namespace Rock.Logging
{
    public class ConsoleLogProvider : FormattableLogProvider
    {
        public ConsoleLogProvider()
            : this(null)
        {
        }

        public ConsoleLogProvider(ILogFormatter logFormatter = null)
            : base(logFormatter ?? Default.FileLogFormatter)
        {
        }

        protected override Task WriteAsync(LogEntry entry, string formattedLogEntry)
        {
            Console.WriteLine(formattedLogEntry);
            return _completedTask;
        }
    }
}