using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rock.Logging
{
    public class RollingFileLogProvider : FileLogProvider
    {
        private readonly Lazy<int> _maxFileSizeMegabytes;

        public RollingFileLogProvider(ILogFormatterFactory logFormatterFactory)
            : base(logFormatterFactory)
        {
            _maxFileSizeMegabytes = new Lazy<int>(() => MaxFileSizeMegabytes);
        }

        public RollingFileLogProvider(ILogFormatterFactory logFormatterFactory, string file, int maxFileSizeMegabytes)
            : base(logFormatterFactory, file)
        {
            _maxFileSizeMegabytes = new Lazy<int>(() => maxFileSizeMegabytes);
        }

        public int MaxFileSizeMegabytes { get; set; }

        protected override Task OnPreWriteAsync(LogEntry entry, string formattedLogEntry)
        {
            if (NeedsRollover())
            {
                System.IO.File.Move(File, GetArchiveFileName());
            }

            return _completedTask;
        }

        private bool NeedsRollover()
        {
            var fileInfo = new FileInfo(File);
            return
                fileInfo.Exists
                && GetSizeInMegabytes(fileInfo) > _maxFileSizeMegabytes.Value;
        }

        private static long GetSizeInMegabytes(FileInfo fileInfo)
        {
            return fileInfo.Length / (1024 * 1024);
        }

        private string GetArchiveFileName()
        {
            var directory = Path.GetDirectoryName(File);

            var fileName = Path.GetFileNameWithoutExtension(File);
            var fileExtension = Path.GetExtension(File);

            var searchPattern = fileName + ".*" + fileExtension;

            var archiveNumber =
                Directory.GetFiles(directory, searchPattern)
                    .Select(GetArchiveNumber)
                    .Where(x => x.All(char.IsNumber))
                    .Select(int.Parse)
                    .DefaultIfEmpty(-1)
                    .Max() + 1;

            return fileName + "." + archiveNumber + fileExtension;
        }

        private static string GetArchiveNumber(string x)
        {
            var name = Path.GetFileNameWithoutExtension(x);
            return name.Substring(name.LastIndexOf('.') + 1);
        }
    }
}