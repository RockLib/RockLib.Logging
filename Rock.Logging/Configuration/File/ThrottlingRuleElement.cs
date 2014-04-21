using System;
using System.ComponentModel;
using System.Configuration;

namespace Rock.Logging.Configuration
{
    public class ThrottlingRuleElement : ConfigurationElement
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

        [ConfigurationProperty("minInterval", DefaultValue = "00:00:00"), TypeConverter(typeof(InfiniteTimeSpanConverter))]
        public TimeSpan MinInterval
        {
            get
            {
                return (TimeSpan)this["minInterval"];
            }
            set
            {
                this["minInterval"] = value;
            }
        }

        [ConfigurationProperty("minEventThreshold", DefaultValue = 1)]
        public int MinEventThreshold
        {
            get
            {
                return (int)this["minEventThreshold"];
            }
            set
            {
                this["minEventThreshold"] = value;
            }
        }
    }
}