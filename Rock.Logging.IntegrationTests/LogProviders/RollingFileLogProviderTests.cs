using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Rock.Logging;
using Rock.Logging.Defaults;
using Rock.Serialization;

// ReSharper disable once CheckNamespace
namespace RollingFileLogProviderTests
{
    // The RollingFileLogProvider behaves just like a FileLogProvider (except for the rolling over part),
    // so test it just like a FileLogProvider.
    public class TheWriteAsyncMethod : FileLogProviderTests.TheWriteAsyncMethod
    {
        [Test]
        public async void CausesTheLogFileToBeArchivedWhenItsSizeGetsTooBig()
        {
            const int maxFileSizeKilobytes = 50;

            var logProvider =
                new RollingFileLogProvider(
                    _logFilePath,
                    maxFileSizeKilobytes,
                    logFormatter: new SerializingLogFormatter(new XmlSerializerSerializer()));

            Assert.That(GetFileCount(), Is.EqualTo(0));

            await logProvider.WriteAsync(GetLogEntry());
            Assert.That(GetFileCount(), Is.EqualTo(1));

            await MakeOneArchiveFile(maxFileSizeKilobytes, logProvider);
            Assert.That(GetFileCount(), Is.EqualTo(2));
        }

        [Test]
        public async void PrunesOldArchiveFiles()
        {
            const int maxFileSizeKilobytes = 50;

            var logProvider =
                new RollingFileLogProvider(
                    _logFilePath,
                    maxFileSizeKilobytes,
                    2,
                    logFormatter: new SerializingLogFormatter(new XmlSerializerSerializer()));

            Assert.That(GetFileCount(), Is.EqualTo(0));

            await logProvider.WriteAsync(GetLogEntry());
            Assert.That(GetFileCount(), Is.EqualTo(1));

            await MakeOneArchiveFile(maxFileSizeKilobytes, logProvider);
            Assert.That(GetFileCount(), Is.EqualTo(2));

            await MakeOneArchiveFile(maxFileSizeKilobytes, logProvider);
            Assert.That(GetFileCount(), Is.EqualTo(3));

            await MakeOneArchiveFile(maxFileSizeKilobytes, logProvider);
            Assert.That(GetFileCount(), Is.EqualTo(3));

            await MakeOneArchiveFile(maxFileSizeKilobytes, logProvider);
            Assert.That(GetFileCount(), Is.EqualTo(3));
        }

        [Test]
        public async void ArchivesTheLogFileEveryHourOnTheHourWhenRolloverPeriodIsHourly()
        {
            RollingFileLogProvider logProvider = new TestingRollingFileLogProvider(
                _logFilePath,
                RolloverPeriod.Hourly,
                new TimeSet(new DateTime(2014, 10, 14, 11, 30, 0), new DateTime(2014, 10, 15, 13, 15, 0)),  // A) Different date, different hour.
                new TimeSet(new DateTime(2014, 10, 14, 11, 30, 0), new DateTime(2014, 10, 15, 11, 45, 0)),  // B) Different date, same hour.
                new TimeSet(new DateTime(2014, 10, 15, 11, 45, 0), new DateTime(2014, 10, 15, 13, 30, 0)),  // C) Same date, different hour.
                new TimeSet(new DateTime(2014, 10, 15, 13, 15, 0), new DateTime(2014, 10, 15, 13, 30, 0))); // D) Same date, same hour.

            Assert.That(GetFileCount(), Is.EqualTo(0));

            await logProvider.WriteAsync(GetLogEntry()); // New file, doesn't consume TimeSet
            Assert.That(GetFileCount(), Is.EqualTo(1));

            await logProvider.WriteAsync(GetLogEntry()); // A) Archive: Different date, different hour.
            Assert.That(GetFileCount(), Is.EqualTo(2));

            await logProvider.WriteAsync(GetLogEntry()); // B) Archive: Different date, same hour.
            Assert.That(GetFileCount(), Is.EqualTo(3));

            await logProvider.WriteAsync(GetLogEntry()); // C) Archive: Same date, different hour.
            Assert.That(GetFileCount(), Is.EqualTo(4));

            await logProvider.WriteAsync(GetLogEntry()); // D) No Archive: Same date, same hour.
            Assert.That(GetFileCount(), Is.EqualTo(4));
        }

