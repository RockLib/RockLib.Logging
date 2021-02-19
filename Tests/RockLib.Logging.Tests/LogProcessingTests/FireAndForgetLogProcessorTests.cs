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

            Error capturedError = null;
            var waitHandle = new AutoResetEvent(false);

            IErrorHandler errorHandler = DelegateErrorHandler.New(error =>
            {
                capturedError = error;
                waitHandle.Set();
            });

            logProcessor.Unlock().SendToLogProvider(logProvider, logEntry, errorHandler, 1);

            waitHandle.WaitOne(10000).Should().BeTrue();

            capturedError.Should().NotBeNull();
            capturedError.Exception.Message.Should().Be("oh, no.");
            capturedError.LogProvider.Should().BeSameAs(logProvider);
            capturedError.LogEntry.Should().BeSameAs(logEntry);
            capturedError.FailureCount.Should().Be(2);
        }
    }
}
