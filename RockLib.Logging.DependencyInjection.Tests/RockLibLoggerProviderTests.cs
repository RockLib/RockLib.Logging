using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace RockLib.Logging.DependencyInjection.Tests
{
    public class RockLibLoggerProviderTests
    {
        [Fact]
        public void ConstructorThrowsIfDelegateIsNull()
        {
            Action action = () => new RockLibLoggerProvider(null);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void CreateLoggerSucceeds()
        {
            var logger = new Mock<ILogger>().Object;
            ILogger CreateLogger() => logger;

            ILoggerProvider provider = new RockLibLoggerProvider(CreateLogger);

            var rockLibLogger = (RockLibLogger)provider.CreateLogger("SomeCategory");

            rockLibLogger.Logger.Should().BeSameAs(logger);
            rockLibLogger.CategoryName.Should().Be("SomeCategory");
        }
    }
}
