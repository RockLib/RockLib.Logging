using System.Configuration;

namespace Rock.Logging.Configuration
{
    public class FormatterElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("template")]
        public string Template
        {
            get
            {
                return (string)this["template"];
            }
            set
            {
                this["template"] = value;
            }
        }
    }
}