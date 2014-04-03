namespace Rock.Framework
{
    public interface IConvertObjectsTo<TTarget>
    {
        TTarget Convert(object @object);
    }
}