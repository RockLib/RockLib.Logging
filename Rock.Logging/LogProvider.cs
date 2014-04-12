namespace Rock.Logging
{
    public abstract class LogProvider
    {
        public const string DefaultTemplate = 
            @"ApplicationID: {applicationId}
Date: {createTime}
Message: {message}
Level: {level}
Extended Properties: {extendedProperties({key} {value})}Exception: {exception}

";

        public static readonly ILogFormatterConfiguration DefaultLogFormatterConfiguration = new LogFormatterConfiguration();

        private readonly LogLevel _loggingLevel;
        private readonly string _template;

        protected LogProvider(LogLevel loggingLevel, string template)
        {
            _loggingLevel = loggingLevel;
            _template = template;
        }

        private class LogFormatterConfiguration : ILogFormatterConfiguration
        {
            public string Name
            {
                get { return null; }
            }

            public string Template
            {
                get { return DefaultTemplate; }
            }
        }
    }
}