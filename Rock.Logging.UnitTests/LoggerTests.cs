using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Rock.DependencyInjection.AutoMock.Moq;
using Rock.Logging;

// ReSharper disable once CheckNamespace
namespace LoggerTests
{
    public abstract class LoggerTestsBase
    {
        protected AutoMocker _mocker;

        [SetUp]
        public void Setup()
        {
            _mocker = new AutoMocker();
        }

        protected Logger GetLogger()
        {
            return _mocker.Get<Logger>();
        }

        public class TheIsEnabledMethod : LoggerTestsBase
        {
            private Mock<ILogProvider> _mockLogProvider;

            [SetUp]
            public void Setup()
            {
                _mockLogProvider = new Mock<ILogProvider>();
            }

            [TestCase(LogLevel.Debug, LogLevel.Debug)]
            [TestCase(LogLevel.Info, LogLevel.Debug)]
            [TestCase(LogLevel.Info, LogLevel.Info)]
            [TestCase(LogLevel.Warn, LogLevel.Debug)]
            [TestCase(LogLevel.Warn, LogLevel.Info)]
            [TestCase(LogLevel.Warn, LogLevel.Warn)]
            [TestCase(LogLevel.Error, LogLevel.Debug)]
            [TestCase(LogLevel.Error, LogLevel.Info)]
            [TestCase(LogLevel.Error, LogLevel.Warn)]
            [TestCase(LogLevel.Error, LogLevel.Error)]
            [TestCase(LogLevel.Fatal, LogLevel.Debug)]
            [TestCase(LogLevel.Fatal, LogLevel.Info)]
            [TestCase(LogLevel.Fatal, LogLevel.Warn)]
            [TestCase(LogLevel.Fatal, LogLevel.Error)]
            [TestCase(LogLevel.Fatal, LogLevel.Fatal)]
            public void ReturnsTrueWhenTheLogLevelParameterIsGreaterThanOrEqualToTheConfiguredLogLevel(LogLevel logLevelParameter, LogLevel configuredLogLevel)
            {
                RunTest(logLevelParameter, configuredLogLevel, true, true);
            }

            [TestCase(LogLevel.Debug, LogLevel.Info)]
            [TestCase(LogLevel.Debug, LogLevel.Warn)]
            [TestCase(LogLevel.Debug, LogLevel.Error)]
            [TestCase(LogLevel.Debug, LogLevel.Fatal)]
            [TestCase(LogLevel.Debug, LogLevel.Audit)]
            [TestCase(LogLevel.Info, LogLevel.Warn)]
            [TestCase(LogLevel.Info, LogLevel.Error)]
            [TestCase(LogLevel.Info, LogLevel.Fatal)]
            [TestCase(LogLevel.Info, LogLevel.Audit)]
            [TestCase(LogLevel.Warn, LogLevel.Error)]
            [TestCase(LogLevel.Warn, LogLevel.Fatal)]
            [TestCase(LogLevel.Warn, LogLevel.Audit)]
            [TestCase(LogLevel.Error, LogLevel.Fatal)]
            [TestCase(LogLevel.Error, LogLevel.Audit)]
            [TestCase(LogLevel.Fatal, LogLevel.Audit)]
            public void ReturnsFalseWhenTheLogLevelParameterIsLessThanTheConfiguredLogLevel(LogLevel logLevelParameter, LogLevel configuredLogLevel)
            {
                RunTest(logLevelParameter, configuredLogLevel, true, false);
            }

            [TestCase(LogLevel.None, LogLevel.None)]
            [TestCase(LogLevel.None, LogLevel.Debug)]
            [TestCase(LogLevel.None, LogLevel.Info)]
            [TestCase(LogLevel.None, LogLevel.Warn)]
            [TestCase(LogLevel.None, LogLevel.Error)]
            [TestCase(LogLevel.None, LogLevel.Fatal)]
            [TestCase(LogLevel.None, LogLevel.Audit)]
            public void AlwaysReturnsFalseWhenTheLogLevelParameterIsNone(LogLevel logLevelParameter, LogLevel configuredLogLevel)
            {
                RunTest(logLevelParameter, configuredLogLevel, true, false);
            }

            [TestCase(LogLevel.Audit, LogLevel.None)]
            [TestCase(LogLevel.Audit, LogLevel.Debug)]
            [TestCase(LogLevel.Audit, LogLevel.Info)]
            [TestCase(LogLevel.Audit, LogLevel.Warn)]
            [TestCase(LogLevel.Audit, LogLevel.Error)]
            [TestCase(LogLevel.Audit, LogLevel.Fatal)]
            [TestCase(LogLevel.Audit, LogLevel.Audit)]
            public void AlwaysReturnsTrueWhenTheLogLevelParameterIsAudit(LogLevel logLevelParameter, LogLevel configuredLogLevel)
            {
                RunTest(logLevelParameter, configuredLogLevel, true, true);
            }

