using NUnit.Framework;
using Rock.DependencyInjection.AutoMock.Moq;

namespace Rock.Logging.UnitTests
{
    public class LoggerTests
    {
        public class TheIsEnabledMethod
        {
            private AutoMocker _mocker;

            [SetUp]
            public void Setup()
            {
                _mocker = new AutoMocker();
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
                RunTest(logLevelParameter, configuredLogLevel, true);
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
                RunTest(logLevelParameter, configuredLogLevel, false);
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
                RunTest(logLevelParameter, configuredLogLevel, false);
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
                RunTest(logLevelParameter, configuredLogLevel, true);
            }

            private void RunTest(LogLevel logLevelParameter, LogLevel configuredLogLevel, bool expected)
            {
                _mocker.GetMock<ILoggerConfiguration>()
                    .Setup(m => m.LogLevel)
                    .Returns(configuredLogLevel);

                var logger = _mocker.Get<Logger>();

                var result = logger.IsEnabled(logLevelParameter);

                Assert.That(result, Is.EqualTo(expected));
            }
        }
    }
}