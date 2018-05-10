using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RockLib.Logging
{
    public class RollingFileLogProvider : FileLogProvider
    {
        public const int DefaultMaxFileSizeKilobytes = 1024; // 1MB
        public const int DefaultMaxArchiveCount = 10;
        public const RolloverPeriod DefaultRolloverPeriod = RolloverPeriod.Never;

        private static readonly Func<DateTime> _defaultGetCurrentTime = () => DateTime.UtcNow;
        private static readonly Func<FileInfo, DateTime> _defaultGetFileCreationTime = fileInfo => fileInfo.CreationTimeUtc;

        private readonly Func<DateTime> _getCurrentTime;
        private readonly Func<FileInfo, DateTime> _getFileCreationTime;

        public RollingFileLogProvider(string file,
            string template = DefaultTemplate,
            LogLevel level = default(LogLevel),
            TimeSpan? timeout = null,
            int maxFileSizeKilobytes = DefaultMaxFileSizeKilobytes,
            int maxArchiveCount = DefaultMaxArchiveCount,
            RolloverPeriod rolloverPeriod = DefaultRolloverPeriod)
            : this(_defaultGetCurrentTime, _defaultGetFileCreationTime, file, new TemplateLogFormatter(template ?? DefaultTemplate), level, timeout, maxFileSizeKilobytes, maxArchiveCount, rolloverPeriod)
        {
        }

        public RollingFileLogProvider(string file,
            ILogFormatter formatter,
            LogLevel level = default(LogLevel),
            TimeSpan? timeout = null,
            int maxFileSizeKilobytes = DefaultMaxFileSizeKilobytes,
            int maxArchiveCount = DefaultMaxArchiveCount,
            RolloverPeriod rolloverPeriod = DefaultRolloverPeriod)
            : this(_defaultGetCurrentTime, _defaultGetFileCreationTime, file, formatter, level, timeout, maxFileSizeKilobytes, maxArchiveCount, rolloverPeriod)
        {
        }

        protected RollingFileLogProvider(
            Func<DateTime> getCurrentTime,
            Func<FileInfo, DateTime> getFileCreationTime,
            string file,
            ILogFormatter formatter,
            LogLevel level = default(LogLevel),
            TimeSpan? timeout = null,
            int maxFileSizeKilobytes = DefaultMaxFileSizeKilobytes,
            int maxArchiveCount = DefaultMaxArchiveCount,
            RolloverPeriod rolloverPeriod = DefaultRolloverPeriod)
            : base(file, formatter, level, timeout)
        {
            _getCurrentTime = getCurrentTime ?? throw new ArgumentNullException(nameof(getCurrentTime));
            _getFileCreationTime = getFileCreationTime ?? throw new ArgumentNullException(nameof(getFileCreationTime));
            MaxFileSizeBytes = GetMaxFileSizeBytes(maxFileSizeKilobytes);
            MaxArchiveCount = maxArchiveCount;
            RolloverPeriod = rolloverPeriod;
        }

        public int MaxFileSizeBytes { get; }
        public int MaxArchiveCount { get; }
        public RolloverPeriod RolloverPeriod { get; }

        protected sealed override void OnPreWrite(LogEntry entry, string formattedLogEntry)
        {
            if (NeedsArchiving())
            {
                ArchiveCurrentFile();
                PruneArchives();
            }
        }

        private bool NeedsArchiving()
        {
            var fileInfo = new FileInfo(File);

            return
                fileInfo.Exists
                && (ExceedsMaxFileSize(fileInfo) || NeedsNewRolloverPeriod(fileInfo));
        }

        private bool ExceedsMaxFileSize(FileInfo fileInfo) => fileInfo.Length > MaxFileSizeBytes;

        private bool NeedsNewRolloverPeriod(FileInfo fileInfo)
        {
            if (RolloverPeriod == RolloverPeriod.Never)
                return false;

            var currentTime = _getCurrentTime();
            var fileCreationTime = _getFileCreationTime(fileInfo);

            // Both Daily and Hourly roll over on a date change.
            if (currentTime.Date != fileCreationTime.Date)
                return true;

            // Hourly rolls over when the date is the same, but the hour is different.
            return
                RolloverPeriod == RolloverPeriod.Hourly
                && currentTime.Hour != fileCreationTime.Hour;
        }

        private void ArchiveCurrentFile() =>
            System.IO.File.Move(File, GetArchiveFileName());

        private string GetArchiveFileName()
        {
            var archiveFiles = GetArchiveFiles(out var directory, out var fileName, out var fileExtension);

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

        private static bool IsNumber(string archiveNumberString) =>
            archiveNumberString.All(char.IsNumber);

        private void PruneArchives()
        {
            var archiveFiles = GetArchiveFiles().OrderBy(f => f.ArchiveNumber).ToList();

            while (archiveFiles.Count > MaxArchiveCount)
            {
                System.IO.File.Delete(archiveFiles.First().File);
                archiveFiles.RemoveAt(0);
            }
        }

        private static int GetMaxFileSizeBytes(int maxFileSizeKilobytes) =>
            maxFileSizeKilobytes * 1024;

        private IEnumerable<ArchiveFile> GetArchiveFiles() =>
            GetArchiveFiles(out string dummy1, out string dummy2, out string dummy3);

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

        private struct ArchiveFile
        {
            public string File { get; set; }
            public int ArchiveNumber { get; set; }
        }
    }
}
