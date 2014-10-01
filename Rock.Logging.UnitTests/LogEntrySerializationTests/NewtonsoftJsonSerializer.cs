using Rock.Serialization;

namespace LoggerTests.LogEntrySerializationTests
{
    public class NewtonsoftJsonSerializer : LogEntrySerializationTestBase
    {
        protected override ISerializer GetSerializer()
        {
            return new Rock.Serialization.NewtonsoftJsonSerializer();
        }
    }
}