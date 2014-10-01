using Rock.Serialization;

namespace LoggerTests.LogEntrySerializationTests
{
    public class XmlSerializer : LogEntrySerializationTestBase
    {
        protected override ISerializer GetSerializer()
        {
            return new XmlSerializerSerializer();
        }
    }
}