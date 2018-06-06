using System.Reflection;
using FluentAssertions;
using Moq;
using Xunit;

namespace RockLib.Logging.AspNetCore.Tests
{
    public class RockLibLoggerProviderTests
    {
        [Fact]
        public void ConstructorSetsLogger()
        {
            var logger = new Mock<ILogger>().Object;

            var provider = new RockLibLoggerProvider(logger);

            provider.Logger.Should().BeSameAs(logger);
        }

        [Fact]
        public void CreateLoggerSucceeds()
        {
            var logger = new Mock<ILogger>().Object;

            var provider = new RockLibLoggerProvider(logger);

            var rockLibLogger = (RockLibLogger)provider.CreateLogger("SomeCategory");

            rockLibLogger.Logger.Should().BeSameAs(logger);
            rockLibLogger.CategoryName.Should().Be("SomeCategory");
        }
    }
}
