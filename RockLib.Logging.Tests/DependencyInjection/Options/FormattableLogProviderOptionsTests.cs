using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RockLib.Logging.DependencyInjection;
using System;
using Xunit;

namespace RockLib.Logging.Tests.DependencyInjection.Options
{
    public class FormattableLogProviderOptionsTests
    {
        private readonly IServiceProvider _emptyServiceProvider = new ServiceCollection().BuildServiceProvider();

        [Fact]
        public void SetTemplateMethodHappyPath()
        {
            var options = new TestFormattableLogProviderOptions();

            options.SetTemplate("foo");

            var formatter = options.FormatterRegistration.Invoke(_emptyServiceProvider);

            formatter.Should().BeOfType<TemplateLogFormatter>()
                .Which.Template.Should().Be("foo");
        }

        [Fact]
        public void SetTemplateMethodSadPath()
        {
            var options = new TestFormattableLogProviderOptions();

            Action act = () => options.SetTemplate(null);

            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*template*");
        }

        [Fact]
        public void SetFormatterMethod1HappyPath()
        {
            var formatter = new Mock<ILogFormatter>().Object;

            var options = new TestFormattableLogProviderOptions();

            options.SetFormatter(formatter);

            var actualFormatter = options.FormatterRegistration.Invoke(_emptyServiceProvider);

            actualFormatter.Should().BeSameAs(formatter);
        }

        [Fact]
        public void SetFormatterMethod1SadPath()
        {
            var options = new TestFormattableLogProviderOptions();

            Action act = () => options.SetFormatter(null);

            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*formatter*");
        }

        [Fact]
        public void SetFormatterMethod2HappyPath()
        {
            var options = new TestFormattableLogProviderOptions();

            options.SetFormatter<TestLogFormatter>(123);

            var formatter = options.FormatterRegistration.Invoke(_emptyServiceProvider);

            formatter.Should().BeOfType<TestLogFormatter>()
                .Which.Foo.Should().Be(123);
        }

        private class TestFormattableLogProviderOptions : FormattableLogProviderOptions
        {
        }

        private class TestLogFormatter : ILogFormatter
        {
            public TestLogFormatter(int foo)
            {
                Foo = foo;
            }

            public int Foo { get; }

            public string Format(LogEntry logEntry) => throw new NotImplementedException();
        }
    }
}
