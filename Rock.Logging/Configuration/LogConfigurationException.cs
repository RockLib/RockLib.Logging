using System;

namespace Rock.Logging.Configuration
{
    public class LogConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogConfigurationException"/> class.
        /// </summary>
        public LogConfigurationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LogConfigurationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public LogConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}