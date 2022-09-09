using FluentAssertions;
using RockLib.Logging.DependencyInjection;
using Xunit;

namespace RockLib.Logging.Tests.DependencyInjection;

public class RollingFileLogProviderOptionsTests
{
    [Fact(DisplayName = "RollingFileLogProviderOptions has the correct default property values")]
    public void DefaultValues()
    {
        var options = new RollingFileLogProviderOptions();

        options.MaxFileSizeKilobytes.Should().Be(RollingFileLogProvider.DefaultMaxFileSizeKilobytes);
        options.MaxArchiveCount.Should().Be(RollingFileLogProvider.DefaultMaxArchiveCount);
        options.RolloverPeriod.Should().Be(RollingFileLogProvider.DefaultRolloverPeriod);
    }
}
