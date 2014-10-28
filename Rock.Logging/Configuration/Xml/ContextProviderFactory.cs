using Rock.Configuration;

namespace Rock.Logging.Configuration
{
    public class ContextProviderFactory : XmlDeserializationProxy<IContextProvider>
    {
        public ContextProviderFactory()
            : base(null)
        {
        }
    }
}