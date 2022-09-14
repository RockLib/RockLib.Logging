using FluentAssertions;
using RockLib.Logging.DependencyInjection;
using System;
using Xunit;

namespace RockLib.Logging.Tests.DependencyInjection;

public static class FileLogProviderOptionsTests
{
    [Fact(DisplayName = "File property throws when set to null")]
    public static void FilePropertySetToNull()
    {
        Action act = () => new FileLogProviderOptions().File = null!;

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*value*");
    }
}
