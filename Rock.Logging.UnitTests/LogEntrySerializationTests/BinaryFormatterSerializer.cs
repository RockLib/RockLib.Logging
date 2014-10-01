using Rock.Serialization;

namespace LoggerTests.LogEntrySerializationTests
{
    public class BinaryFormatterSerializer : LogEntrySerializationTestBase
    {
        protected override ISerializer GetSerializer()
        {
            return new Rock.Serialization.BinaryFormatterSerializer();
        }
    }
}