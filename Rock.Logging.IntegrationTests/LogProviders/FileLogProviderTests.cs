using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Rock.Collections;
using Rock.Logging;
using Rock.Serialization;

// ReSharper disable once CheckNamespace
namespace FileLogProviderTests
{
    public class TheWriteAsyncMethod
    {
        private const string _xmlDeclaration = @"<?xml version=""1.0"" encoding=""utf-8""?>";

        private static readonly string _logFileDirectory = Path.Combine(Path.GetTempPath(), "Rock.Logging.IntegrationTests");
        private static readonly string _logFilePath = Path.Combine(_logFileDirectory, "log.txt");

        private static readonly IEqualityComparer _equalityComparer = new DeepEqualityComparer();

        [SetUp]
        public void Setup()
        {
            if (Directory.Exists(_logFileDirectory))
            {
                Directory.Delete(_logFileDirectory, true);
            }

            Directory.CreateDirectory(_logFileDirectory);
        }

        [TearDown]
        public void Teardown()
        {
            if (Directory.Exists(_logFileDirectory))
            {
                Directory.Delete(_logFileDirectory, true);
            }
        }

        [Test]
        public async void ASingleLogEntryIsWrittenToTheTargetLogFile()
        {
            var serializer = new XmlSerializerSerializer();

            var logProvider = CreateLogProvider(serializer, _logFilePath);

            var logEntry = new LogEntry("Hello, world!", new { Foo = "bar" });
            await logProvider.WriteAsync(logEntry);

            var logFileContents = File.ReadAllText(_logFilePath);

            var xmlDocuments =
                logFileContents.Split(
                    new[] { _xmlDeclaration },
                    StringSplitOptions.RemoveEmptyEntries);

            Assert.That(xmlDocuments.Length, Is.EqualTo(1));

            var deserializedLogEntry = serializer.DeserializeFromString<LogEntry>(xmlDocuments[0]);
            Assert.That(_equalityComparer.Equals(logEntry, deserializedLogEntry), Is.True);
        }

        [Test]
        public async void MultipleLogEntriesAreWrittenToTheTargetLogFile()
        {
            var serializer = new XmlSerializerSerializer();

            var logProvider = CreateLogProvider(serializer, _logFilePath);

            var logEntry0 = new LogEntry("Hello, world!", new { Foo = "bar" });
            await logProvider.WriteAsync(logEntry0);

            var logEntry1 = new LogEntry("Good-bye, cruel world!", new { Omg = "wtf" });
            await logProvider.WriteAsync(logEntry1);

            var logEntry2 = new LogEntry("Awkward message, trying to compete with the other two", new { Sad = "panda" });
            await logProvider.WriteAsync(logEntry2);

            var logFileContents = File.ReadAllText(_logFilePath);

            var xmlDocuments =
                logFileContents.Split(
                    new[] { _xmlDeclaration },
                    StringSplitOptions.RemoveEmptyEntries);

            Assert.That(xmlDocuments.Length, Is.EqualTo(3));

            var deserializedLogEntry0 = serializer.DeserializeFromString<LogEntry>(xmlDocuments[0]);
            Assert.That(_equalityComparer.Equals(logEntry0, deserializedLogEntry0), Is.True);

            var deserializedLogEntry1 = serializer.DeserializeFromString<LogEntry>(xmlDocuments[1]);
            Assert.That(_equalityComparer.Equals(logEntry1, deserializedLogEntry1), Is.True);

            var deserializedLogEntry2 = serializer.DeserializeFromString<LogEntry>(xmlDocuments[2]);
            Assert.That(_equalityComparer.Equals(logEntry2, deserializedLogEntry2), Is.True);
        }

        [Test]
        public async void AnExistingFileIsAppended()
        {
            File.WriteAllLines(_logFilePath, new [] { _xmlDeclaration });

            var fileInfo = new FileInfo(_logFilePath);
            Assert.That(fileInfo.Length, Is.EqualTo(_xmlDeclaration.Length + Environment.NewLine.Length));

            var serializer = new XmlSerializerSerializer();

            var logProvider = CreateLogProvider(serializer, _logFilePath);

            var logEntry = new LogEntry("Hello, world!", new { Foo = "bar" });
            await logProvider.WriteAsync(logEntry);

            var logFileContents = File.ReadAllText(_logFilePath);

            var xmlDocuments =
                logFileContents.Split(
                    new[] { _xmlDeclaration + Environment.NewLine },
                    StringSplitOptions.RemoveEmptyEntries);

            Assert.That(xmlDocuments.Length, Is.EqualTo(1));

            var deserializedLogEntry = serializer.DeserializeFromString<LogEntry>(xmlDocuments[0]);
            Assert.That(_equalityComparer.Equals(logEntry, deserializedLogEntry), Is.True);

            var serializedLogEntry = serializer.SerializeToString(logEntry);
            Assert.That(logFileContents.Length, Is.EqualTo(_xmlDeclaration.Length + Environment.NewLine.Length + serializedLogEntry.Length + Environment.NewLine.Length));
        }

        [Test]
        public async void IsThreadSafe()
        {
            const int tasksPerProcessor = 5;
            const int logEntriesPerTask = 5;

            var serializer = new XmlSerializerSerializer();

            var logProvider = CreateLogProvider(serializer, _logFilePath);

            var tasks = new Task[Environment.ProcessorCount * tasksPerProcessor];

            var logEntry = new LogEntry("Hello, world!", new { Foo = "bar" });

            Func<Task> getWriteLogEntriesTask =
                async () =>
                {
                    for (var i = 0; i < logEntriesPerTask; i++)
                    {
                        await logProvider.WriteAsync(logEntry);
                    }
                };

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = getWriteLogEntriesTask();
            }

            await Task.WhenAll(tasks);

            var logFileContents = File.ReadAllText(_logFilePath);

            var xmlDocuments =
                logFileContents.Split(
                    new[] { _xmlDeclaration + Environment.NewLine },
                    StringSplitOptions.RemoveEmptyEntries);

            // We are reasonably certain that we are thread-safe because we didn't lose
            // any log entries (also because we didn't throw an exception).
            Assert.That(xmlDocuments.Length, Is.EqualTo(Environment.ProcessorCount * logEntriesPerTask * tasksPerProcessor));
        }

        protected virtual ILogProvider CreateLogProvider(XmlSerializerSerializer serializer, string logFilePath)
        {
            return new FileLogProvider(
                new SerializingLogFormatterFactory(serializer),
                logFilePath);
        }
    }
}
