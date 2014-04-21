using System.Configuration;

namespace Rock.Logging.Configuration
{
    [ConfigurationCollection(typeof(CategoryElement), AddItemName = "category")]
    public class CategoryElementCollection : ConfigurationElementCollection
    {
        public CategoryElement this[int index]
        {
            get
            {
                return (CategoryElement)BaseGet(index);
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

        public new CategoryElement this[string key]
        {
            get
            {
                return (CategoryElement)BaseGet(key);
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
            return new CategoryElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CategoryElement)element).Name;
        }
    }
}