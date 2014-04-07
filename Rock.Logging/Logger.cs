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
            _contextProviders = GetContextProviders(contextProviders);
        }

        private IEnumerable<IContextProvider> GetContextProviders(IEnumerable<IContextProvider> contextProviders)
        {
            // This behavior might not be completely intuitive - it probably needs good documentation.
            // "Logger itself does not implement IContextProvider, but if a subclass of Logger does,
            // then this instance's .AddContextData method will be called when the other context
            // providers' .AddContextData methods are called."
            var contextProviderList = contextProviders.ToList();

            var thisContextProvider = this as IContextProvider;
            if (thisContextProvider != null)
            {
                contextProviderList.Insert(0, thisContextProvider);
            }

            return contextProviderList;
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

            foreach (var contextProvider in _contextProviders)
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
    }
}