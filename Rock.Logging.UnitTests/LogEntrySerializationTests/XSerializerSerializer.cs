using Rock.Serialization;

namespace LoggerTests.LogEntrySerializationTests
{
    public class XSerializerSerializer : LogEntrySerializationTestBase
    {
        protected override ISerializer GetSerializer()
        {
            return new Rock.Serialization.XSerializerSerializer(new XSerializerSerializerConfiguration { Indent = true });
        }
    }
}