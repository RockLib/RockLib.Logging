using Moq;
using RockLib.Dynamic;
using RockLib.Logging.LogProcessing;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.Tests.LogProcessingTests;

public static class SynchronousLogProcessorTests
{
    [Fact]
    public static void ProcessLogEntryCallsWriteAsyncOnTheLogProvider()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        using var logProcessor = new SynchronousLogProcessor();
#pragma warning restore CS0618 // Type or member is obsolete

        var mockLogProvider = new Mock<ILogProvider>();
        var logEntry = new LogEntry();

        mockLogProvider.Setup(m => m.WriteAsync(It.IsAny<LogEntry>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(0));

        logProcessor.Unlock().SendToLogProvider(mockLogProvider.Object, logEntry, null, 1);

        mockLogProvider.Verify(m => m.WriteAsync(logEntry, It.IsAny<CancellationToken>()), Times.Once);
    }
}
