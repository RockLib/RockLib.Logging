using FluentAssertions;
using Xunit;

namespace RockLib.Logging.AspNetCore.Tests
{
    using static LoggingActionFilter;
    using static Logger;

    public class InfoLogAttributeTests
    {
        [Fact(DisplayName = "Constructor 1 sets properties from non-null parameters")]
        public void Constructor1HappyPath1()
        {
            const string messageFormat = "My message format: {0}.";
            const string loggerName = "MyLogger";
            const string exceptionMessageFormat = "My exception message format: {0}.";
            const LogLevel exceptionLogLevel = LogLevel.Fatal;

            var infoLogAttribute = new InfoLogAttribute(messageFormat, loggerName, exceptionMessageFormat, exceptionLogLevel);

            infoLogAttribute.MessageFormat.Should().Be(messageFormat);
            infoLogAttribute.LoggerName.Should().Be(loggerName);
            infoLogAttribute.LogLevel.Should().Be(LogLevel.Info);
            infoLogAttribute.ExceptionMessageFormat.Should().Be(exceptionMessageFormat);
            infoLogAttribute.ExceptionLogLevel.Should().Be(exceptionLogLevel);
        }

        [Fact(DisplayName = "Constructor 1 sets properties from null parameters")]
        public void Constructor1HappyPath2()
        {
            var infoLogAttribute = new InfoLogAttribute(null, null, null);

            infoLogAttribute.MessageFormat.Should().Be(DefaultMessageFormat);
            infoLogAttribute.LoggerName.Should().Be(DefaultName);
            infoLogAttribute.LogLevel.Should().Be(LogLevel.Info);
            infoLogAttribute.ExceptionMessageFormat.Should().Be(DefaultExceptionMessageFormat);
            infoLogAttribute.ExceptionLogLevel.Should().Be(DefaultExceptionLogLevel);
        }

        [Fact(DisplayName = "Constructor 2 sets properties from non-null parameters")]
        public void Constructor2HappyPath1()
        {
            string messageFormat = "My message format: {0}.";
            string loggerName = "MyLogger";

            var infoLogAttribute = new InfoLogAttribute(messageFormat, loggerName);

            infoLogAttribute.MessageFormat.Should().Be(messageFormat);
            infoLogAttribute.LoggerName.Should().Be(loggerName);
            infoLogAttribute.LogLevel.Should().Be(LogLevel.Info);
        }

        [Fact(DisplayName = "Constructor 2 sets properties from null parameters")]
        public void Constructor2HappyPath2()
        {
            var infoLogAttribute = new InfoLogAttribute(null, null);

            infoLogAttribute.MessageFormat.Should().Be(DefaultMessageFormat);
            infoLogAttribute.LoggerName.Should().Be(DefaultName);
            infoLogAttribute.LogLevel.Should().Be(LogLevel.Info);
        }
    }
}
