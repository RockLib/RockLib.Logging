using System.Xml.Serialization;
using Rock.Serialization;

namespace Rock.Logging.Configuration
{
    [XmlRoot("throttlingRule")]
    public class ThrottlingRuleEvaluatorProxy : XmlDeserializationProxy<IThrottlingRuleEvaluator>
    {
        public ThrottlingRuleEvaluatorProxy()
            : base(typeof(ThrottlingRuleEvaluator))
        {
        }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}