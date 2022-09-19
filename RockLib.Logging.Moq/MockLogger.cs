using Moq;

namespace RockLib.Logging.Moq;

/// <summary>
/// Provides a mock implementation of <see cref="ILogger"/>.
/// </summary>
public class MockLogger : Mock<ILogger>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MockLogger"/> class.
    /// </summary>
    /// <param name="level">The level of the mock logger.</param>
    /// <param name="name">The name of the mock logger.</param>
    /// <param name="behavior">The behavior of the mock.</param>
    public MockLogger(LogLevel level = LogLevel.Debug, string name = Logger.DefaultName, MockBehavior behavior = MockBehavior.Default)
        : base(behavior) => this.SetupLogger(level, name);
}
