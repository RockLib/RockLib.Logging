using System;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.Logging
{
    /// <summary>
    /// An implementation of <see cref="ILogProvider"/> that writes log entries to standard out.
    /// </summary>
    public class ConsoleLogProvider : ILogProvider
    {
        /// <summary>
        /// The default template.
        /// </summary>
        public const string DefaultTemplate = @"----------------------------------------------------------------------------------------------------{newLine}LOG INFO{newLine}{newLine}Message: {message}{newLine}Create Time: {createTime(O)}{newLine}Level: {level}{newLine}Log ID: {uniqueId}{newLine}User Name: {userName}{newLine}Machine Name: {machineName}{newLine}Machine IP Address: {machineIpAddress}{newLine}{newLine}EXTENDED PROPERTY INFO{newLine}{newLine}{extendedProperties({key}: {value})}{newLine}EXCEPTION INFO{newLine}{newLine}{exception}";

        /// <summary>
        /// The default timeout.
        /// </summary>
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogProvider"/> class.
        /// </summary>
        /// <param name="template">The template used to format log entries.</param>
        /// <param name="level">The level of the log provider.</param>
        /// <param name="timeout">The timeout of the log provider.</param>
        public ConsoleLogProvider(
            string template = DefaultTemplate, LogLevel level = default(LogLevel), TimeSpan? timeout = null)
            : this(new TemplateLogFormatter(template ?? DefaultTemplate), level, timeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogProvider"/> class.
        /// </summary>
        /// <param name="formatter">An object that formats log entries prior to writing to standard out.</param>
        /// <param name="level">The level of the log provider.</param>
        /// <param name="timeout">The timeout of the log provider.</param>
        public ConsoleLogProvider(
            ILogFormatter formatter, LogLevel level = default(LogLevel), TimeSpan? timeout = null)
        {
            if (!Enum.IsDefined(typeof(LogLevel), level))
                throw new ArgumentException($"Log level is not defined: {level}.", nameof(level));
            if (timeout.HasValue && timeout.Value < TimeSpan.Zero)
                throw new ArgumentException("Timeout cannot be negative.", nameof(timeout));

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
        public Task WriteAsync(LogEntry logEntry, CancellationToken cancellationToken = default(CancellationToken))
        {
            var formattedLog = Formatter.Format(logEntry);
            return Console.Out.WriteLineAsync(formattedLog);
        }
    }
}
