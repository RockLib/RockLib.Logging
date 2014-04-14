namespace Rock.Logging
{
    public class LogFormatterFactory : ILogFormatterFactory
    {
        private readonly ILogFormatterConfiguration _configuration;

        public LogFormatterFactory(ILogFormatterConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ILogFormatter GetInstance()
        {
            return new LogFormatter(_configuration.Template);
        }
    }
}