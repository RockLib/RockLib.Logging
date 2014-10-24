using Rock.Configuration;

namespace Rock.Logging.Configuration
{
    public class ContextProviderFactory : XmlDeserializingFactory<IContextProvider>
    {
        public ContextProviderFactory()
            : base(null)
        {
        }
    }
}