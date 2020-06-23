using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace RockLib.Logging.Moq.Tests
{
    public class MockLoggerTests
    {
        [Fact(DisplayName = "Constructor adds setups for Level and Name properties")]
        public void ConstructorHappyPath()
        {
            var mockLogger = new MockLogger(LogLevel.Info, "TestLogger", MockBehavior.Strict);

            mockLogger.Setups.Should().HaveCount(2);
            mockLogger.Behavior.Should().Be(MockBehavior.Strict);
            mockLogger.Object.Level.Should().Be(LogLevel.Info);
            mockLogger.Object.Name.Should().Be("TestLogger");
        }
    }
}
