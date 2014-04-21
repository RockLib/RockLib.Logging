using System.Configuration;

namespace Rock.Logging.Configuration
{
    public class CategoryElement : ConfigurationElement
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

        [ConfigurationProperty("providers", IsDefaultCollection = true)]
        public ProviderElementCollection Providers
        {
            get
            {
                return (ProviderElementCollection)this["providers"]; // questionable
            }
            set
            {
                this["providers"] = value;
            }
        }

        [ConfigurationProperty("throttlingRule")]
        public string ThrottlingRule
        {
            get
            {
                return (string)this["throttlingRule"];
            }
            set
            {
                this["throttlingRule"] = value;
            }
        }
    }

     [ConfigurationCollection(typeof(ProviderElement), AddItemName = "provider")]
    public class ProviderElementCollection : ConfigurationElementCollection
    {
        public ProviderElement this[int index]
        {
            get
            {
                return (ProviderElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public new ProviderElement this[string key]
        {
            get
            {
                return (ProviderElement)BaseGet(key);
            }
            set
            {
                if (BaseGet(key) != null)
                {
                    BaseRemove(key);
                }
                BaseAdd(value, true);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ProviderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ProviderElement)element).ProviderType;
        }
    }
}