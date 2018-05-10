using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.Logging
{
    public class FileLogProvider : ILogProvider, IDisposable
    {
        public const string DefaultTemplate = ConsoleLogProvider.DefaultTemplate;

        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(3);

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        public FileLogProvider(string file,
            string template = DefaultTemplate,
            LogLevel level = default(LogLevel),
            TimeSpan? timeout = null)
            : this(file, new TemplateLogFormatter(template ?? DefaultTemplate), level, timeout)
        {
        }

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

        public string File { get; }

        public ILogFormatter Formatter { get; }

        public LogLevel Level { get; }

        public TimeSpan Timeout { get; }

        public async Task WriteAsync(LogEntry logEntry, CancellationToken cancellationToken = default(CancellationToken))
        {
            var formattedLogEntry = Formatter.Format(logEntry);

            await _semaphore.WaitAsync().ConfigureAwait(false);

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
        /// A method called before the log entry is written. This method is thread-safe. That is,
        /// only one thread will execute this method at any given time.
        /// </summary>
        protected virtual void OnPreWrite(LogEntry entry, string formattedLogEntry)
        {
        }

        public virtual void Dispose()
        {
            _semaphore.Dispose();
        }
    }
}
