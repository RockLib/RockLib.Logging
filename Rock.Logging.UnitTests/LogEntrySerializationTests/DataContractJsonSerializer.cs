using Rock.Serialization;

namespace LoggerTests.LogEntrySerializationTests
{
    public class DataContractJsonSerializer : LogEntrySerializationTestBase
    {
        protected override ISerializer GetSerializer()
        {
            return new DataContractJsonSerializerSerializer();
        }
    }
}