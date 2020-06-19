using Moq;

namespace RockLib.Logging.Moq
{
    public class MockLogger : Mock<ILogger>
    {
        public MockLogger(LogLevel level = LogLevel.Debug, string name = Logger.DefaultName, MockBehavior behavior = MockBehavior.Default)
            : base(behavior)
        {
            this.Setup(level, name);
        }
    }
}
