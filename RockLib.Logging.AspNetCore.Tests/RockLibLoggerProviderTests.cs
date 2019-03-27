using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace RockLib.Logging.AspNetCore.Tests
{
    public class RockLibLoggerProviderTests
    {
        [Fact]
        public void ConstructorSetsCreateLogger()
        {
            Func<ILogger> createLogger = () => null;

            var provider = new RockLibLoggerProvider(createLogger);

            provider.CreateLogger.Should().BeSameAs(createLogger);
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
