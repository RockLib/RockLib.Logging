using System;
using System.Diagnostics;

namespace RockLib.Logging.Diagnostics;

using static Logger;

/// <summary>
/// A trace listener that records trace messages with an <see cref="ILogger"/>.
/// </summary>
public class LoggingTraceListener : TraceListener
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingTraceListener"/> class.
    /// </summary>
    /// <param name="logger">The logger that will record trace messages.</param>
    /// <param name="logLevel">The level that trace messages will be logged at.</param>
    public LoggingTraceListener(ILogger logger, LogLevel? logLevel = null)
        : base(logger?.Name ?? DefaultName)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        LogLevel = logLevel ?? logger.Level;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingTraceListener"/> class.
    /// </summary>
    /// <param name="loggerName">
    /// The name of the logger to create, using <see cref="LoggerFactory"/>, that will record
    /// trace messages.
    /// </param>
    /// <param name="logLevel">The level that trace messages will be logged at.</param>
    public LoggingTraceListener(string loggerName = DefaultName, LogLevel? logLevel = null)
        : this(LoggerFactory.Create(loggerName ?? DefaultName), logLevel)
    {
    }

    /// <summary>
    /// The logger that records trace messages.
    /// </summary>
    public ILogger Logger { get; }

    /// <summary>
    /// The level that trace messages are logged at.
    /// </summary>
    public LogLevel LogLevel { get; }

    /// <summary>
    /// Writes the specified message to <see cref="Logger"/>.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public override void Write(string message) => WriteLog(message);

    /// <summary>
    /// Writes the specified message to <see cref="Logger"/>.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public override void WriteLine(string message) => WriteLog(message);

    private void WriteLog(string message)
    {
        if (Logger.IsEnabled(LogLevel))
            Logger.Log(new LogEntry(message, LogLevel));
    }
}
