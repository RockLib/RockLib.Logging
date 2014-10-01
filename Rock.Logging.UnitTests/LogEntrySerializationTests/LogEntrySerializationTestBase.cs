using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using Rock.Collections;
using Rock.Logging;
using Rock.Serialization;

namespace LoggerTests.LogEntrySerializationTests
{
    public abstract class LogEntrySerializationTestBase
    {
        private readonly IEqualityComparer _equalityComparer = new DeepEqualityComparer();

        protected ISerializer _serializer;

        [SetUp]
        public void Setup()
        {
            _serializer = GetSerializer();
        }

        [Test]
        public void String()
        {
            var logEntry = GetLogEntry();

            var serialized = _serializer.SerializeToString(logEntry);

            var roundTrip = _serializer.DeserializeFromString<LogEntry>(serialized);

            Assert.That(_equalityComparer.Equals(logEntry, roundTrip), Is.True);
        }

        [Test]
        public void Stream()
        {
            var logEntry = GetLogEntry();

            byte[] data;

            using (var stream = new MemoryStream())
            {
                _serializer.SerializeToStream(stream, logEntry);
                stream.Flush();
                data = stream.ToArray();
            }

            LogEntry roundTrip;

            using (var stream = new MemoryStream(data))
            {
                roundTrip = _serializer.DeserializeFromStream<LogEntry>(stream);
            }

            Assert.That(_equalityComparer.Equals(logEntry, roundTrip), Is.True);
        }

        private static LogEntry GetLogEntry()
        {
            var now = DateTime.UtcNow;

            return new LogEntry
            {
                CreateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second)
            };
        }

        protected abstract ISerializer GetSerializer();
    }
}