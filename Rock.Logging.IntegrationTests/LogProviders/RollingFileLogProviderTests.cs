using System;
using System.IO;
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
            const double maxFileSizeMegabytes = .05;

            var serializer = new XmlSerializerSerializer();

            var logProvider =
                new RollingFileLogProvider(
                    new SerializingLogFormatterFactory(serializer),
                    _logFilePath,
                    maxFileSizeMegabytes);

            LogEntry logEntry;

            try
            {
                throw new Exception();
            }
            catch (Exception ex)
            {
                logEntry = new LogEntry("Hello, world!", new { Foo = "bar", Who = "there" }, ex, "We're in a test!");
            }

            // Write to the log file until it has exceeded the max file size.
            while (IsTooSmallForRollOver(maxFileSizeMegabytes))
            {
                await logProvider.WriteAsync(logEntry);
                Assert.That(Directory.GetFiles(_logFileDirectory).Length, Is.EqualTo(1));
            }

            // This log entry should cause the existing log file to be archived, and a new one created.
            await logProvider.WriteAsync(logEntry);

            Assert.That(Directory.GetFiles(_logFileDirectory).Length, Is.EqualTo(2));
        }

        private static bool IsTooSmallForRollOver(double maxFileSizeMegabytes)
        {
            var fileInfo = new FileInfo(_logFilePath);

            return !fileInfo.Exists || (((double)fileInfo.Length) / (1024 * 1024)) < maxFileSizeMegabytes;
        }

        protected override ILogProvider CreateLogProvider(XmlSerializerSerializer serializer, string logFilePath)
        {
            return new RollingFileLogProvider(
                new SerializingLogFormatterFactory(serializer),
                logFilePath);
        }
    }
}
