namespace Rock.Framework.Serialization
{
    using Newtonsoft.Json;

    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        public string Serialize(object item)
        {
            return JsonConvert.SerializeObject(item);
        }
    }
}