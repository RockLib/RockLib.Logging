using FluentAssertions;
using Moq;
using RockLib.Dynamic;
using RockLib.Logging.LogProcessing;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.Tests.LogProcessingTests
{
    public class BackgroundLogProcessorTests
    {
        [Fact]
        public void ProcessLogEntryCallsWriteAsyncOnTheLogProvider()
        {
            var logProcessor = new BackgroundLogProcessor();

            var mockContextProvider = new Mock<IContextProvider>();
            var mockLogProvider = new Mock<ILogProvider>();

            var logger = new Logger(logProcessor,
                logProviders: new[] { mockLogProvider.Object },
                contextProviders: new[] { mockContextProvider.Object });

            var logEntry = new LogEntry();

            try
            {
                logProcessor.ProcessLogEntry(logger, logEntry, null);
            }
            finally
            {
                logProcessor.Dispose();
            }

            mockContextProvider.Verify(m => m.AddContext(logEntry), Times.Once);
            mockLogProvider.Verify(m => m.WriteAsync(logEntry, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void ProcessLogEntryDoesNothingWhenIsDisposedIsTrue()
        {
            var logProcessor = new BackgroundLogProcessor();

            var mockContextProvider = new Mock<IContextProvider>();
            var mockLogProvider = new Mock<ILogProvider>();

            var logger = new Logger(logProcessor,
                logProviders: new[] { mockLogProvider.Object },
                contextProviders: new[] { mockContextProvider.Object });

            var logEntry = new LogEntry();

            logProcessor.Dispose();

            logProcessor.ProcessLogEntry(logger, logEntry, null);

            mockContextProvider.Verify(m => m.AddContext(It.IsAny<LogEntry>()), Times.Never);
            mockLogProvider.Verify(m => m.WriteAsync(It.IsAny<LogEntry>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void IfWriteAsyncTimesOutHandleErrorIsCalled()
        {
            var logProcessor = new BackgroundLogProcessor();

            var mockLogProvider = new Mock<ILogProvider>();
            var logEntry = new LogEntry();

            mockLogProvider.Setup(m => m.WriteAsync(It.IsAny<LogEntry>(), It.IsAny<CancellationToken>())).Returns(() => Task.Delay(200));
            mockLogProvider.Setup(m => m.Timeout).Returns(TimeSpan.FromMilliseconds(10));

            ErrorEventArgs capturedArgs = null;
            var waitHandle = new AutoResetEvent(false);

            Action<ErrorEventArgs> handleError = args =>
            {
                capturedArgs = args;
                waitHandle.Set();
            };

            logProcessor.Unlock().SendToLogProvider(mockLogProvider.Object, logEntry, handleError, 1);

            waitHandle.WaitOne(2000).Should().BeTrue();

            capturedArgs.Should().NotBeNull();
            capturedArgs.IsTimeout.Should().BeTrue();
            capturedArgs.LogProvider.Should().BeSameAs(mockLogProvider.Object);
            capturedArgs.LogEntry.Should().BeSameAs(logEntry);
            capturedArgs.FailureCount.Should().Be(2);
        }

        [Fact]
        public void IfWriteAsyncThrowsWhileAwaitingHandleErrorIsCalled()
        {
            var logProcessor = new BackgroundLogProcessor();

            var logProvider = new FakeLogProvider();
            var logEntry = new LogEntry();

            ErrorEventArgs capturedArgs = null;

            Action<ErrorEventArgs> handleError = args =>
            {
                capturedArgs = args;
            };

            logProcessor.Unlock().SendToLogProvider(logProvider, logEntry, handleError, 1);

            logProcessor.Dispose();

            capturedArgs.Should().NotBeNull();
            capturedArgs.Exception.Message.Should().Be("oh, no.");
            capturedArgs.LogProvider.Should().BeSameAs(logProvider);
            capturedArgs.LogEntry.Should().BeSameAs(logEntry);
            capturedArgs.FailureCount.Should().Be(2);
        }
    }
}
