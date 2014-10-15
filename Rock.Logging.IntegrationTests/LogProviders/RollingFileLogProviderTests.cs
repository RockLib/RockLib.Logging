using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Rock.Logging;
using Rock.Serialization;

// ReSharper disable once CheckNamespace
namespace RollingFileLogProviderTests
{
    // The RollingFileLogProvider behaves just like a FileLogProvider (except for the rolling over part),
    // so text it just like a FileLogProvider.
    public class TheWriteAsyncMethod : FileLogProviderTests.TheWriteAsyncMethod
    {
        [Test]
        public async void CausesTheLogFileToBeArchivedWhenItsSizeGetsTooBig()
        {
            const int maxFileSizeKilobytes = 50;

            var serializer = new XmlSerializerSerializer();

            var logProvider =
                new RollingFileLogProvider(
                    new SerializingLogFormatterFactory(serializer),
                    _logFilePath,
                    maxFileSizeKilobytes);

            Assert.That(Directory.GetFiles(_logFileDirectory).Length, Is.EqualTo(0));

            await logProvider.WriteAsync(GetLogEntry());
            Assert.That(Directory.GetFiles(_logFileDirectory).Length, Is.EqualTo(1));

            await MakeOneArchiveFile(maxFileSizeKilobytes, logProvider);
            Assert.That(Directory.GetFiles(_logFileDirectory).Length, Is.EqualTo(2));
        }

        [Test]
        public async void PrunesOldArchiveFiles()
        {
            const int maxFileSizeKilobytes = 50;

            var serializer = new XmlSerializerSerializer();

            var logProvider =
                new RollingFileLogProvider(
                    new SerializingLogFormatterFactory(serializer),
                    _logFilePath,
                    maxFileSizeKilobytes,
                    2);

            Assert.That(Directory.GetFiles(_logFileDirectory).Length, Is.EqualTo(0));

            await logProvider.WriteAsync(GetLogEntry());
            Assert.That(Directory.GetFiles(_logFileDirectory).Length, Is.EqualTo(1));

            await MakeOneArchiveFile(maxFileSizeKilobytes, logProvider);
            Assert.That(Directory.GetFiles(_logFileDirectory).Length, Is.EqualTo(2));

            await MakeOneArchiveFile(maxFileSizeKilobytes, logProvider);
            Assert.That(Directory.GetFiles(_logFileDirectory).Length, Is.EqualTo(3));

            await MakeOneArchiveFile(maxFileSizeKilobytes, logProvider);
            Assert.That(Directory.GetFiles(_logFileDirectory).Length, Is.EqualTo(3));

            await MakeOneArchiveFile(maxFileSizeKilobytes, logProvider);
            Assert.That(Directory.GetFiles(_logFileDirectory).Length, Is.EqualTo(3));
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
                new SerializingLogFormatterFactory(serializer),
                logFilePath);
        }
    }
}
