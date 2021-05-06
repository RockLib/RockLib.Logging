using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using RockLib.Logging.Moq;
using System.Collections.Generic;
using MicrosoftLogLevel = Microsoft.Extensions.Logging.LogLevel;
using static Microsoft.Extensions.Logging.LogLevel;

namespace RockLib.Logging.Microsoft.Extensions.Tests
{
    public class RockLibLoggerTests
    {
        [Fact(DisplayName = "Constructor sets expected properties")]
        public void ConstructorHappyPath1()
        {
            var logger = new MockLogger().Object;
            var scopeProvider = new Mock<IExternalScopeProvider>().Object;

            var rockLibLogger = new RockLibLogger(logger, "MyCategory", scopeProvider);

            rockLibLogger.Logger.Should().BeSameAs(logger);
            rockLibLogger.CategoryName.Should().Be("MyCategory");
            rockLibLogger.ScopeProvider.Should().BeSameAs(scopeProvider);
        }

        [Fact(DisplayName = "Constructor sets handles null scopeProvider parameter")]
        public void ConstructorHappyPath2()
        {
            var logger = new MockLogger().Object;

            var rockLibLogger = new RockLibLogger(logger, "MyCategory", null);

            rockLibLogger.Logger.Should().BeSameAs(logger);
            rockLibLogger.CategoryName.Should().Be("MyCategory");
            rockLibLogger.ScopeProvider.Should().BeNull();
        }

        [Fact(DisplayName = "Constructor throws when logger parameter is null")]
        public void ConstructorSadPath1()
        {
            Action act = () => new RockLibLogger(null, "MyCategory", null);

            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*logger*");
        }

        [Fact(DisplayName = "Constructor throws when categoryName parameter is null")]
        public void ConstructorSadPath2()
        {
            var logger = new MockLogger().Object;

            Action act = () => new RockLibLogger(logger, null, null);

            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*categoryName*");
        }

        private class TestScopeProvider : IExternalScopeProvider
        {
            public object State { get; set; }
            public void ForEachScope<TState>(Action<object, TState> callback, TState state) => callback(State, state);
            public IDisposable Push(object state) => throw new NotImplementedException();
        }

        [Fact(DisplayName = "Log method passes mapped log entry to logger")]
        public void LogMethodHappyPath1()
        {
            var mockLogger = new MockLogger(LogLevel.Warn);
            var scopeProvider = new TestScopeProvider { State = "MyState" };

            var rockLibLogger = new RockLibLogger(mockLogger.Object, "MyCategory", scopeProvider);

            var ex = new Exception();
            var eventId = new EventId(123);

            string capturedState = null;
            Exception capturedException = null;

            rockLibLogger.Log(Warning, eventId, "Hello, world!", ex, Format);

            var extendedProperties = new Dictionary<string, object>
            {
                ["Microsoft.Extensions.Logging.EventId"] = eventId,
                ["Microsoft.Extensions.Logging.State"] = "Hello, world!",
                ["Microsoft.Extensions.Logging.CategoryName"] = "MyCategory",
                ["Microsoft.Extensions.Logging.Scope"] = new object[] { "MyState" }
            };

            mockLogger.VerifyWarn("formatted", extendedProperties, Times.Once());

            capturedState.Should().Be("Hello, world!");
            capturedException.Should().BeSameAs(ex);

            string Format(string state, Exception exception)
            {
                capturedState = state;
                capturedException = exception;
                return "formatted";
            }
        }

        [Fact(DisplayName = "Log method does nothing when logLevel is not enabled for logger")]
        public void LogMethodHappyPath2()
        {
            var mockLogger = new MockLogger(LogLevel.Fatal);
            var scopeProvider = new TestScopeProvider { State = "MyState" };

            var rockLibLogger = new RockLibLogger(mockLogger.Object, "MyCategory", scopeProvider);

            var ex = new Exception();
            var eventId = new EventId(123);

            string capturedState = null;
            Exception capturedException = null;

            rockLibLogger.Log(Warning, eventId, "Hello, world!", ex, Format);

            mockLogger.VerifyWarn(Times.Never());

            capturedState.Should().BeNull();
            capturedException.Should().BeNull();

            string Format(string state, Exception exception)
            {
                capturedState = state;
                capturedException = exception;
                return "formatted";
            }
        }

        [Fact(DisplayName = "Log method throws when formatter parameter is null")]
        public void LogMethodSadPath()
        {
            var logger = new MockLogger().Object;

            var rockLibLogger = new RockLibLogger(logger, "MyCategory", null);

            rockLibLogger.Invoking(x =>
                x.Log(Warning, new EventId(123), "Hello, world!", new Exception(), null))
                .Should().ThrowExactly<ArgumentNullException>("*formatter*");
        }

