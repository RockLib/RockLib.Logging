using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.Logging
{
    /// <summary>
    /// An implementation of <see cref="ILogProvider"/> that writes log entries to a file.
    /// </summary>
    public class FileLogProvider : ILogProvider, IDisposable
    {
        /// <summary>
        /// The default template.
        /// </summary>
        public const string DefaultTemplate = ConsoleLogProvider.DefaultTemplate;

        /// <summary>
        /// The default timeout.
        /// </summary>
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(3);

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogProvider"/> class.
        /// </summary>
        /// <param name="file">The file that logs will be written to.</param>
        /// <param name="template">The template used to format log entries.</param>
        /// <param name="level">The level of the log provider.</param>
        /// <param name="timeout">The timeout of the log provider.</param>
        public FileLogProvider(string file,
            string template = DefaultTemplate,
            LogLevel level = default(LogLevel),
            TimeSpan? timeout = null)
            : this(file, new TemplateLogFormatter(template ?? DefaultTemplate), level, timeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogProvider"/> class.
        /// </summary>
        /// <param name="file">The path to a writable file.</param>
        /// <param name="formatter">An object that formats log entries prior to writing to file.</param>
        /// <param name="level">The level of the log provider.</param>
        /// <param name="timeout">The timeout of the log provider.</param>
        public FileLogProvider(string file,
            ILogFormatter formatter,
            LogLevel level = default(LogLevel),
            TimeSpan? timeout = null)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
            Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            Level = level;
            Timeout = timeout ?? DefaultTimeout;

            var dir = Path.GetDirectoryName(File);

            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// Gets the file that logs will be written to.
        /// </summary>
        public string File { get; }

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
        /// Formats the log entry using the <see cref="Formatter"/> property and writes it the file
        /// specified by the <see cref="File"/> property.
        /// </summary>
        /// <param name="logEntry">The log entry to write.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
        /// <returns>A task that completes when the log entry has been written to file.</returns>
        public async Task WriteAsync(LogEntry logEntry, CancellationToken cancellationToken = default(CancellationToken))
        {
            var formattedLogEntry = Formatter.Format(logEntry);

            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                OnPreWrite(logEntry, formattedLogEntry);

                using (var writer = new StreamWriter(File, true))
                {
                    await writer.WriteLineAsync(formattedLogEntry).ConfigureAwait(false);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// A method called before log entries are written. This method is synchronized. That is,
        /// only one thread will execute this method at any given time.
        /// </summary>
        /// <remarks>The base method does nothing.</remarks>
        protected virtual void OnPreWrite(LogEntry logEntry, string formattedLogEntry)
        {
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="FileLogProvider"/> class.
        /// </summary>
        public virtual void Dispose()
        {
            _semaphore.Dispose();
        }
    }
}
