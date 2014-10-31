using System.Xml.Serialization;

namespace Rock.Logging.Configuration
{
    [XmlRoot("category")]
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