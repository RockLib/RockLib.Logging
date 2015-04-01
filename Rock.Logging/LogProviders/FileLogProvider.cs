using System;
using System.IO;
using System.Threading.Tasks;
using Rock.Immutable;

namespace Rock.Logging
{
    public class FileLogProvider : FormattableLogProvider, IDisposable
    {
        internal const string DefaultTemplate = "----------------------------------------------------------------------------------------------------{newLine}LOG INFO{newLine}{newLine}Message: {message}{newLine}Create Time: {createTime(O)}{newLine}Type of Message: {level} {newLine}Environment: {environment}{newLine}Application ID: {applicationId} {newLine}Application User ID: {applicationUserId} {newLine}Machine Name: {machineName}{newLine}{newLine}EXTENDED PROPERTY INFO{newLine}{newLine}{extendedProperties({key}: {value})}{newLine}EXCEPTION INFO{newLine}{newLine}Exception Type: {exceptionType}{newLine}Exception Context: {exceptionContext}{newLine}{newLine}{exception}";

        private static readonly Semimutable<ILogFormatter> _defaultLogFormatter = new Semimutable<ILogFormatter>(GetDefaultDefaultLogFormatter);

        private static readonly string _defaultFile =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "RockFramework",
                ApplicationId.Current,
                "log.txt");

        protected readonly string _file;
        private readonly IAsyncWaitHandle _waitHandle;
        private readonly bool _wasWaitHandleProvided;

        public FileLogProvider()
            : this(null, null, null)
        {
        }

        public FileLogProvider(
            string file = null,
            ILogFormatter logFormatter = null,
            IAsyncWaitHandle waitHandle = null)
            : base(logFormatter ?? DefaultLogFormatter)
        {
            _file = file ?? _defaultFile;
            _waitHandle = waitHandle ?? new SemaphoreSlimAsyncWaitHandle();
            _wasWaitHandleProvided = waitHandle != null;

            var dir = Path.GetDirectoryName(_file);

            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        protected override async Task WriteAsync(LogEntry entry, string formattedLogEntry)
        {
            await _waitHandle.WaitAsync();

            try
            {
                await OnPreWriteAsync(entry, formattedLogEntry);

                using (var writer = new StreamWriter(_file, true))
                {
                    await writer.WriteLineAsync(formattedLogEntry);
                }

                await OnPostWriteAsync(entry, formattedLogEntry);
            }
            finally
            {
                _waitHandle.Release();
            }
        }

        /// <summary>
        /// A method called before the log entry is written. This method is thread-safe. That is,
        /// only one thread will execute this method at any given time.
        /// </summary>
        protected virtual Task OnPreWriteAsync(LogEntry entry, string formattedLogEntry)
        {
            return _completedTask;
        }

        /// <summary>
        /// A method called after the log entry is written. This method is thread-safe. That is,
        /// only one thread will execute this method at any given time.
        /// </summary>
        protected virtual Task OnPostWriteAsync(LogEntry entry, string formattedLogEntry)
        {
            return _completedTask;
        }

        public virtual void Dispose()
        {
            // Don't dispose a wait handle that was passed to the constructor - since we
            // didn't create it, we shouldn't dispose it.
            if (!_wasWaitHandleProvided)
            {
                _waitHandle.Dispose();
            }
        }

        public static ILogFormatter DefaultLogFormatter
        {
            get { return _defaultLogFormatter.Value; }
        }

        public static void SetDefaultLogFormatter(ILogFormatter logFormatter)
        {
            _defaultLogFormatter.Value = logFormatter;
        }

        private static ILogFormatter GetDefaultDefaultLogFormatter()
        {
            return new TemplateLogFormatter(DefaultTemplate);
        }
    }
}