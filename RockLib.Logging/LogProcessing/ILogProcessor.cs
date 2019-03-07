using System;

namespace RockLib.Logging.LogProcessing
{
    public interface ILogProcessor : IDisposable
    {
        bool IsDisposed { get; }
        void ProcessLogEntry(ILogger logger, LogEntry logEntry, Action<ErrorEventArgs> errorHandler);
    }
}