        [Fact(DisplayName = "IsEnabled method returns false given LogLevel.None")]
        public void IsEnabledMethodHappyPath1()
        {
            var logger = new MockLogger().Object;

            var rockLibLogger = new RockLibLogger(logger, "MyCategory", null);

            var isEnabled = rockLibLogger.IsEnabled(None);

            isEnabled.Should().BeFalse();
        }

        [Theory(DisplayName = "IsEnabled method returns according to Logger.Level")]
        [InlineData(LogLevel.NotSet, Trace, true)]
        [InlineData(LogLevel.NotSet, Debug, true)]
        [InlineData(LogLevel.NotSet, Information, true)]
        [InlineData(LogLevel.NotSet, Warning, true)]
        [InlineData(LogLevel.NotSet, MicrosoftLogLevel.Error, true)]
        [InlineData(LogLevel.NotSet, Critical, true)]
        [InlineData(LogLevel.Debug, Trace, true)]
        [InlineData(LogLevel.Debug, Debug, true)]
        [InlineData(LogLevel.Debug, Information, true)]
        [InlineData(LogLevel.Debug, Warning, true)]
        [InlineData(LogLevel.Debug, MicrosoftLogLevel.Error, true)]
        [InlineData(LogLevel.Debug, Critical, true)]
        [InlineData(LogLevel.Info, Trace, false)]
        [InlineData(LogLevel.Info, Debug, false)]
        [InlineData(LogLevel.Info, Information, true)]
        [InlineData(LogLevel.Info, Warning, true)]
        [InlineData(LogLevel.Info, MicrosoftLogLevel.Error, true)]
        [InlineData(LogLevel.Info, Critical, true)]
        [InlineData(LogLevel.Warn, Trace, false)]
        [InlineData(LogLevel.Warn, Debug, false)]
        [InlineData(LogLevel.Warn, Information, false)]
        [InlineData(LogLevel.Warn, Warning, true)]
        [InlineData(LogLevel.Warn, MicrosoftLogLevel.Error, true)]
        [InlineData(LogLevel.Warn, Critical, true)]
        [InlineData(LogLevel.Error, Trace, false)]
        [InlineData(LogLevel.Error, Debug, false)]
        [InlineData(LogLevel.Error, Information, false)]
        [InlineData(LogLevel.Error, Warning, false)]
        [InlineData(LogLevel.Error, MicrosoftLogLevel.Error, true)]
        [InlineData(LogLevel.Error, Critical, true)]
        [InlineData(LogLevel.Fatal, Trace, false)]
        [InlineData(LogLevel.Fatal, Debug, false)]
        [InlineData(LogLevel.Fatal, Information, false)]
        [InlineData(LogLevel.Fatal, Warning, false)]
        [InlineData(LogLevel.Fatal, MicrosoftLogLevel.Error, false)]
        [InlineData(LogLevel.Fatal, Critical, true)]
        [InlineData(LogLevel.Audit, Trace, false)]
        [InlineData(LogLevel.Audit, Debug, false)]
        [InlineData(LogLevel.Audit, Information, false)]
        [InlineData(LogLevel.Audit, Warning, false)]
        [InlineData(LogLevel.Audit, MicrosoftLogLevel.Error, false)]
        [InlineData(LogLevel.Audit, Critical, false)]
        public void IsEnabledInfodHappyPath2(LogLevel loggerLogLevel, MicrosoftLogLevel inputLogLevel, bool expectedValue)
        {
            var logger = new MockLogger(loggerLogLevel).Object;

            var rockLibLogger = new RockLibLogger(logger, "MyCategory", null);

            rockLibLogger.IsEnabled(inputLogLevel).Should().Be(expectedValue);
        }

        [Fact(DisplayName = "BeginScope method pushes state onto scope provider")]
        public void BeginScopeMethodHappyPath1()
        {
            var expectedToken = new Mock<IDisposable>().Object;
            var mockScopeProvider = new Mock<IExternalScopeProvider>();
            mockScopeProvider.Setup(m => m.Push(It.IsAny<object>())).Returns(expectedToken);
            var logger = new MockLogger().Object;

            var rockLibLogger = new RockLibLogger(logger, "MyCategory", mockScopeProvider.Object);

            var token = rockLibLogger.BeginScope("Hello, world!");

            mockScopeProvider.Verify(m => m.Push("Hello, world!"), Times.Once());
            token.Should().BeSameAs(expectedToken);
        }

        [Fact(DisplayName = "BeginScope method does nothing when scope provider is null")]
        public void BeginScopeMethodHappyPath2()
        {
            var logger = new MockLogger().Object;
            
            var rockLibLogger = new RockLibLogger(logger, "MyCategory", null);

            var token = rockLibLogger.BeginScope("Hello, world!");

            token.Should().BeNull();
        }
    }
}
