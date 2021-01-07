using FluentAssertions;
using Xunit;

namespace RockLib.Logging.Http.Tests
{
    using static LoggingActionFilter;
    using static Logger;

    public class AuditLogAttributeTests
    {
        [Fact(DisplayName = "Constructor sets properties from non-null parameters")]
        public void ConstructorHappyPath1()
        {
            string messageFormat = "My message format: {0}.";
            string loggerName = "MyLogger";

            var infoLogAttribute = new AuditLogAttribute(messageFormat, loggerName);

            infoLogAttribute.MessageFormat.Should().Be(messageFormat);
            infoLogAttribute.LoggerName.Should().Be(loggerName);
            infoLogAttribute.LogLevel.Should().Be(LogLevel.Audit);
        }

        [Fact(DisplayName = "Constructor sets properties from null parameters")]
        public void ConstructorHappyPath2()
        {
            var infoLogAttribute = new AuditLogAttribute(null, null);

            infoLogAttribute.MessageFormat.Should().Be(DefaultMessageFormat);
            infoLogAttribute.LoggerName.Should().Be(DefaultName);
            infoLogAttribute.LogLevel.Should().Be(LogLevel.Audit);
        }
    }
}
