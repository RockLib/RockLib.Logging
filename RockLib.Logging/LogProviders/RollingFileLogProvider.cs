using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.Logging
{
    /// <summary>
    /// An implementation of <see cref="ILogProvider"/> that writes log entries to a file. Depending
    /// on configured criteria, the log file is archived (renamed) and a new file is written in its
    /// place.
    /// </summary>
    public class RollingFileLogProvider : FileLogProvider
    {
        /// <summary>
        /// The default value for <see cref="MaxFileSizeBytes"/>.
        /// </summary>
        public const int DefaultMaxFileSizeKilobytes = 1024; // 1MB

        /// <summary>
        /// The default value for <see cref="MaxArchiveCount"/>.
        /// </summary>
        public const int DefaultMaxArchiveCount = 10;

        /// <summary>
        /// The default value for <see cref="RolloverPeriod"/>.
        /// </summary>
        public const RolloverPeriod DefaultRolloverPeriod = RolloverPeriod.Never;

        private static readonly Func<DateTime> _defaultGetCurrentTime = () => DateTime.UtcNow;
        private static readonly Func<FileInfo, DateTime> _defaultGetFileCreationTime = fileInfo => fileInfo.CreationTimeUtc;

        private readonly Func<DateTime> _getCurrentTime;
        private readonly Func<FileInfo, DateTime> _getFileCreationTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingFileLogProvider"/> class.
        /// </summary>
        /// <param name="file">The file that logs will be written to.</param>
        /// <param name="template">The template used to format log entries.</param>
        /// <param name="level">The level of the log provider.</param>
        /// <param name="timeout">The timeout of the log provider.</param>
        /// <param name="maxFileSizeKilobytes">
        /// The maximum file size, in bytes, of the file. If the file size is greater than this value,
        /// it is archived.
        /// </param>
        /// <param name="maxArchiveCount">
        /// The maximum number of archive files that will be kept. If the number of archive files is
        /// greater than this value, then they are deleted, oldest first.
        /// </param>
        /// <param name="rolloverPeriod">
        /// The rollover period, indicating if/how the file should archived on a periodic basis.
        /// </param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingFileLogProvider"/> class.
        /// </summary>
        /// <param name="file">The path to a writable file.</param>
        /// <param name="formatter">An object that formats log entries prior to writing to file.</param>
        /// <param name="level">The level of the log provider.</param>
        /// <param name="timeout">The timeout of the log provider.</param>
        /// <param name="maxFileSizeKilobytes">
        /// The maximum file size, in bytes, of the file. If the file size is greater than this value,
        /// it is archived.
        /// </param>
        /// <param name="maxArchiveCount">
        /// The maximum number of archive files that will be kept. If the number of archive files is
        /// greater than this value, then they are deleted, oldest first.
        /// </param>
        /// <param name="rolloverPeriod">
        /// The rollover period, indicating if/how the file should archived on a periodic basis.
        /// </param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingFileLogProvider"/> class. This protected
        /// constructor exists to facilitate testing - its <paramref name="getCurrentTime"/> and
        /// <paramref name="getFileCreationTime"/> parameters allow a subclass to inject system clock
        /// and file system behavior.
        /// </summary>
        /// <param name="getCurrentTime">A function that gets the current time.</param>
        /// <param name="getFileCreationTime">A function that gets a file's creation time.</param>
        /// <param name="file">The path to a writable file.</param>
        /// <param name="formatter">An object that formats log entries prior to writing to file.</param>
        /// <param name="level">The level of the log provider.</param>
        /// <param name="timeout">The timeout of the log provider.</param>
        /// <param name="maxFileSizeKilobytes">
        /// The maximum file size, in bytes, of the file. If the file size is greater than this value,
        /// it is archived.
        /// </param>
        /// <param name="maxArchiveCount">
        /// The maximum number of archive files that will be kept. If the number of archive files is
        /// greater than this value, then they are deleted, oldest first.
        /// </param>
        /// <param name="rolloverPeriod">
        /// The rollover period, indicating if/how the file should archived on a periodic basis.
        /// </param>
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
            if (maxFileSizeKilobytes < 0)
                throw new ArgumentException("MaxFileSizeKilobytes cannot be negative.", nameof(maxFileSizeKilobytes));
            if (maxFileSizeKilobytes > (int.MaxValue / 1024))
                throw new ArgumentException($"MaxFileSizeKilobytes cannot be greater than {int.MaxValue / 1024}.", nameof(maxFileSizeKilobytes));
            if (maxArchiveCount < 0)
                throw new ArgumentException("MaxArchiveCount cannot be negative.", nameof(maxArchiveCount));
            if (!Enum.IsDefined(typeof(RolloverPeriod), rolloverPeriod))
                throw new ArgumentException($"Rollover period is not defined: {rolloverPeriod}.", nameof(rolloverPeriod));

            _getCurrentTime = getCurrentTime ?? throw new ArgumentNullException(nameof(getCurrentTime));
            _getFileCreationTime = getFileCreationTime ?? throw new ArgumentNullException(nameof(getFileCreationTime));
            MaxFileSizeBytes = GetMaxFileSizeBytes(maxFileSizeKilobytes);
            MaxArchiveCount = maxArchiveCount;
            RolloverPeriod = rolloverPeriod;
        }

        /// <summary>
        /// Gets the maximum file size, in bytes, of the file. If the file size is greater than this
        /// value, it is archived.
        /// </summary>
        public int MaxFileSizeBytes { get; }

        /// <summary>
        /// Gets the maximum number of archive files that will be kept. If the number of archive files
        /// is greater than this value, then they are deleted, oldest first.
        /// </summary>
        public int MaxArchiveCount { get; }

        /// <summary>
        /// Gets the rollover period, indicating if/how the file should archived on a periodic basis.
        /// </summary>
        public RolloverPeriod RolloverPeriod { get; }

        /// <summary>
        /// Check to see if the current file needs to be archived. If it does, archive it
        /// and prune the archive files if needed. Then, regardless of whether the file needed
        /// archiving, call the base method.
        /// </summary>
        /// <param name="formattedLogEntry">The formatted log entry.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
        /// <returns> A task that represents the asynchronous write operation.</returns>
        protected sealed override Task SynchronizedWriteAsync(string formattedLogEntry, CancellationToken cancellationToken)
        {
            if (NeedsArchiving())
            {
                ArchiveCurrentFile();
                PruneArchives();
            }

            return base.SynchronizedWriteAsync(formattedLogEntry, cancellationToken);
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
