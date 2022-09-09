using Moq;
using RockLib.Dynamic;
using RockLib.Logging.LogProcessing;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.Tests.LogProcessingTests;

public class SynchronousLogProcessorTests
{
    [Fact]
    public void ProcessLogEntryCallsWriteAsyncOnTheLogProvider()
    {
        var logProcessor = new SynchronousLogProcessor();

        var mockLogProvider = new Mock<ILogProvider>();
        var logEntry = new LogEntry();

        mockLogProvider.Setup(m => m.WriteAsync(It.IsAny<LogEntry>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(0));

        logProcessor.Unlock().SendToLogProvider(mockLogProvider.Object, logEntry, null, 1);

        mockLogProvider.Verify(m => m.WriteAsync(logEntry, It.IsAny<CancellationToken>()), Times.Once);
    }
}
