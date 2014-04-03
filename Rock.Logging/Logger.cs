using System.Collections.Generic;
using System.Linq;

namespace Rock.Logging
{
    public class Logger : ILogger
    {
        private readonly ILoggerConfiguration _configuration;
        private readonly IEnumerable<IContextProvider> _contextProviders;

        public Logger()
            : this(Default.LoggerConfiguration, Default.ContextProviders)
        {
        }

        public Logger(ILoggerConfiguration configuration, IEnumerable<IContextProvider> contextProviders)
        {
            _configuration = configuration;
            _contextProviders = contextProviders.ToList();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None && logLevel >= _configuration.LogLevel;
        }

        public void Log(LogEntry logEntry)
        {
            if (!IsEnabled(logEntry.LogLevel))
            {
                return;
            }

            foreach (var contextProvider in GetContextProviders())
            {
                contextProvider.AddContextData(logEntry);
            }

            // The logger should have a way to set its properties. A "property setter" object. One
            // that would have access to the application id, the machine name, user name, etc. It
            // would set the values on the LogEntry that was passed in to this method.

            //// ApplicationId
            //// CreateTime
            //// ExceptionData
            //// ExtendedProperties
            //// Level
            //// MachineIpAddress
            //// MachineName
            //// Message
            //// web-specific things
            ////  Referrer
            ////  RequestMethod
            ////  Url
            ////  UserAgentBrowser
            //// UserId <- breaking change
            //// UserIpAddress
            //// UserName
            //logEntry["ApplicationId"] = _configuration.ApplicationId;
            //logEntry["Message"] = message;
            //logEntry["ServerName"] = "blah";
        }

        protected virtual IEnumerable<IContextProvider> GetContextProviders()
        {
            return _contextProviders;
        }
    }
}