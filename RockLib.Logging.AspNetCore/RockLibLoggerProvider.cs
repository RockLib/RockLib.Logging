namespace RockLib.Logging.AspNetCore
{
    internal class RockLibLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider
    {
        private readonly string _rockLibLoggerName;

        public RockLibLoggerProvider(string rockLibLoggerName = null)
        {
            _rockLibLoggerName = rockLibLoggerName;
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            var logger = LoggerFactory.GetInstance(_rockLibLoggerName);

            return new RockLibLogger(logger, categoryName);
        }

        public void Dispose()
        {
        }
    }
}
