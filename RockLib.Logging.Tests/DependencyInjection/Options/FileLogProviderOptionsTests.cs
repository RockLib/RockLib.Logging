using FluentAssertions;
using RockLib.Logging.DependencyInjection;
using System;
using Xunit;

namespace RockLib.Logging.Tests.DependencyInjection.Options
{
    public class FileLogProviderOptionsTests
    {
        [Fact]
        public void FilePropertySetterSadPath()
        {
            var options = new FileLogProviderOptions();

            Action act = () => options.File = null;

            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*value*");
        }
    }
}
