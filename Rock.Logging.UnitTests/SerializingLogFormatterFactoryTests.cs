using NUnit.Framework;
using Rock.Logging;

// ReSharper disable once CheckNamespace
namespace SerializingLogFormatterFactoryTests
{
    public class TheGetInstanceMethod
    {
        [Test]
        public void ReturnsASerializingLogFormatterInitializedWithTheSerializerFromTheFactory()
        {
            var mockSerializer = SerializingLogFormatterTests.TheFormatMethod.GetMockSerializer();

            var logFormatterFactory = new SerializingLogFormatterFactory(mockSerializer.Object);

            var logFormatter = logFormatterFactory.GetInstance();

            SerializingLogFormatterTests.TheFormatMethod.AssertSerializingLogFormatter(logFormatter, mockSerializer);
        }
    }
}
