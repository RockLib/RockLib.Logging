using System.Xml.Serialization;
using Rock.Configuration;

namespace Rock.Logging.Configuration
{
    [XmlRoot("throttlingRule")]
    public class ThrottlingRuleEvaluatorFactory : XmlDeserializingFactory<IThrottlingRuleEvaluator>
    {
        public ThrottlingRuleEvaluatorFactory()
            : base(typeof(ThrottlingRuleEvaluator))
        {
        }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}