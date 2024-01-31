using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using RockLib.Dynamic;
using RockLib.Logging.LogProcessing;
using Xunit;

namespace RockLib.Logging.Tests.LogProcessingTests;

public static class LogProcessorTests
{
    [Fact]
    public static void IsDisposedIsFalseInitially()
    {
        using var logProcessor = new TestLogProcessor();

        logProcessor.IsDisposed.Should().Be(false);
    }

    [Fact]
    public static void DisposeSetsIsDisposedToTrue()
    {
        var logProcessor = new TestLogProcessor();

        logProcessor.Dispose();

        logProcessor.IsDisposed.Should().Be(true);
    }

    [Fact]
    public static void ProcessLogEntryCallsContextProvidersAddContextMethod()
    {
        using var logProcessor = new TestLogProcessor();

        var mockContextProvider1 = new Mock<IContextProvider>();
        var mockContextProvider2 = new Mock<IContextProvider>();
        var mockLogProvider = new Mock<ILogProvider>();

        using var logger = new Logger(logProcessor,
            logProviders: new[] { mockLogProvider.Object },
            contextProviders: new[] { mockContextProvider1.Object, mockContextProvider2.Object });

        var logEntry = new LogEntry();

        logProcessor.ProcessLogEntry(logger, logEntry);

        mockContextProvider1.Verify(m => m.AddContext(logEntry), Times.Once);
        mockContextProvider2.Verify(m => m.AddContext(logEntry), Times.Once);
    }

    [Fact]
    public static void ProcessLogEntryCallsSendToLogProviderForEachLogProvider()
    {
        using var logProcessor = new TestLogProcessor();

        var mockContextProvider = new Mock<IContextProvider>();
        var mockLogProvider1 = new Mock<ILogProvider>();
        var mockLogProvider2 = new Mock<ILogProvider>();

        using var logger = new Logger(logProcessor,
            logProviders: new[] { mockLogProvider1.Object, mockLogProvider2.Object },
            contextProviders: new[] { mockContextProvider.Object });

        var logEntry = new LogEntry();

        logProcessor.ProcessLogEntry(logger, logEntry);

        logProcessor.SendToLogProviderInvocations.Count.Should().Be(2);

        var invocation1 = logProcessor.SendToLogProviderInvocations[0];
        var invocation2 = logProcessor.SendToLogProviderInvocations[1];

        invocation1.LogProvider.Should().BeSameAs(mockLogProvider1.Object);
        invocation1.LogEntry.Should().BeSameAs(logEntry);
        invocation1.ErrorHandler.Should().BeSameAs(NullErrorHandler.Instance);
        invocation1.FailureCount.Should().Be(0);

        invocation2.LogProvider.Should().BeSameAs(mockLogProvider2.Object);
        invocation2.LogEntry.Should().BeSameAs(logEntry);
        invocation2.ErrorHandler.Should().BeSameAs(NullErrorHandler.Instance);
        invocation2.FailureCount.Should().Be(0);

        logProcessor.HandleErrorInvocations.Should().BeEmpty();
    }

    [Fact]
    public static void ProcessLogEntryDoesNotCallSendToLogProviderForLogProvidersWithALevelGreaterThanTheLogEntry()
    {
        using var logProcessor = new TestLogProcessor();

        var mockContextProvider = new Mock<IContextProvider>();
        var mockLogProvider1 = new Mock<ILogProvider>();
        var mockLogProvider2 = new Mock<ILogProvider>();

        mockLogProvider1.Setup(m => m.Level).Returns(LogLevel.Error);
        mockLogProvider2.Setup(m => m.Level).Returns(LogLevel.Info);

        using var logger = new Logger(logProcessor,
            logProviders: new[] { mockLogProvider1.Object, mockLogProvider2.Object },
            contextProviders: new[] { mockContextProvider.Object });

        var logEntry = new LogEntry() { Level = LogLevel.Info };

        logProcessor.ProcessLogEntry(logger, logEntry);

        logProcessor.SendToLogProviderInvocations.Count.Should().Be(1);

        var invocation = logProcessor.SendToLogProviderInvocations[0];

        invocation.LogProvider.Should().BeSameAs(mockLogProvider2.Object);
        invocation.LogEntry.Should().BeSameAs(logEntry);
        invocation.ErrorHandler.Should().BeSameAs(NullErrorHandler.Instance);
        invocation.FailureCount.Should().Be(0);

        logProcessor.HandleErrorInvocations.Should().BeEmpty();
    }

    [Fact]
    public static void IfSendToLogProviderThrowsHandleErrorIsCalled()
    {
        using var logProcessor = new TestLogProcessor(sendToLogProviderShouldThrow: true);

        var mockContextProvider = new Mock<IContextProvider>();
        var mockLogProvider = new Mock<ILogProvider>();

        using var logger = new Logger(logProcessor,
            logProviders: new[] { mockLogProvider.Object },
            contextProviders: new[] { mockContextProvider.Object });

        var logEntry = new LogEntry();

        logProcessor.ProcessLogEntry(logger, logEntry);

        logProcessor.HandleErrorInvocations.Count.Should().Be(1);

        var invocation = logProcessor.HandleErrorInvocations[0];

        invocation.Exception!.Message.Should().Be("error.");
        invocation.LogProvider.Should().BeSameAs(mockLogProvider.Object);
        invocation.LogEntry.Should().BeSameAs(logEntry);
        invocation.ErrorHandler.Should().BeSameAs(NullErrorHandler.Instance);
        invocation.FailureCount.Should().Be(1);
    }

