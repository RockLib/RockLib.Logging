namespace RockLib.Logging;

/// <summary>
/// Defines various logging levels.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// The logging level has not been set.
    /// </summary>
    NotSet,

    /// <summary>
    /// The Debug logging level.
    /// </summary>
    Debug,

    /// <summary>
    /// The Info logging level.
    /// </summary>
    Info,

    /// <summary>
    /// The Warn logging level.
    /// </summary>
    Warn,

    /// <summary>
    /// The Error logging level.
    /// </summary>
    Error,

    /// <summary>
    /// The Fatal logging level.
    /// </summary>
    Fatal,

    /// <summary>
    /// The Audit logging level.
    /// </summary>
    Audit
}