namespace Rock.Framework.Serialization
{
    public static class ToJsonExtension
    {
        private static IJsonSerializer _serializer = Default.JsonSerializer;

        public static string ToJson(this object item)
        {
            return _serializer.Serialize(item);
        }

        public static IJsonSerializer JsonSerializer
        {
            get { return _serializer; }
            set { _serializer = value ?? Default.JsonSerializer; }
        }
    }
}
