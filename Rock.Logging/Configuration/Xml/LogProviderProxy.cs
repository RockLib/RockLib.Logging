using System;
using System.Xml.Serialization;
using Rock.Collections;
using Rock.DependencyInjection;
using Rock.Serialization;

namespace Rock.Logging.Configuration
{
    public class LogProviderProxy : XmlDeserializationProxy<ILogProvider>
    {
        [XmlAttribute("formatter")]
        public string Formatter { get; set; }

        public ILogProvider CreateInstance(IKeyedEnumerable<string, LogFormatterProxy> formatterFactories, IResolver resolver)
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