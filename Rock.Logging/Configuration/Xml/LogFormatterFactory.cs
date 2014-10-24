using System.Xml.Serialization;
using Rock.Configuration;

namespace Rock.Logging.Configuration
{
    [XmlRoot("formatter")]
    public class LogFormatterFactory : XmlDeserializingFactory<ILogFormatter>
    {
        public LogFormatterFactory()
            : base(typeof(TemplateLogFormatter))
        {
        }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}