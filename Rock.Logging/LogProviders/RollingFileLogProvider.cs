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

        private readonly int _maxFileSizeBytes;
        private readonly int _maxArchiveCount;
        private readonly RolloverPeriod _rolloverPeriod;

        public RollingFileLogProvider()                                                                                                             // ReSharper disable RedundantArgumentDefaultValue
            : this(null, _defaultMaxFileSizeKilobytes, _defaultMaxArchiveCount, _defaultRolloverPeriod, null, null)                                 // ReSharper restore RedundantArgumentDefaultValue
        {
        }

        public RollingFileLogProvider(
            string file = null,
            int maxFileSizeKilobytes = _defaultMaxFileSizeKilobytes,
            int maxArchiveCount = _defaultMaxArchiveCount,
            RolloverPeriod rolloverPeriod = _defaultRolloverPeriod,
            ILogFormatter logFormatter = null,
            IAsyncWaitHandle waitHandle = null)
            : base(file, logFormatter, waitHandle)
        {
            _maxFileSizeBytes = GetMaxFileSizeBytes(maxFileSizeKilobytes);
            _maxArchiveCount = maxArchiveCount;
            _rolloverPeriod = rolloverPeriod;
        }

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
            var fileInfo = new FileInfo(_file);

            return
                fileInfo.Exists
                && (ExceedsMaxFileSize(fileInfo) || NeedsNewRolloverPeriod(fileInfo));
        }

        private bool ExceedsMaxFileSize(FileInfo fileInfo)
        {
            return fileInfo.Length > _maxFileSizeBytes;
        }

        private bool NeedsNewRolloverPeriod(FileInfo fileInfo)
        {
            if (_rolloverPeriod == RolloverPeriod.Never)
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
                _rolloverPeriod == RolloverPeriod.Hourly
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
            File.Move(_file, GetArchiveFileName());
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

            while (archiveFiles.Count > _maxArchiveCount)
            {
                File.Delete(archiveFiles.First().File);
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
            directory = Path.GetDirectoryName(_file);

            fileName = Path.GetFileNameWithoutExtension(_file);
            fileExtension = Path.GetExtension(_file);

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