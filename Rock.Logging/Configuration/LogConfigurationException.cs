using System;

namespace Rock.Logging.Configuration
{
    public class LogConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LogConfigurationException(string message)
            : base(message)
        {
        }
    }
}