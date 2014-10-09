using Rock.Serialization;

namespace Rock.Logging
{
    public class SerializingLogFormatter : ILogFormatter
    {
        private readonly ISerializer _serializer;

        public SerializingLogFormatter(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public string Format(LogEntry entry)
        {
            return _serializer.SerializeToString(entry);
        }
    }
}