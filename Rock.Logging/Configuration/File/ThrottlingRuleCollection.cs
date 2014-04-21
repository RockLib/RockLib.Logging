using System.Configuration;

namespace Rock.Logging.Configuration
{
    [ConfigurationCollection(typeof(ThrottlingRuleElement), AddItemName = "throttlingRule")]
    public class ThrottlingRuleCollection : ConfigurationElementCollection
    {
        public ThrottlingRuleElement this[int index]
        {
            get
            {
                return (ThrottlingRuleElement)BaseGet(index);
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

        public new ThrottlingRuleElement this[string key]
        {
            get
            {
                return (ThrottlingRuleElement)BaseGet(key);
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
            return new ThrottlingRuleElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ThrottlingRuleElement)element).Name;
        }
    }
}