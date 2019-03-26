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
    public class FireAndForgetLogProcessorTests
    {
        [Fact]
        public void ProcessLogEntryCallsWriteAsyncOnTheLogProvider()
        {
            var logProcessor = new FireAndForgetLogProcessor();

            var mockLogProvider = new Mock<ILogProvider>();
            var logEntry = new LogEntry();

            mockLogProvider.Setup(m => m.WriteAsync(It.IsAny<LogEntry>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(0));

            logProcessor.Unlock().SendToLogProvider(mockLogProvider.Object, logEntry, null, 1);

            mockLogProvider.Verify(m => m.WriteAsync(logEntry, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void IfWriteAsyncThrowsWhileAwaitingHandleErrorIsCalled()
        {
            var logProcessor = new FireAndForgetLogProcessor();

            var logProvider = new FakeLogProvider();
            var logEntry = new LogEntry();

            ErrorEventArgs capturedArgs = null;
            var waitHandle = new AutoResetEvent(false);

            Action<ErrorEventArgs> handleError = args =>
            {
                capturedArgs = args;
                waitHandle.Set();
            };

            logProcessor.Unlock().SendToLogProvider(logProvider, logEntry, handleError, 1);

            waitHandle.WaitOne(10000).Should().BeTrue();

            capturedArgs.Should().NotBeNull();
            capturedArgs.Exception.Message.Should().Be("oh, no.");
            capturedArgs.LogProvider.Should().BeSameAs(logProvider);
            capturedArgs.LogEntry.Should().BeSameAs(logEntry);
            capturedArgs.FailureCount.Should().Be(2);
        }
    }
}
