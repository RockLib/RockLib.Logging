using System.Xml.Serialization;
using Rock.Collections;
using Rock.Configuration;
using Rock.DependencyInjection;

namespace Rock.Logging.Configuration
{
    public class LogProviderFactory : XmlDeserializingFactory<ILogProvider>
    {
        public LogProviderFactory()
            : base(null)
        {
        }

        [XmlAttribute("formatter")]
        public string Formatter { get; set; }

        public ILogProvider CreateInstance(IKeyedEnumerable<string, LogFormatterFactory> formatterFactories, IResolver resolver)
        {
            ILogFormatter logFormatter;
            if (Formatter != null && formatterFactories.Contains(Formatter))
            {
                logFormatter = formatterFactories[Formatter].CreateInstance(resolver);
            }
            else
            {
                logFormatter = null;
            }

            if (logFormatter != null)
            {
                resolver =
                    resolver == null
                        ? new AutoContainer(logFormatter)
                        : resolver.MergeWith(new AutoContainer(logFormatter));
            }

            return CreateInstance(resolver);
        }
    }
}