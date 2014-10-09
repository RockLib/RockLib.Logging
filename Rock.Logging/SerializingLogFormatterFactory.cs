using Rock.Serialization;

namespace Rock.Logging
{
    public class SerializingLogFormatterFactory : ILogFormatterFactory
    {
        private readonly ISerializer _serializer;

        public SerializingLogFormatterFactory(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public ILogFormatter GetInstance()
        {
            return new SerializingLogFormatter(_serializer);
        }
    }
}