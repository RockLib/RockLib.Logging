using System;
using System.IO;
using System.Threading.Tasks;
using Rock.Defaults.Implementation;
using Rock.Reflection;

namespace Rock.Logging
{
    public class FileLogProvider : FormattableLogProvider, IDisposable
    {
        private static readonly string _defaultFile =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "RockFramework",
                Default.ApplicationInfo.ApplicationId,
                "log.txt");

        private static readonly Type _defaultAsyncWaitHandleType = typeof(SemaphoreSlimAsyncWaitHandle);

        private string _filePropertyBackingField;
        private readonly Lazy<string> _file;
        private readonly Lazy<IAsyncWaitHandle> _waitHandle;
        private readonly bool _wasWaitHandleProvided;

        public FileLogProvider(ILogFormatterFactory logFormatterFactory)
            : base(logFormatterFactory)
        {
            File = _defaultFile;
            WaitHandleType = _defaultAsyncWaitHandleType.AssemblyQualifiedName;

            _file = new Lazy<string>(() => File);
            _waitHandle = new Lazy<IAsyncWaitHandle>(() => SlowFactory.CreateInstance<IAsyncWaitHandle>(WaitHandleType));

            _wasWaitHandleProvided = false;
        }

        public FileLogProvider(ILogFormatterFactory logFormatterFactory, string file, IAsyncWaitHandle waitHandle = null)
            : base(logFormatterFactory)
        {
            File = file;
            WaitHandleType =
                waitHandle != null
                    ? waitHandle.GetType().AssemblyQualifiedName
                    : _defaultAsyncWaitHandleType.AssemblyQualifiedName;

            _file = new Lazy<string>(() => file);

            _waitHandle = new Lazy<IAsyncWaitHandle>(() => waitHandle ?? (IAsyncWaitHandle)Activator.CreateInstance(_defaultAsyncWaitHandleType));

            _wasWaitHandleProvided = waitHandle != null;
        }

        // Until WriteAsync has been called, this property is mutable. After it has been called,
        // the value of this property cannot be changed. You can call its setter at this point,
        // but its value will not be changed.
        /// <summary>
        /// Gets or sets the file path where logs are to be written to.
        /// </summary>
        /// <remarks>
        /// Until <see cref="WriteAsync"/> has been called, this property is mutable. After it
        /// has been called, the value of this property cannot be changed. You can call its 
        /// setter after calling <see cref="WriteAsync"/>, but its value will not be changed.
        /// </remarks>
        public string File
        {
            get { return _file.IsValueCreated ? _file.Value : _filePropertyBackingField; }
            set { _filePropertyBackingField = value; }
        }

        public string WaitHandleType { get; set; }

        protected override async Task WriteAsync(LogEntry entry, string formattedLogEntry)
        {
            await _waitHandle.Value.WaitAsync();

            try
            {
                // Ensure that _file.Value is called before calling OnPreWriteAsync in
                // case that method accesses the File property. We do this because the
                // File property returns _file.Value when _file.IsValueCreated is true.
                // Basically, "lock down" the File property before calling OnPreWriteAsync.
                var file = _file.Value;

                await OnPreWriteAsync(entry, formattedLogEntry);

                using (var writer = new StreamWriter(file, true))
                {
                    await writer.WriteLineAsync(formattedLogEntry);
                }

                await OnPostWriteAsync(entry, formattedLogEntry);
            }
            finally
            {
                _waitHandle.Value.Release();
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
            if (!_wasWaitHandleProvided && _waitHandle.IsValueCreated)
            {
                _waitHandle.Value.Dispose();
            }
        }
    }
}