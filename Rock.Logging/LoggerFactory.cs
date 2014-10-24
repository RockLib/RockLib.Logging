using Rock.Logging.Defaults.Implementation;

namespace Rock.Logging
{
    public static class LoggerFactory
    {
        public static ILogger GetInstance(string category = null)
        {
            return Default.LoggerFactory.Get<Logger>(category);
        }

        public static TLogger GetInstance<TLogger>(string category = null)
            where TLogger : ILogger
        {
            return Default.LoggerFactory.Get<TLogger>(category);
        }
    }
}