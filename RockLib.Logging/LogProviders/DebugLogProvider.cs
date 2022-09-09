#define DEBUG
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.Logging;

/// <summary>
/// An implementation of <see cref="ILogProvider"/> that writes log entries to debug.
/// </summary>
public class DebugLogProvider : ILogProvider
{
    /// <summary>
    /// The default template.
    /// </summary>
    public const string DefaultTemplate = ConsoleLogProvider.DefaultTemplate;

    /// <summary>
    /// The default timeout.
    /// </summary>
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugLogProvider"/> class.
    /// </summary>
    /// <param name="template">The template used to format log entries.</param>
    /// <param name="level">The level of the log provider.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    public DebugLogProvider(
        string template = DefaultTemplate, LogLevel level = default, TimeSpan? timeout = null)
        : this(new TemplateLogFormatter(template ?? DefaultTemplate), level, timeout)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugLogProvider"/> class.
    /// </summary>
    /// <param name="formatter">An object that formats log entries prior to writing to standard out.</param>
    /// <param name="level">The level of the log provider.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    public DebugLogProvider(
        ILogFormatter formatter, LogLevel level = default, TimeSpan? timeout = null)
    {
        if (!Enum.IsDefined(typeof(LogLevel), level))
        {
            throw new ArgumentException($"Log level is not defined: {level}.", nameof(level));
        }

        if (timeout.HasValue && timeout.Value < TimeSpan.Zero)
        {
            throw new ArgumentException("Timeout cannot be negative.", nameof(timeout));
        }

        Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        Level = level;
        Timeout = timeout ?? DefaultTimeout;
    }

    /// <summary>
    /// Gets an object that formats log entries.
    /// </summary>
    public ILogFormatter Formatter { get; }

    /// <summary>
    /// Gets the log level.
    /// </summary>
    public LogLevel Level { get; }

    /// <summary>
    /// Gets the timeout.
    /// </summary>
    public TimeSpan Timeout { get; }

    /// <summary>
    /// Formats the log entry using the <see cref="Formatter"/> property and writes it to standard out.
    /// </summary>
    /// <param name="logEntry">The log entry to write.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <returns>A task that completes when the log entry has been written to standard out.</returns>
    public Task WriteAsync(LogEntry logEntry, CancellationToken cancellationToken = default)
    {
        var formattedLog = Formatter.Format(logEntry);
        WriteToDebug(formattedLog);
        return Task.CompletedTask;
    }

    protected virtual void WriteToDebug(string formattedLog) => Debug.WriteLine(formattedLog);
}
