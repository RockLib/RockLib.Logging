using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Moq;
using RockLib.Dynamic;
using RockLib.Logging.LogProcessing;
using Xunit;

namespace RockLib.Logging.Tests.LogProcessingTests
{
    public class LogProcessorTests
    {
        [Fact]
        public void IsDisposedIsFalseInitially()
        {
            var logProcessor = new TestLogProcessor();

            logProcessor.IsDisposed.Should().Be(false);
        }

        [Fact]
        public void DisposeSetsIsDisposedToTrue()
        {
            var logProcessor = new TestLogProcessor();

            logProcessor.Dispose();

            logProcessor.IsDisposed.Should().Be(true);
        }

        [Fact]
        public void ProcessLogEntryCallContextProvidersAddContextMethod()
        {
            var logProcessor = new TestLogProcessor();

            var mockContextProvider1 = new Mock<IContextProvider>();
            var mockContextProvider2 = new Mock<IContextProvider>();
            var mockLogProvider = new Mock<ILogProvider>();

            var logger = new Logger(logProcessor,
                logProviders: new[] { mockLogProvider.Object },
                contextProviders: new[] { mockContextProvider1.Object, mockContextProvider2.Object });

            var logEntry = new LogEntry();

            logProcessor.ProcessLogEntry(logger, logEntry, null);

            mockContextProvider1.Verify(m => m.AddContext(logEntry), Times.Once);
            mockContextProvider2.Verify(m => m.AddContext(logEntry), Times.Once);
        }

        [Fact]
        public void ProcessLogEntryCallsSendToLogProviderForEachLogProvider()
        {
            var logProcessor = new TestLogProcessor();

            var mockContextProvider = new Mock<IContextProvider>();
            var mockLogProvider1 = new Mock<ILogProvider>();
            var mockLogProvider2 = new Mock<ILogProvider>();

            var logger = new Logger(logProcessor,
                logProviders: new[] { mockLogProvider1.Object, mockLogProvider2.Object },
                contextProviders: new[] { mockContextProvider.Object });

            var logEntry = new LogEntry();

            logProcessor.ProcessLogEntry(logger, logEntry, null);

            logProcessor.SendToLogProviderInvocations.Count.Should().Be(2);

            var invocation1 = logProcessor.SendToLogProviderInvocations[0];
            var invocation2 = logProcessor.SendToLogProviderInvocations[1];

            invocation1.LogProvider.Should().BeSameAs(mockLogProvider1.Object);
            invocation1.LogEntry.Should().BeSameAs(logEntry);
            invocation1.ErrorHandler.Should().BeNull();
            invocation1.FailureCount.Should().Be(0);

            invocation2.LogProvider.Should().BeSameAs(mockLogProvider2.Object);
            invocation2.LogEntry.Should().BeSameAs(logEntry);
            invocation2.ErrorHandler.Should().BeNull();
            invocation2.FailureCount.Should().Be(0);

            logProcessor.HandleErrorInvocations.Should().BeEmpty();
        }

        [Fact]
        public void ProcessLogEntryDoesNotCallSendToLogProviderForLogProvidersWithALevelGreaterThanTheLogEntry()
        {
            var logProcessor = new TestLogProcessor();

            var mockContextProvider = new Mock<IContextProvider>();
            var mockLogProvider1 = new Mock<ILogProvider>();
            var mockLogProvider2 = new Mock<ILogProvider>();

            mockLogProvider1.Setup(m => m.Level).Returns(LogLevel.Error);
            mockLogProvider2.Setup(m => m.Level).Returns(LogLevel.Info);

            var logger = new Logger(logProcessor,
                logProviders: new[] { mockLogProvider1.Object, mockLogProvider2.Object },
                contextProviders: new[] { mockContextProvider.Object });

            var logEntry = new LogEntry() { Level = LogLevel.Info };

            logProcessor.ProcessLogEntry(logger, logEntry, null);

            logProcessor.SendToLogProviderInvocations.Count.Should().Be(1);

            var invocation = logProcessor.SendToLogProviderInvocations[0];

            invocation.LogProvider.Should().BeSameAs(mockLogProvider2.Object);
            invocation.LogEntry.Should().BeSameAs(logEntry);
            invocation.ErrorHandler.Should().BeNull();
            invocation.FailureCount.Should().Be(0);

            logProcessor.HandleErrorInvocations.Should().BeEmpty();
        }

