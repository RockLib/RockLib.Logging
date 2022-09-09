using FluentAssertions;
using Moq;
using RockLib.Dynamic;
using RockLib.Logging.LogProcessing;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.Tests.LogProcessingTests;

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
            logProcessor.ProcessLogEntry(logger, logEntry);
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

        logProcessor.ProcessLogEntry(logger, logEntry);

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

        Error capturedError = null;
        var waitHandle = new AutoResetEvent(false);

        var errorHandler = DelegateErrorHandler.New(error =>
        {
            capturedError = error;
            waitHandle.Set();
        });

        logProcessor.Unlock().SendToLogProvider(mockLogProvider.Object, logEntry, errorHandler, 1);

        waitHandle.WaitOne(10000).Should().BeTrue();

        capturedError.Should().NotBeNull();
        capturedError.IsTimeout.Should().BeTrue();
        capturedError.LogProvider.Should().BeSameAs(mockLogProvider.Object);
        capturedError.LogEntry.Should().BeSameAs(logEntry);
        capturedError.FailureCount.Should().Be(2);
    }

    [Fact]
    public void IfWriteAsyncThrowsWhileAwaitingHandleErrorIsCalled()
    {
        var logProcessor = new BackgroundLogProcessor();

        var logProvider = new FakeLogProvider();
        var logEntry = new LogEntry();

        Error capturedError = null;

        var errorHandler = DelegateErrorHandler.New(error =>
        {
            capturedError = error;
        });

        logProcessor.Unlock().SendToLogProvider(logProvider, logEntry, errorHandler, 1);

        logProcessor.Dispose();

        capturedError.Should().NotBeNull();
        capturedError.Exception.Message.Should().Be("oh, no.");
        capturedError.LogProvider.Should().BeSameAs(logProvider);
        capturedError.LogEntry.Should().BeSameAs(logEntry);
        capturedError.FailureCount.Should().Be(2);
    }
}
