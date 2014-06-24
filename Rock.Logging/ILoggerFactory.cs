namespace Rock.Logging
{
    public interface ILoggerFactory
    {
        TLogger Get<TLogger>(string category = null)
            where TLogger : ILogger;
    }
}