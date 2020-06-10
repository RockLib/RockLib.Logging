using Moq;

namespace RockLib.Logging.Moq
{
    public class MockLogger : Mock<ILogger>
    {
        public MockLogger(string name = "default", LogLevel level = LogLevel.Debug, MockBehavior behavior = MockBehavior.Default)
            : base(behavior)
        {
            this.Setup(name, level);
        }
    }
}
