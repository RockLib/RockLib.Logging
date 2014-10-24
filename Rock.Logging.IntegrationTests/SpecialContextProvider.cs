namespace Rock.Logging.IntegrationTests
{
    public class SpecialContextProvider : IContextProvider
    {
        public void AddContextData(LogEntry logEntry)
        {
            logEntry.ExtendedProperties.Add("Answer", "42");
        }
    }
}