        [Test]
        public async void ArchivesTheLogFileEveryDayAtMidnightWhenRolloverPeriodIsDaily()
        {
            RollingFileLogProvider logProvider = new TestingRollingFileLogProvider(
                _logFilePath,
                RolloverPeriod.Daily,
                new TimeSet(new DateTime(2014, 10, 14, 11, 30, 0), new DateTime(2014, 10, 15, 13, 15, 0)),  // A) Different date, different hour.
                new TimeSet(new DateTime(2014, 10, 14, 11, 30, 0), new DateTime(2014, 10, 15, 11, 45, 0)),  // B) Different date, same hour.
                new TimeSet(new DateTime(2014, 10, 15, 11, 45, 0), new DateTime(2014, 10, 15, 13, 30, 0)),  // C) Same date, different hour.
                new TimeSet(new DateTime(2014, 10, 15, 13, 15, 0), new DateTime(2014, 10, 15, 13, 30, 0))); // D) Same date, same hour.

            Assert.That(GetFileCount(), Is.EqualTo(0));

            await logProvider.WriteAsync(GetLogEntry()); // New file, doesn't consume TimeSet
            Assert.That(GetFileCount(), Is.EqualTo(1));

            await logProvider.WriteAsync(GetLogEntry()); // A) Archive: Different date, different hour.
            Assert.That(GetFileCount(), Is.EqualTo(2));

            await logProvider.WriteAsync(GetLogEntry()); // B) Archive: Different date, same hour.
            Assert.That(GetFileCount(), Is.EqualTo(3));

            await logProvider.WriteAsync(GetLogEntry()); // C) No Archive: Same date, different hour.
            Assert.That(GetFileCount(), Is.EqualTo(3));

            await logProvider.WriteAsync(GetLogEntry()); // D) No Archive: Same date, same hour.
            Assert.That(GetFileCount(), Is.EqualTo(3));
        }

        private static int GetFileCount()
        {
            var fileCount = Directory.GetFiles(_logFileDirectory).Length;
            return fileCount;
        }

        private static async Task MakeOneArchiveFile(int maxFileSizeKilobytes, RollingFileLogProvider logProvider)
        {
            var logEntry = GetLogEntry();

            // Write to the log file until it has exceeded the max file size.
            while (IsTooSmallForRollOver(maxFileSizeKilobytes))
            {
                await logProvider.WriteAsync(logEntry);
            }

            // This log entry should cause the existing log file to be archived, and a new one created.
            await logProvider.WriteAsync(logEntry);
        }

        private static LogEntry GetLogEntry()
        {
            try
            {
                throw new Exception();
            }
            catch (Exception ex)
            {
                return new LogEntry("Hello, world!", new { Foo = "bar", Who = "there" }, ex, "We're in a test!");
            }
        }

        private static bool IsTooSmallForRollOver(double maxFileSizeMegabytes)
        {
            var fileInfo = new FileInfo(_logFilePath);

            return !fileInfo.Exists || (((double)fileInfo.Length) / 1024) < maxFileSizeMegabytes;
        }

        protected override ILogProvider CreateLogProvider(XmlSerializerSerializer serializer, string logFilePath)
        {
            return new RollingFileLogProvider(
                logFilePath,
                logFormatter: new SerializingLogFormatter(new XmlSerializerSerializer()));
        }

        private class TestingRollingFileLogProvider : RollingFileLogProvider
        {
            private readonly TimeSet[] _timeSets;
            private int _timeSetsIndex;

            public TestingRollingFileLogProvider(
                string file,
                RolloverPeriod rolloverPeriod,
                params TimeSet[] timeSets)
                : base(
                    file,
                    rolloverPeriod: rolloverPeriod,
                    logFormatter: new SerializingLogFormatter(new XmlSerializerSerializer()))
            {
                _timeSets = timeSets;
            }

            protected override void SetTimes(FileInfo fileInfo, out DateTime currentTime, out DateTime creationTime)
            {
                var timeSet = _timeSets[_timeSetsIndex++ % _timeSets.Length];
                currentTime = timeSet.CurrentTime;
                creationTime = timeSet.CreationTime;
            }
        }

        private struct TimeSet
        {
            private readonly DateTime _creationTime;
            private readonly DateTime _currentTime;

            public TimeSet(DateTime creationTime, DateTime currentTime)
            {
                _creationTime = creationTime;
                _currentTime = currentTime;
            }

            public DateTime CreationTime { get { return _creationTime; } }
            public DateTime CurrentTime { get { return _currentTime; } }
        }
    }
}
