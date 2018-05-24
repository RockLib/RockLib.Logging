using System.Reflection;
using FluentAssertions;
using Xunit;

namespace RockLib.Logging.Extensions.Tests
{
    public class RockLibLoggerProviderTests
    {
        private readonly FieldInfo _nameField;
        private readonly FieldInfo _loggerField;
        private readonly FieldInfo _categoryField;

        public RockLibLoggerProviderTests()
        {
            _nameField = typeof(RockLibLoggerProvider).GetField("_rockLibName", BindingFlags.NonPublic | BindingFlags.Instance);
            _loggerField = typeof(RockLibLogger).GetField("_logger", BindingFlags.NonPublic | BindingFlags.Instance);
            _categoryField = typeof(RockLibLogger).GetField("_categoryName", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [Fact]
        public void EmptyConstructorSetsNullRockLibName()
        {
            var provider = new RockLibLoggerProvider();

            _nameField.GetValue(provider).Should().BeNull();
        }

        [Fact]
        public void ConstructorSetsRockLibName()
        {
            var provider = new RockLibLoggerProvider("SomeRockName");

            _nameField.GetValue(provider).Should().Be("SomeRockName");
        }

        [Fact]
        public void CreateLoggerSucceeds()
        {
            var provider = new RockLibLoggerProvider("SomeRockLibName");
            var logger = provider.CreateLogger("SomeCategory");

            ((ILogger) _loggerField.GetValue(logger)).Name.Should().Be("SomeRockLibName");
            _categoryField.GetValue(logger).Should().Be("SomeCategory");
        }
    }
}
