namespace Rock.Framework
{
    using Serialization;
    using System.Dynamic;

    internal static class Default
    {
        public static readonly IJsonSerializer JsonSerializer = new NewtonsoftJsonSerializer();
        public static readonly IConvertObjectsTo<ExpandoObject> ObjectToExpandoObjectConverter = new ReflectinatorObjectToExpandoObjectConverter();
    }
}
