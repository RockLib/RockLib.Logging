using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rock.Logging
{
    public class RollingFileLogProvider : FileLogProvider
    {
        private const int _defaultMaxFileSizeKilobytes = 1024; // 1MB
        private const int _defaultMaxArchiveCount = 10;
        private const RolloverPeriod _defaultRolloverPeriod = RolloverPeriod.Never;

        private readonly Lazy<int> _maxFileSizeBytes;
        private readonly Lazy<int> _maxArchiveCount;
        private readonly Lazy<RolloverPeriod> _rolloverPeriod;

        public RollingFileLogProvider(ILogFormatterFactory logFormatterFactory)
            : base(logFormatterFactory)
        {
            MaxFileSizeKilobytes = _defaultMaxFileSizeKilobytes;
            MaxArchiveCount = _defaultMaxArchiveCount;
            RolloverPeriod = _defaultRolloverPeriod;

            _maxFileSizeBytes = new Lazy<int>(() => GetMaxFileSizeBytes(MaxFileSizeKilobytes));
            _maxArchiveCount = new Lazy<int>(() => MaxArchiveCount);
            _rolloverPeriod = new Lazy<RolloverPeriod>(() => RolloverPeriod);
        }

        public RollingFileLogProvider(
            ILogFormatterFactory logFormatterFactory,
            string file,
            int maxFileSizeKilobytes = _defaultMaxFileSizeKilobytes,
            int maxArchiveCount = _defaultMaxArchiveCount,
            RolloverPeriod rolloverPeriod = _defaultRolloverPeriod,
            IAsyncWaitHandle waitHandle = null)
            : base(logFormatterFactory, file, waitHandle)
        {
            MaxFileSizeKilobytes = maxFileSizeKilobytes;
            MaxArchiveCount = maxArchiveCount;
            RolloverPeriod = rolloverPeriod;

            _maxFileSizeBytes = new Lazy<int>(() => GetMaxFileSizeBytes(maxFileSizeKilobytes));
            _maxArchiveCount = new Lazy<int>(() => maxArchiveCount);
            _rolloverPeriod = new Lazy<RolloverPeriod>(() => rolloverPeriod);
        }

        public int MaxFileSizeKilobytes { get; set; }
        public int MaxArchiveCount { get; set; }
        public RolloverPeriod RolloverPeriod { get; set; }

        protected override Task OnPreWriteAsync(LogEntry entry, string formattedLogEntry)
        {
            if (NeedsArchiving())
            {
                ArchiveLog();
                PruneArchives();
            }

            return _completedTask;
        }

        private bool NeedsArchiving()
        {
            var fileInfo = new FileInfo(File);

            return
                fileInfo.Exists
                && (ExceedsMaxFileSize(fileInfo) || NeedsNewRolloverPeriod(fileInfo));
        }

        private bool ExceedsMaxFileSize(FileInfo fileInfo)
        {
            return fileInfo.Length > _maxFileSizeBytes.Value;
        }

        private bool NeedsNewRolloverPeriod(FileInfo fileInfo)
        {
            if (_rolloverPeriod.Value == RolloverPeriod.Never)
            {
                return false;
            }

            DateTime currentTime, creationTime;
            SetTimes(fileInfo, out currentTime, out creationTime);

            // Both Daily and Hourly roll over on a date change.
            if (currentTime.Date != creationTime.Date)
            {
                return true;
            }

            // Hourly rolls over when the date is the same, but the hour is different.
            return
                _rolloverPeriod.Value == RolloverPeriod.Hourly
                && currentTime.Hour != creationTime.Hour;
        }

        /// <summary>
        /// Sets <paramref name="currentTime"/> to <see cref="DateTime.UtcNow"/> and <paramref name="creationTime"/> 
        /// to the value of the <see cref="FileSystemInfo.CreationTimeUtc"/> property of <paramref name="fileInfo"/>. 
        /// This virtual method exists to facilitate the testing of periodic archiving.
        /// </summary>
        protected virtual void SetTimes(FileInfo fileInfo, out DateTime currentTime, out DateTime creationTime)
        {
            currentTime = DateTime.UtcNow;
            creationTime = fileInfo.CreationTimeUtc;
        }

        private void ArchiveLog()
        {
            System.IO.File.Move(File, GetArchiveFileName());
        }

        private string GetArchiveFileName()
        {
            string directory;
            string fileName;
            string fileExtension;

            var archiveFiles = GetArchiveFiles(out directory, out fileName, out fileExtension);

            var archiveNumber =
                archiveFiles
                    .Select(f => f.ArchiveNumber)
                    .DefaultIfEmpty()
                    .Max() + 1;

            return Path.Combine(directory, fileName + "." + archiveNumber + fileExtension);
        }

        private static string GetArchiveNumberString(string file)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            return name.Substring(name.LastIndexOf('.') + 1);
        }

        private static bool IsNumber(string archiveNumberString)
        {
            return archiveNumberString.All(char.IsNumber);
        }

        private void PruneArchives()
        {
            var archiveFiles = GetArchiveFiles().OrderBy(f => f.ArchiveNumber).ToList();

            while (archiveFiles.Count > _maxArchiveCount.Value)
            {
                System.IO.File.Delete(archiveFiles.First().File);
                archiveFiles.RemoveAt(0);
            }
        }

        private static int GetMaxFileSizeBytes(int maxFileSizeKilobytes)
        {
            return maxFileSizeKilobytes * 1024;
        }

        private IEnumerable<ArchiveFile> GetArchiveFiles()
        {
            string dummy1, dummy2, dummy3;
            return GetArchiveFiles(out dummy1, out dummy2, out dummy3);
        }

        private IEnumerable<ArchiveFile> GetArchiveFiles(out string directory, out string fileName, out string fileExtension)
        {
            directory = Path.GetDirectoryName(File);

            fileName = Path.GetFileNameWithoutExtension(File);
            fileExtension = Path.GetExtension(File);

            var searchPattern = fileName + ".*" + fileExtension;

            return
                from file in Directory.GetFiles(directory, searchPattern)
                let archiveNumberString = GetArchiveNumberString(file)
                where IsNumber(archiveNumberString)
                select new ArchiveFile { File = file, ArchiveNumber = int.Parse(archiveNumberString) };
        }

        private class ArchiveFile
        {
            public string File { get; set; }
            public int ArchiveNumber { get; set; }
        }
    }
}