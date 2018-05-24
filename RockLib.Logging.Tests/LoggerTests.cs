using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.Tests
{
    public class LoggerTests
    {
        [Fact]
        public void NameIsSetFromConstructor()
        {
            var logger = new Logger("foo");

            logger.Name.Should().BeSameAs("foo");
        }

        [Fact]
        public void LevelIsSetFromConstructor()
        {
            var logger = new Logger(level: LogLevel.Error);

            logger.Level.Should().Be(LogLevel.Error);
        }

        [Fact]
        public void ProvidersIsSetFromConstructor()
        {
            var providers = new ILogProvider[0];

            var logger = new Logger(providers: providers);

            logger.Providers.Should().BeSameAs(providers);
        }

        [Fact]
        public void IsDisabledIsSetFromConstructor()
        {
            var logger = new Logger(isDisabled: true);

            logger.IsDisabled.Should().Be(true);
        }

        [Fact]
        public void LogThrowsArgumentNullExceptionIfLogEntryIsNull()
        {
            var logger = new Logger();

            Assert.Throws<ArgumentNullException>(() => logger.Log(null));
        }

        [Fact]
        public void LogThrowsObjectDisposedExceptionAfterDisposeMethodHasBeenCalled()
        {
            var logger = new Logger();

            logger.Dispose();

            Assert.Throws<ObjectDisposedException>(() => logger.Log(new LogEntry("Hello, world!", LogLevel.Info)));
        }

        [Fact]
        public void LogPassesTheLogEntryToEachLogProvider()
        {
            var logProviders = new[]
            {
                new SynchronousLogProvider(),
                new SynchronousLogProvider()
            };

            var logger = new Logger(providers: logProviders, level: LogLevel.Info);

            var logEntry = new LogEntry("Hello, world!", LogLevel.Info);

            logger.Log(logEntry);
            logger.Dispose();

            foreach (var logProvider in logProviders)
            {
                logProvider.SentLogEntries.Count.Should().Be(1);
                var sentLogEntry = logProvider.SentLogEntries[0];
                sentLogEntry.Should().BeSameAs(logEntry);
            }
        }

        [Fact]
        public void LogDoesNotPassLogEntriesToLogProvidersWhenIsDisabledIsTrue()
        {
            var logProviders = new[]
            {
                new SynchronousLogProvider(),
                new SynchronousLogProvider()
            };

            var logger = new Logger(providers: logProviders, level: LogLevel.Info, isDisabled: true);

            var logEntry = new LogEntry("Hello, world!", LogLevel.Info);

            logger.Log(logEntry);

            foreach (var logProvider in logProviders)
                logProvider.SentLogEntries.Count.Should().Be(0);
        }

        [Fact]
        public void LogDoesNotPassLogEntriesToLogProvidersWhenLevelIsHigherThanTheLogEntryLevel()
        {
            var logProviders = new[]
            {
                new SynchronousLogProvider(),
                new SynchronousLogProvider()
            };

            var logger = new Logger(providers: logProviders, level: LogLevel.Error);

            var logEntry = new LogEntry("Hello, world!", LogLevel.Info);

            logger.Log(logEntry);

            foreach (var logProvider in logProviders)
                logProvider.SentLogEntries.Count.Should().Be(0);
        }

        [Fact]
        public void LogDoesNotPassLogEntriesToLogProvidersWithALevelHigherThanTheLogEntryLevel()
        {
            var synchronousLogProvider = new SynchronousLogProvider();
            var auditLevelLogProvider = new AuditLevelLogProvider();

            var logProviders = new[]
            {
                synchronousLogProvider,
                auditLevelLogProvider,
            };

            var logger = new Logger(providers: logProviders, level: LogLevel.Info);

            var logEntry = new LogEntry("Hello, world!", LogLevel.Info);

            logger.Log(logEntry);
            logger.Dispose();

            synchronousLogProvider.SentLogEntries.Count.Should().Be(1);
            auditLevelLogProvider.SentLogEntries.Count.Should().Be(0);
        }

        [Fact]
        public void LogAddsCallerInfoExtendedPropertyToLogEntry()
        {
            var logProviders = new[]
            {
                new SynchronousLogProvider()
            };

            var logger = new Logger(providers: logProviders, level: LogLevel.Info);

            var logEntry = new LogEntry("Hello, world!", LogLevel.Info);

            logger.Log(logEntry);
            logger.Dispose();

            logEntry.ExtendedProperties.Should().ContainKey("CallerInfo");
        }

        [Fact]
        public void DisposeBlocksUntilLogEntriesAreSent()
        {
            var logProvider = new SemaphoreDelayedLogProvider();

            var logger = new Logger(providers: new[] { logProvider }, level: LogLevel.Info);

            var logEntry = new LogEntry("Hello, world!", LogLevel.Info);

            logger.Log(logEntry);

            var disposeThread = new Thread(() => logger.Dispose());
            disposeThread.Start();

            Thread.Sleep(100); // (we could wait all day here)

            logProvider.SentLogEntries.Count.Should().Be(0);

            logProvider.Semaphore.Release(); // Unblock the logProvider's WriteAsync method...
            disposeThread.Join(); // ...which unblocks the logger's Dispose method allowing the thread to complete.

            logProvider.SentLogEntries.Count.Should().Be(1);
        }

        private class SynchronousLogProvider : ILogProvider
        {
            public TimeSpan Timeout => TimeSpan.Zero;

            public virtual LogLevel Level => LogLevel.NotSet;

            public Task WriteAsync(LogEntry logEntry, CancellationToken cancellationToken)
            {
                SentLogEntries.Add(logEntry);
                return Task.CompletedTask;
            }

            public List<LogEntry> SentLogEntries { get; } = new List<LogEntry>();
        }

        private class AuditLevelLogProvider : SynchronousLogProvider
        {
            public override LogLevel Level => LogLevel.Audit;
        }

        private class SemaphoreDelayedLogProvider : ILogProvider
        {
            public TimeSpan Timeout => TimeSpan.FromSeconds(30);

            public LogLevel Level => LogLevel.NotSet;

            public List<LogEntry> SentLogEntries { get; } = new List<LogEntry>();

            public SemaphoreSlim Semaphore { get; } = new SemaphoreSlim(0, 1);

            public async Task WriteAsync(LogEntry logEntry, CancellationToken cancellationToken)
            {
                await Semaphore.WaitAsync(cancellationToken);
                SentLogEntries.Add(logEntry);
            }
        }
    }
}
