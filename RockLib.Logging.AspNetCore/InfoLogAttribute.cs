namespace RockLib.Logging.AspNetCore
{
    /// <summary>
    /// An action filter that records an info log each time the action is executed.
    /// </summary>
    public class InfoLogAttribute : LoggingActionFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoLogAttribute"/> class.
        /// </summary>
        /// <param name="messageFormat">
        /// The message format string. The action name is used as the <c>{0}</c> placeholder when
        /// formatting the message.
        /// </param>
        /// <param name="loggerName">The name of the logger.</param>
        /// <param name="exceptionMessageFormat">
        /// The message format string to use when a request has an uncaught exception. The action
        /// name is used as the <c>{0}</c> placeholder when formatting the message.
        /// </param>
        /// <param name="exceptionLogLevel">
        /// The level to log at when a request has an uncaught exception.
        /// </param>
        public InfoLogAttribute(string messageFormat = DefaultMessageFormat, string loggerName = Logger.DefaultName,
            string exceptionMessageFormat = DefaultExceptionMessageFormat, LogLevel exceptionLogLevel = DefaultExceptionLogLevel)
            : base(messageFormat, loggerName, LogLevel.Info, exceptionMessageFormat, exceptionLogLevel)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoLogAttribute"/> class.
        /// </summary>
        /// <param name="messageFormat">
        /// The message format string. The action name is used as the <c>{0}</c> placeholder when
        /// formatting the message.
        /// </param>
        /// <param name="loggerName">The name of the logger.</param>
        public InfoLogAttribute(string messageFormat, string loggerName)
            : base(messageFormat, loggerName, LogLevel.Info)
        {
        }
    }
}
