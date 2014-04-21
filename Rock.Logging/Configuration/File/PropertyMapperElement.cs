using System.Configuration;

namespace Rock.Logging.Configuration
{
    public class PropertyMapperElement : ConfigurationElement
    {
        [ConfigurationProperty("property", IsRequired = false, IsKey = true)]
        public string Property
        {
            get
            {
                return (string)this["property"];
            }
            set
            {
                this["property"] = value;
            }
        }


        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get
            {
                return (string)this["value"];
            }
            set
            {
                this["value"] = value;
            }
        }
    }
}