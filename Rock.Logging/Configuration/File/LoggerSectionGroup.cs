using System.Configuration;

namespace Rock.Logging.Configuration
{
    public class LoggerSectionGroup : ConfigurationElement
    {
        [ConfigurationProperty("loggerSettings", IsRequired = false)]
        public LoggerSettingsSection LoggerSettings
        {
            get
            {
                return (LoggerSettingsSection)this["loggerSettings"];
            }
            set
            {
                this["loggerSettings"] = value;
            }
        }
    }
}