        [Fact]
        public void IfSendToLogProviderThrowsHandleErrorIsCalled()
        {
            var logProcessor = new TestLogProcessor(sendToLogProviderShouldThrow: true);

            var mockContextProvider = new Mock<IContextProvider>();
            var mockLogProvider = new Mock<ILogProvider>();

            var logger = new Logger(logProcessor,
                logProviders: new[] { mockLogProvider.Object },
                contextProviders: new[] { mockContextProvider.Object });

            var logEntry = new LogEntry();

            logProcessor.ProcessLogEntry(logger, logEntry, null);

            logProcessor.HandleErrorInvocations.Count.Should().Be(1);

            var invocation = logProcessor.HandleErrorInvocations[0];

            invocation.Exception.Message.Should().Be("error.");
            invocation.LogProvider.Should().BeSameAs(mockLogProvider.Object);
            invocation.LogEntry.Should().BeSameAs(logEntry);
            invocation.ErrorHandler.Should().BeNull();
            invocation.FailureCount.Should().Be(1);
        }

        [Fact]
        public void HandleErrorInvokesErrorHandlerCallbackWhenProvided()
        {
            ErrorEventArgs capturedArgs = null;

            Action<ErrorEventArgs> errorHandler = args =>
            {
                capturedArgs = args;
            };

            var logProcessor = new TestLogProcessor();

            var exception = new Exception();
            var logProvider = new Mock<ILogProvider>().Object;
            var logEntry = new LogEntry();

            logProcessor.Unlock().HandleError(exception, logProvider, logEntry, errorHandler, 321, "Oops: {0}", new object[] { 123 });

            capturedArgs.Should().NotBeNull();
            capturedArgs.Exception.Should().BeSameAs(exception);
            capturedArgs.LogProvider.Should().BeSameAs(logProvider);
            capturedArgs.LogEntry.Should().BeSameAs(logEntry);
            capturedArgs.FailureCount.Should().Be(321);
            capturedArgs.Message.Should().Be("Oops: 123");
        }

        [Fact]
        public void IfErrorHandlerSetsShouldRetryToTrueSendToLogProviderIsCalled()
        {
            Action<ErrorEventArgs> errorHandler = args =>
            {
                if (args.FailureCount < 2)
                    args.ShouldRetry = true;
            };

            var logProcessor = new TestLogProcessor();

            var exception = new Exception();
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
        public void IfRetriedSendToLogProviderThrowsHandleErrorIsCalled()
        {
            Action<ErrorEventArgs> errorHandler = args =>
            {
                if (args.FailureCount < 2)
                    args.ShouldRetry = true;
            };

            var logProcessor = new TestLogProcessor(sendToLogProviderShouldThrow: true);

            var exception = new Exception();
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
            invocation2.Exception.Message.Should().Be("error.");
            invocation2.LogProvider.Should().BeSameAs(logProvider);
            invocation2.LogEntry.Should().BeSameAs(logEntry);
            invocation2.FailureCount.Should().Be(2);
            invocation2.ErrorMessageFormat.Should().Be("Error while re-sending log entry {0} to log provider {1}.");
        }

        private class TestLogProcessor : LogProcessor
        {
            private readonly bool _sendToLogProviderShouldThrow;

            public TestLogProcessor(bool sendToLogProviderShouldThrow = false)
            {
                _sendToLogProviderShouldThrow = sendToLogProviderShouldThrow;
            }

            public List<(ILogProvider LogProvider, LogEntry LogEntry, Action<ErrorEventArgs> ErrorHandler, int FailureCount)> SendToLogProviderInvocations { get; } = new List<(ILogProvider LogProvider, LogEntry LogEntry, Action<ErrorEventArgs> ErrorHandler, int FailureCount)>();
            public List<(Exception Exception, ILogProvider LogProvider, LogEntry LogEntry, Action<ErrorEventArgs> ErrorHandler, int FailureCount, string ErrorMessageFormat, object[] ErrorMessageArgs)> HandleErrorInvocations { get; } = new List<(Exception Exception, ILogProvider LogProvider, LogEntry LogEntry, Action<ErrorEventArgs> ErrorHandler, int FailureCount, string ErrorMessageFormat, object[] ErrorMessageArgs)>();

            protected override void SendToLogProvider(ILogProvider logProvider, LogEntry logEntry, Action<ErrorEventArgs> errorHandler, int failureCount)
            {
                SendToLogProviderInvocations.Add((logProvider, logEntry, errorHandler, failureCount));
                if (_sendToLogProviderShouldThrow)
                    throw new Exception("error.");
            }

            protected override void HandleError(Exception exception, ILogProvider logProvider, LogEntry logEntry, Action<ErrorEventArgs> errorHandler, int failureCount, string errorMessageFormat, params object[] errorMessageArgs)
            {
                HandleErrorInvocations.Add((exception, logProvider, logEntry, errorHandler, failureCount, errorMessageFormat, errorMessageArgs));
                base.HandleError(exception, logProvider, logEntry, errorHandler, failureCount, errorMessageFormat, errorMessageArgs);
            }
        }
    }
}
