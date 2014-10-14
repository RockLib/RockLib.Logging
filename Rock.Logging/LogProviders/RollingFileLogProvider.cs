using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rock.Logging
{
    public class RollingFileLogProvider : FileLogProvider
    {
        private const int _defaultMaxFileSizeKilobytes = 1024; // 1MB
        private readonly Lazy<int> _maxFileSizeBytes;

        public RollingFileLogProvider(ILogFormatterFactory logFormatterFactory)
            : base(logFormatterFactory)
        {
            MaxFileSizeKilobytes = _defaultMaxFileSizeKilobytes;
            _maxFileSizeBytes = new Lazy<int>(() => GetMaxFileSizeBytes(MaxFileSizeKilobytes));
        }

        public RollingFileLogProvider(
            ILogFormatterFactory logFormatterFactory,
            string file,
            int maxFileSizeKilobytes = _defaultMaxFileSizeKilobytes,
            IAsyncWaitHandle waitHandle = null)
            : base(logFormatterFactory, file, waitHandle)
        {
            MaxFileSizeKilobytes = maxFileSizeKilobytes;
            _maxFileSizeBytes = new Lazy<int>(() => GetMaxFileSizeBytes(maxFileSizeKilobytes));
        }

        public int MaxFileSizeKilobytes { get; set; }

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
                && fileInfo.Length > _maxFileSizeBytes.Value;
        }

        private string GetArchiveFileName()
        {
            var directory = Path.GetDirectoryName(File);

            var fileName = Path.GetFileNameWithoutExtension(File);
            var fileExtension = Path.GetExtension(File);

            var searchPattern = fileName + ".*" + fileExtension;

            var archiveNumber =
                Directory
                    .GetFiles(directory, searchPattern)
                    .Select(GetArchiveNumberString)
                    .Where(IsNumber)
                    .Select(int.Parse)
                    .DefaultIfEmpty()
                    .Max() + 1;

            return Path.Combine(directory, fileName + "." + archiveNumber + fileExtension);
        }

        private static string GetArchiveNumberString(string file)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            return name.Substring(name.LastIndexOf('.') + 1);
        }

        private static bool IsNumber(string archiveNumber)
        {
            return archiveNumber.All(char.IsNumber);
        }

        private static int GetMaxFileSizeBytes(int maxFileSizeKilobytes)
        {
            return maxFileSizeKilobytes * 1024;
        }
    }
}