            [TestCase(LogLevel.Debug, LogLevel.Debug)]
            [TestCase(LogLevel.Info, LogLevel.Debug)]
            [TestCase(LogLevel.Info, LogLevel.Info)]
            [TestCase(LogLevel.Warn, LogLevel.Debug)]
            [TestCase(LogLevel.Warn, LogLevel.Info)]
            [TestCase(LogLevel.Warn, LogLevel.Warn)]
            [TestCase(LogLevel.Error, LogLevel.Debug)]
            [TestCase(LogLevel.Error, LogLevel.Info)]
            [TestCase(LogLevel.Error, LogLevel.Warn)]
            [TestCase(LogLevel.Error, LogLevel.Error)]
            [TestCase(LogLevel.Fatal, LogLevel.Debug)]
            [TestCase(LogLevel.Fatal, LogLevel.Info)]
            [TestCase(LogLevel.Fatal, LogLevel.Warn)]
            [TestCase(LogLevel.Fatal, LogLevel.Error)]
            [TestCase(LogLevel.Fatal, LogLevel.Fatal)]

            [TestCase(LogLevel.Audit, LogLevel.None)]
            [TestCase(LogLevel.Audit, LogLevel.Debug)]
            [TestCase(LogLevel.Audit, LogLevel.Info)]
            [TestCase(LogLevel.Audit, LogLevel.Warn)]
            [TestCase(LogLevel.Audit, LogLevel.Error)]
            [TestCase(LogLevel.Audit, LogLevel.Fatal)]
            [TestCase(LogLevel.Audit, LogLevel.Audit)]
            public void AlwaysReturnFalseWhenIsLoggingEnabledIsFalse(LogLevel logLevelParameter, LogLevel configuredLogLevel)
            {
                RunTest(logLevelParameter, configuredLogLevel, false, false);
            }

            private void RunTest(LogLevel logLevelParameter, LogLevel configuredLogLevel, bool configuredIsLoggingEnabled, bool expected)
            {
                _mocker.GetMock<IEnumerable<ILogProvider>>()
                    .Setup(m => m.GetEnumerator())
                    .Returns(GetMockLogProviders());

                _mocker.GetMock<ILoggerConfiguration>()
                    .Setup(m => m.LoggingLevel)
                    .Returns(configuredLogLevel);

                _mocker.GetMock<ILoggerConfiguration>()
                    .Setup(m => m.IsLoggingEnabled)
                    .Returns(configuredIsLoggingEnabled);

                var logger = _mocker.Get<Logger>();

                var result = logger.IsEnabled(logLevelParameter);

                Assert.That(result, Is.EqualTo(expected));
            }

            private IEnumerator<ILogProvider> GetMockLogProviders()
            {
                yield return _mockLogProvider.Object;
            }
        }

        public class TheLogMethod
        {
            
        }

        public class TheHandleExceptionMethod : LoggerTestsBase
        {
            private Mock<ILogProvider> _mockLogProvider;

            [SetUp]
            public void Setup()
            {
                _mockLogProvider = new Mock<ILogProvider>();

                _mocker.GetMock<IEnumerable<ILogProvider>>()
                    .Setup(m => m.GetEnumerator())
                    .Returns(GetMockLogProviders());

                _mocker.GetMock<ILoggerConfiguration>()
                    .Setup(m => m.IsLoggingEnabled)
                    .Returns(true);

                _mocker.GetMock<ILoggerConfiguration>()
                    .Setup(m => m.LoggingLevel)
                    .Returns(LogLevel.Debug);

                _mocker.GetMock<IThrottlingRuleEvaluator>()
                    .Setup(m => m.ShouldLog(It.IsAny<LogEntry>()))
                    .Returns(true);
            }

            private IEnumerator<ILogProvider> GetMockLogProviders()
            {
                yield return _mockLogProvider.Object;
            }

            [Test]
            public void CallsTheLogMethodWithALogEntryContainingTheException()
            {
                var exception = new Exception();

                var testingLogger = new TestingLogger(GetLogger());
                ILogger logger = testingLogger;

                logger.HandleException(exception);

                Assert.That(testingLogger.LogEntries.Count, Is.EqualTo(1));
                Assert.That(testingLogger.LogEntries[0].Exception, Is.SameAs(exception));
            }

            [Test]
            public void CallsTheLogMethodWithALogEntryWithALogLevelOfError()
            {
                var exception = new Exception();

                var testingLogger = new TestingLogger(GetLogger());
                ILogger logger = testingLogger;

                logger.HandleException(exception);

                Assert.That(testingLogger.LogEntries.Count, Is.EqualTo(1));
                Assert.That(testingLogger.LogEntries[0].LogLevel, Is.EqualTo(LogLevel.Error));
            }

            private class TestingLogger : Logger
            {
                private readonly List<LogEntry> _logEntries = new List<LogEntry>();

                public TestingLogger(Logger logger)
                    : base(logger)
                {
                }

                protected override void OnPreLog(LogEntry logEntry)
                {
                    _logEntries.Add(logEntry);
                }

                public IReadOnlyList<LogEntry> LogEntries
                {
                    get { return _logEntries.AsReadOnly(); }
                }
            }
        }
    }
}