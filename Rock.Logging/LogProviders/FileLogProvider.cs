using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Rock.Logging
{
    public class FileLogProvider : FormattableLogProvider, IDisposable
    {
        private const string DefaultFile = @"C:\temp\MyAppLog.txt";

        private string _file;
        private Mutex _mutex;

        public FileLogProvider(ILogFormatterFactory logFormatterFactory)
            : base(logFormatterFactory)
        {
            File = DefaultFile;
        }

        public string File
        {
            get { return _file; }
            set
            {
                if (value == _file)
                {
                    return;
                }

                _file = value ?? DefaultFile;

                if (_mutex != null)
                {
                    _mutex.Close();
                }

                _mutex = new Mutex(false, "Rock.Logging.FileLogProvider." + _file.Replace('\\', '.'));
            }
        }

        protected override async Task Write(string formattedLogEntry)
        {
            _mutex.WaitOne();

            try
            {
                using (var writer = new StreamWriter(File, true))
                {
                    await writer.WriteLineAsync(formattedLogEntry);
                }
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public virtual void Dispose()
        {
            if (_mutex != null)
            {
                _mutex.Close();
            }
        }
    }
}