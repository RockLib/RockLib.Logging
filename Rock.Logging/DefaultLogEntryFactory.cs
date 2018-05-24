using Rock.Immutable;
using Rock.Logging.Defaults;

namespace Rock.Logging
{
    public static class DefaultLogEntryFactory
    {
        private static readonly Semimutable<ILogEntryFactory> _logEntryFactory = new Semimutable<ILogEntryFactory>(GetDefault);

        public static ILogEntryFactory Current
        {
            get { return _logEntryFactory.Value; }
        }

        public static void SetCurrent(ILogEntryFactory logEntryFactory)
        {
            _logEntryFactory.Value = logEntryFactory;
        }

        private static ILogEntryFactory GetDefault()
        {
            return new LogEntryFactory();
        }
    }
}