using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using MSE = Microsoft.Extensions.Logging;

namespace RockLib.Logging.AspNetCore.Tests
{
    public class RockLibLoggerTests
    {
        [Fact]
        public void LogThrowsWithNullFormatter()
        {
            var loggerMock = new Mock<ILogger>();

            var rlLogger = new RockLibLogger(loggerMock.Object, null);

            Action action = () => 
                rlLogger.Log(MSE.LogLevel.None, new EventId(1), new Dictionary<string, string>(), null, null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: formatter");
        }

        [Fact]
        public void LogSendsWithPropertiesAndExtendedProperties()
        {
            LogEntry logEntry = null;
            var exception = new Exception("Some random exception");
            var eventId = new EventId(15, "SomeEvent");
            var state = new Dictionary<string, string>();

            var loggerMock = new Mock<ILogger>();
            loggerMock.Setup(lm => lm.Level).Returns(LogLevel.Debug);
            loggerMock
                .Setup(lm => lm.Log(It.IsAny<LogEntry>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Callback<LogEntry, string, string, int>((le, s1, s2, i) => logEntry = le);

            var rlLogger = new RockLibLogger(loggerMock.Object, "SomeCategory");
            rlLogger.Log(MSE.LogLevel.Debug, eventId, state, exception, (dictionary, ex) => "Simple message");

            logEntry.Should().NotBeNull();
            logEntry.Level.Should().Be(LogLevel.Debug);
            logEntry.Message.Should().Be("Simple message");
            logEntry.Exception.Should().BeSameAs(exception);

            logEntry.ExtendedProperties["Microsoft.Extensions.Logging.EventId"].Should().Be(eventId);
            logEntry.ExtendedProperties["Microsoft.Extensions.Logging.State"].Should().Be(state);
            logEntry.ExtendedProperties["Microsoft.Extensions.Logging.CategoryName"].Should().Be("SomeCategory");
        }

        [Fact]
        public void LogDoesNothingWhenDisabled()
        {
            var loggerMock = new Mock<ILogger>();
            loggerMock.Setup(lm => lm.IsDisabled).Returns(true);

            var rlLogger = new RockLibLogger(loggerMock.Object, null);

            rlLogger.Log(MSE.LogLevel.None, new EventId(1), new Dictionary<string, string>(), null, (dictionary, exception) => "");

            loggerMock.Verify(lm => lm.IsDisabled, Times.Once);
            loggerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void BeginScopeThrowsNotImplmented()
        {
            var loggerMock = new Mock<ILogger>();

            var rlLogger = new RockLibLogger(loggerMock.Object, null);

            Action action = () =>
                rlLogger.BeginScope(new Dictionary<string, string>());

            action.Should().Throw<NotImplementedException>();
        }

        [Theory]
        [InlineData(LogLevel.NotSet, MSE.LogLevel.None, false)]
        [InlineData(LogLevel.NotSet, MSE.LogLevel.Trace, true)]
        [InlineData(LogLevel.NotSet, MSE.LogLevel.Debug, true)]
        [InlineData(LogLevel.NotSet, MSE.LogLevel.Information, true)]
        [InlineData(LogLevel.NotSet, MSE.LogLevel.Warning, true)]
        [InlineData(LogLevel.NotSet, MSE.LogLevel.Error, true)]
        [InlineData(LogLevel.NotSet, MSE.LogLevel.Critical, true)]
        [InlineData(LogLevel.Debug, MSE.LogLevel.None, false)]
        [InlineData(LogLevel.Debug, MSE.LogLevel.Trace, true)]
        [InlineData(LogLevel.Debug, MSE.LogLevel.Debug, true)]
        [InlineData(LogLevel.Debug, MSE.LogLevel.Information, true)]
        [InlineData(LogLevel.Debug, MSE.LogLevel.Warning, true)]
        [InlineData(LogLevel.Debug, MSE.LogLevel.Error, true)]
        [InlineData(LogLevel.Debug, MSE.LogLevel.Critical, true)]
        [InlineData(LogLevel.Info, MSE.LogLevel.None, false)]
        [InlineData(LogLevel.Info, MSE.LogLevel.Trace, false)]
        [InlineData(LogLevel.Info, MSE.LogLevel.Debug, false)]
        [InlineData(LogLevel.Info, MSE.LogLevel.Information, true)]
        [InlineData(LogLevel.Info, MSE.LogLevel.Warning, true)]
        [InlineData(LogLevel.Info, MSE.LogLevel.Error, true)]
        [InlineData(LogLevel.Info, MSE.LogLevel.Critical, true)]
        [InlineData(LogLevel.Warn, MSE.LogLevel.None, false)]
        [InlineData(LogLevel.Warn, MSE.LogLevel.Trace, false)]
        [InlineData(LogLevel.Warn, MSE.LogLevel.Debug, false)]
        [InlineData(LogLevel.Warn, MSE.LogLevel.Information, false)]
        [InlineData(LogLevel.Warn, MSE.LogLevel.Warning, true)]
        [InlineData(LogLevel.Warn, MSE.LogLevel.Error, true)]
        [InlineData(LogLevel.Warn, MSE.LogLevel.Critical, true)]
        [InlineData(LogLevel.Error, MSE.LogLevel.None, false)]
        [InlineData(LogLevel.Error, MSE.LogLevel.Trace, false)]
        [InlineData(LogLevel.Error, MSE.LogLevel.Debug, false)]
        [InlineData(LogLevel.Error, MSE.LogLevel.Information, false)]
        [InlineData(LogLevel.Error, MSE.LogLevel.Warning, false)]
        [InlineData(LogLevel.Error, MSE.LogLevel.Error, true)]
        [InlineData(LogLevel.Error, MSE.LogLevel.Critical, true)]
        [InlineData(LogLevel.Fatal, MSE.LogLevel.None, false)]
        [InlineData(LogLevel.Fatal, MSE.LogLevel.Trace, false)]
        [InlineData(LogLevel.Fatal, MSE.LogLevel.Debug, false)]
        [InlineData(LogLevel.Fatal, MSE.LogLevel.Information, false)]
        [InlineData(LogLevel.Fatal, MSE.LogLevel.Warning, false)]
        [InlineData(LogLevel.Fatal, MSE.LogLevel.Error, false)]
        [InlineData(LogLevel.Fatal, MSE.LogLevel.Critical, true)]
        public void RockLibLoggerIsEnabledCallsILoggerIsEnabled(LogLevel rlLogLevel, MSE.LogLevel mseLogLevel, bool shouldBeEnabled)
        {
            var loggerMock = new Mock<ILogger>();
            loggerMock.Setup(lm => lm.IsDisabled).Returns(false);
            loggerMock.Setup(lm => lm.Level).Returns(rlLogLevel);

            var rlLogger = new RockLibLogger(loggerMock.Object, null);

            rlLogger.IsEnabled(mseLogLevel).Should().Be(shouldBeEnabled);
        }
    }
}
