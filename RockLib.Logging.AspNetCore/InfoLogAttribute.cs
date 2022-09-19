namespace RockLib.Logging.AspNetCore;

/// <summary>
/// An action filter that records an info log each time an action is executed.
/// </summary>
public sealed class InfoLogAttribute : LoggingActionFilterAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InfoLogAttribute"/> class.
    /// </summary>
    /// <param name="messageFormat">
    /// The message format string. The action name is used as the <c>{0}</c> placeholder when
    /// formatting the message.
    /// </param>
    /// <param name="loggerName">The name of the logger.</param>
    public InfoLogAttribute(string? messageFormat = DefaultMessageFormat, string? loggerName = Logger.DefaultName)
        : base(messageFormat, loggerName, LogLevel.Info)
    {
    }
}
