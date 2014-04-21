using System;
using System.Reflection;

namespace Rock.Logging
{
    public class Mapper : IMapper
    {
        private readonly PropertyInfo _property;
        private readonly object _value;

        public Mapper(PropertyInfo property, string value)
        {
            _property = property;
            _value = Convert.ChangeType(value, property.PropertyType);
        }

        public void SetValue(object instance)
        {
            _property.SetValue(instance, _value);
        }
    }
}