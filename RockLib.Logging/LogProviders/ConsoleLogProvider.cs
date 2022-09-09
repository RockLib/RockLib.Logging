using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.Logging;

/// <summary>
/// An implementation of <see cref="ILogProvider"/> that writes log entries to standard out.
/// </summary>
public class ConsoleLogProvider : ILogProvider
{
    /// <summary>
    /// Defines the types of output streams used by <see cref="ConsoleLogProvider"/>.
    /// </summary>
    public enum Output
    {
        /// <summary>
        /// The <see cref="ConsoleLogProvider"/> will write to the standard output stream.
        /// </summary>
        StdOut,

        /// <summary>
        /// The <see cref="ConsoleLogProvider"/> will write to the standard error stream.
        /// </summary>
        StdErr
    }

    /// <summary>
    /// The default template.
    /// </summary>
    public const string DefaultTemplate = @"----------------------------------------------------------------------------------------------------{newLine}LOG INFO{newLine}{newLine}Message: {message}{newLine}Create Time: {createTime(O)}{newLine}Level: {level}{newLine}Log ID: {uniqueId}{newLine}User Name: {userName}{newLine}Machine Name: {machineName}{newLine}Machine IP Address: {machineIpAddress}{newLine}Correlation ID: {correlationId}{newLine}Business Process ID: {businessProcessId}{newLine}Business Activity ID: {businessActivityId}{newLine}{newLine}EXTENDED PROPERTY INFO{newLine}{newLine}{extendedProperties({key}: {value})}{newLine}EXCEPTION INFO{newLine}{newLine}{exception}";

    /// <summary>
    /// The default timeout.
    /// </summary>
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(1);

    private readonly TextWriter _consoleWriter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleLogProvider"/> class.
    /// </summary>
    /// <param name="template">The template used to format log entries.</param>
    /// <param name="level">The level of the log provider.</param>
    /// <param name="output">The type of output stream to use.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    public ConsoleLogProvider(
        string template = DefaultTemplate, LogLevel level = default, Output output = default, TimeSpan? timeout = null)
        : this(new TemplateLogFormatter(template ?? DefaultTemplate), level, output, timeout)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleLogProvider"/> class.
    /// </summary>
    /// <param name="formatter">An object that formats log entries prior to writing to standard out.</param>
    /// <param name="level">The level of the log provider.</param>
    /// <param name="output">The type of output stream to use.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    public ConsoleLogProvider(
        ILogFormatter formatter, LogLevel level = default, Output output = default, TimeSpan? timeout = null)
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

        switch (output)
        {
            case Output.StdOut:
                _consoleWriter = Console.Out;
                break;
            case Output.StdErr:
                _consoleWriter = Console.Error;
                break;
            default:
                throw new ArgumentException($"Output stream is not defined: {output}.", nameof(output));
        }
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
        return _consoleWriter.WriteLineAsync(formattedLog);
    }
}
