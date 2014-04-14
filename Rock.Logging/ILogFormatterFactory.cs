namespace Rock.Logging
{
    public interface ILogFormatterFactory
    {
        ILogFormatter GetInstance();
    }
}