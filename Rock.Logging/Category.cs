using System.Xml.Serialization;
using Rock.Logging.Configuration;

namespace Rock.Logging
{
    public class Category
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("throttlingRule")]
        public string ThrottlingRule { get; set; }

        [XmlArray("providers")]
        [XmlArrayItem("provider")]
        public LogProviderProxy[] LogProviders { get; set; }
    }
}