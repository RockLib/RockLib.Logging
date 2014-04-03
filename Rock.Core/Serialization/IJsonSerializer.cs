namespace Rock.Framework.Serialization
{
    public interface IJsonSerializer
    {
        string Serialize(object item);
    }
}