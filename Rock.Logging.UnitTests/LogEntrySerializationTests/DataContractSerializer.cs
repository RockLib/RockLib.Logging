using Rock.Serialization;

namespace LoggerTests.LogEntrySerializationTests
{
    public class DataContractSerializer : LogEntrySerializationTestBase
    {
        protected override ISerializer GetSerializer()
        {
            return new DataContractSerializerSerializer();
        }
    }
}