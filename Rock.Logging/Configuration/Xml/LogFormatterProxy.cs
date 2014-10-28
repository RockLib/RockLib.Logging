using System.Xml.Serialization;
using Rock.Configuration;

namespace Rock.Logging.Configuration
{
    [XmlRoot("formatter")]
    public class LogFormatterProxy : XmlDeserializationProxy<ILogFormatter>
    {
        public LogFormatterProxy()
            : base(typeof(TemplateLogFormatter))
        {
        }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}