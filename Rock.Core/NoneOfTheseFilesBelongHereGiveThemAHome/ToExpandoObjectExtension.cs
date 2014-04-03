namespace Rock.Framework
{
    using System.Dynamic;

    public static class ToExpandoObjectExtension
    {
        private static IConvertObjectsTo<ExpandoObject> _converter = Default.ObjectToExpandoObjectConverter;

        public static ExpandoObject ToExpandoObject(this object @object)
        {
            return _converter.Convert(@object);
        }

        public static IConvertObjectsTo<ExpandoObject> ObjectToExpandoObjectConverter
        {
            get { return _converter; }
            set { _converter = value ?? Default.ObjectToExpandoObjectConverter; }
        }
    }
}