    [Fact]
    public static void HandleErrorInvokesErrorHandlerCallbackWhenProvided()
    {
        Error? capturedError = null;

        var errorHandler = DelegateErrorHandler.New(error =>
        {
            capturedError = error;
        });

        using var logProcessor = new TestLogProcessor();

        var exception = new NotSupportedException();
        var logProvider = new Mock<ILogProvider>().Object;
        var logEntry = new LogEntry();

        logProcessor.Unlock().HandleError(exception, logProvider, logEntry, errorHandler, 321, "Oops: {0}", new object[] { 123 });

        capturedError.Should().NotBeNull();
        capturedError!.Exception.Should().BeSameAs(exception);
        capturedError.LogProvider.Should().BeSameAs(logProvider);
        capturedError.LogEntry.Should().BeSameAs(logEntry);
        capturedError.FailureCount.Should().Be(321);
        capturedError.Message.Should().Be("Oops: 123");
    }

    [Fact]
    public static void IfErrorHandlerSetsShouldRetryToTrueSendToLogProviderIsCalled()
    {
        var errorHandler = DelegateErrorHandler.New(error =>
        {
            if (error.FailureCount < 2)
            {
                error.ShouldRetry = true;
            }
        });

        using var logProcessor = new TestLogProcessor();

        var exception = new NotSupportedException();
        var logProvider = new Mock<ILogProvider>().Object;
        var logEntry = new LogEntry();

        logProcessor.Unlock().HandleError(exception, logProvider, logEntry, errorHandler, 1, "Oops: {0}", new object[] { 123 });

        logProcessor.SendToLogProviderInvocations.Count.Should().Be(1);

        var sendToLogProviderInvocation = logProcessor.SendToLogProviderInvocations[0];

        sendToLogProviderInvocation.LogProvider.Should().BeSameAs(logProvider);
        sendToLogProviderInvocation.LogEntry.Should().BeSameAs(logEntry);
        sendToLogProviderInvocation.ErrorHandler.Should().BeSameAs(errorHandler);
        sendToLogProviderInvocation.FailureCount.Should().Be(1);
    }

    [Fact]
    public static void IfRetriedSendToLogProviderThrowsHandleErrorIsCalled()
    {
        var errorHandler = DelegateErrorHandler.New(error =>
        {
            if (error.FailureCount < 2)
                error.ShouldRetry = true;
        });

        using var logProcessor = new TestLogProcessor(sendToLogProviderShouldThrow: true);

        var exception = new NotSupportedException();
        var logProvider = new Mock<ILogProvider>().Object;
        var logEntry = new LogEntry();

        logProcessor.Unlock().HandleError(exception, logProvider, logEntry, errorHandler, 1, "Oops: {0}", new object[] { 123 });

        logProcessor.HandleErrorInvocations.Count.Should().Be(2);

        var invocation1 = logProcessor.HandleErrorInvocations[0];
        var invocation2 = logProcessor.HandleErrorInvocations[1];

        // Original HandleError call
        invocation1.Exception.Should().BeSameAs(exception);
        invocation1.LogProvider.Should().BeSameAs(logProvider);
        invocation1.LogEntry.Should().BeSameAs(logEntry);
        invocation1.FailureCount.Should().Be(1);
        invocation1.ErrorMessageFormat.Should().Be("Oops: {0}");

        // Resend HandleError call
        invocation2.Exception.Should().NotBeSameAs(exception);
        invocation2.Exception!.Message.Should().Be("error.");
        invocation2.LogProvider.Should().BeSameAs(logProvider);
        invocation2.LogEntry.Should().BeSameAs(logEntry);
        invocation2.FailureCount.Should().Be(2);
        invocation2.ErrorMessageFormat.Should().Be("Error while re-sending log entry {0} to log provider {1}.");
    }

    private sealed class TestLogProcessor : LogProcessor
    {
        private readonly bool _sendToLogProviderShouldThrow;

        public TestLogProcessor(bool sendToLogProviderShouldThrow = false) => _sendToLogProviderShouldThrow = sendToLogProviderShouldThrow;

        public List<(ILogProvider LogProvider, LogEntry LogEntry, IErrorHandler ErrorHandler, int FailureCount)> SendToLogProviderInvocations { get; } =  new();
        public List<(Exception? Exception, ILogProvider LogProvider, LogEntry LogEntry, IErrorHandler ErrorHandler, int FailureCount, string ErrorMessageFormat, object[] ErrorMessageArgs)> HandleErrorInvocations { get; } = new();

        protected override void SendToLogProvider(ILogProvider logProvider, LogEntry logEntry, IErrorHandler errorHandler, int failureCount)
        {
            SendToLogProviderInvocations.Add((logProvider, logEntry, errorHandler, failureCount));
            if (_sendToLogProviderShouldThrow)
            {
                throw new NotSupportedException("error.");
            }
        }

        protected override void HandleError(Exception? exception, ILogProvider logProvider, LogEntry logEntry, 
            IErrorHandler errorHandler, int failureCount, string errorMessageFormat, params object[] errorMessageArgs)
        {
            HandleErrorInvocations.Add((exception, logProvider, logEntry, errorHandler, failureCount, errorMessageFormat, errorMessageArgs));
            base.HandleError(exception, logProvider, logEntry, errorHandler, failureCount, errorMessageFormat, errorMessageArgs);
        }
    }
}
