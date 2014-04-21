using System.Configuration;

namespace Rock.Logging.Configuration
{
    [ConfigurationCollection(typeof(FormatterElement), AddItemName = "formatter")]
    public class FormatterCollection : ConfigurationElementCollection
    {
        public FormatterElement this[int index]
        {
            get
            {
                return (FormatterElement)BaseGet(index);
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

        public new FormatterElement this[string key]
        {
            get
            {
                return (FormatterElement)BaseGet(key);
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
            return new FormatterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FormatterElement)element).Name;
        }
    }
}