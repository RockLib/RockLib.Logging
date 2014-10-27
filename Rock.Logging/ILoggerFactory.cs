namespace Rock.Logging
{
    public interface ILoggerFactory
    {
        TLogger Get<TLogger>(string categoryName = null)
            where TLogger : ILogger;
    }
}