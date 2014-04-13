namespace Rock.Logging.Diagnostics
{
    public interface IStep
    {
        IStepSnapshot GetSnapshot();
    }
}