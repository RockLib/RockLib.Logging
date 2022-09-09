using FluentAssertions;
using RockLib.Logging.DependencyInjection;
using System;
using Xunit;

namespace RockLib.Logging.Tests.DependencyInjection;

public class FileLogProviderOptionsTests
{
    [Fact(DisplayName = "File property throws when set to null")]
    public void FilePropertySetterSadPath()
    {
        var options = new FileLogProviderOptions();

        Action act = () => options.File = null;

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*value*");
    }
}
