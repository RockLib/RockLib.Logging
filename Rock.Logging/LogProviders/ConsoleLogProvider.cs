using System;
using System.Threading.Tasks;
using Rock.Immutable;

namespace Rock.Logging
{
    public class ConsoleLogProvider : FormattableLogProvider
    {
        private static readonly Semimutable<ILogFormatter> _defaultLogFormatter = new Semimutable<ILogFormatter>(GetDefaultDefaultLogFormatter);

        public ConsoleLogProvider()
            : this(null)
        {
        }

        public ConsoleLogProvider(ILogFormatter logFormatter = null)
            : base(logFormatter ?? DefaultLogFormatter)
        {
        }

        protected override Task WriteAsync(ILogEntry entry, string formattedLogEntry)
        {
            Console.WriteLine(formattedLogEntry);
            return _completedTask;
        }

        public static ILogFormatter DefaultLogFormatter
        {
            get { return _defaultLogFormatter.Value; }
        }

        public static void SetDefaultLogFormatter(ILogFormatter logFormatter)
        {
            _defaultLogFormatter.Value = logFormatter;
        }

        private static ILogFormatter GetDefaultDefaultLogFormatter()
        {
            return new TemplateLogFormatter(FileLogProvider.DefaultTemplate);
        }
    }
}