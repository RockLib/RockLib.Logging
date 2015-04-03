namespace Rock.Logging.IntegrationTests
{
    public class SpecialContextProvider : IContextProvider
    {
        public void AddContextData(ILogEntry logEntry)
        {
            logEntry.ExtendedProperties.Add("Answer", "42");
        }
    }
}