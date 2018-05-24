using System;
using Moq;
using NUnit.Framework;
using Rock.Logging;
using Rock.Serialization;

// ReSharper disable once CheckNamespace
namespace SerializingLogFormatterTests
{
    public class TheFormatMethod
    {
        [Test]
        public void ReturnsWhatItsSerializerReturns()
        {
            var mockSerializer = GetMockSerializer();

            var logFormatter = new SerializingLogFormatter(mockSerializer.Object);

            AssertSerializingLogFormatter(logFormatter, mockSerializer);
        }

        internal static Mock<ISerializer> GetMockSerializer()
        {
            var mockSerializer = new Mock<ISerializer>();

            mockSerializer
                .Setup(m => m.SerializeToString(It.IsAny<object>(), It.IsAny<Type>()))
                .Returns("Hello, world!");
            
            return mockSerializer;
        }

        internal static void AssertSerializingLogFormatter(ILogFormatter logFormatter, Mock<ISerializer> mockSerializer)
        {
            Assert.That(logFormatter.Format(null), Is.EqualTo("Hello, world!"));
            mockSerializer.Verify(m => m.SerializeToString(It.IsAny<object>(), It.IsAny<Type>()), Times.Once());
        }
    }
}
