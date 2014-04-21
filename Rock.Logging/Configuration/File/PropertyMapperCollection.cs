using System.Configuration;

namespace Rock.Logging.Configuration
{
    [ConfigurationCollection(typeof(PropertyMapperElement), AddItemName = "mapper")]
    public class PropertyMapperCollection : ConfigurationElementCollection
    {
        public PropertyMapperElement this[int index]
        {
            get
            {
                return (PropertyMapperElement)BaseGet(index);
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

        public new PropertyMapperElement this[string key]
        {
            get
            {
                return (PropertyMapperElement)BaseGet(key);
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
            return new PropertyMapperElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PropertyMapperElement)element).Property;
        }
    }
}