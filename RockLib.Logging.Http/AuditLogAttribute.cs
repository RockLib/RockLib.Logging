namespace RockLib.Logging.Http
{
    /// <summary>
    /// An action filter that records an audit log each time the action is executed.
    /// </summary>
    public class AuditLogAttribute : LoggingActionFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogAttribute"/> class.
        /// </summary>
        /// <param name="messageFormat">
        /// The message format string. The action name is used as the <c>{0}</c> placeholder when
        /// formatting the message.
        /// </param>
        /// <param name="loggerName">The name of the logger.</param>
        public AuditLogAttribute(string messageFormat = DefaultMessageFormat, string loggerName = Logger.DefaultName)
            : base(messageFormat, loggerName, LogLevel.Audit)
        {
        }
    }
}
