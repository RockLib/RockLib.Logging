using RockLib.Diagnostics;
using System;
using System.Diagnostics;

namespace RockLib.Logging.LogProcessing;

/// <summary>
/// A base class for implementations of the <see cref="ILogProcessor"/> interface.
/// </summary>
public abstract class LogProcessor : ILogProcessor
{
    /// <summary>
    /// Gets a <see cref="System.Diagnostics.TraceSource"/> for diagnostics.
    /// </summary>
    protected static TraceSource TraceSource { get; } = Tracing.GetTraceSource(Logger.TraceSourceName);

    /// <summary>
    /// Gets a value indicating whether the log processor has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Disposes the log processor.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the log processor.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true"/> if disposing; <see langword="false"/> if finalizing.
    /// </param>
    protected virtual void Dispose(bool disposing) => IsDisposed = true;

    /// <summary>
    /// Processes the log entry on behalf of the logger.
    /// </summary>
    /// <param name="logger">
    /// The logger that the log entry is processed on behalf of. Its log
    /// providers and context providers define how the log entry is processed.
    /// </param>
    /// <param name="logEntry">The log entry to process.</param>
    public virtual void ProcessLogEntry(ILogger logger, LogEntry logEntry)
    {
        foreach (var contextProvider in logger.ContextProviders)
        {
            try
            {
                contextProvider.AddContext(logEntry);
            }
            catch (Exception ex)
            {
                TraceSource.TraceEvent(TraceEventType.Warning, ex.HResult,
                    "[{0:s}] - Error while adding context to log entry {1} using context provider {2}.{3}{4}",
                    DateTime.Now, logEntry.UniqueId, contextProvider, Environment.NewLine, ex);

                continue;
            }
        }

        var errorHandler = logger.ErrorHandler ?? NullErrorHandler.Instance;

        foreach (var logProvider in logger.LogProviders)
        {
            if (logEntry.Level < logProvider.Level)
                continue;

            try
            {
                SendToLogProvider(logProvider, logEntry, errorHandler, 0);
            }
            catch (Exception ex)
            {
                HandleError(ex, logProvider, logEntry, errorHandler, 1,
                    "Error while sending log entry {0} to log provider {1}.", logEntry.UniqueId, logProvider);
            }
        }
    }

    /// <summary>
    /// Send the log entry to the log provider.
    /// </summary>
    /// <param name="logProvider">The log provider to send the log entry to.</param>
    /// <param name="logEntry">The log entry that is being processed.</param>
    /// <param name="errorHandler">
    /// The object that handles errors that occur during log processing.
    /// </param>
    /// <param name="failureCount">The number of times this log entry has failed to send.</param>
    protected abstract void SendToLogProvider(
        ILogProvider logProvider, LogEntry logEntry, IErrorHandler errorHandler, int failureCount);

    /// <summary>
    /// Handles an error.
    /// </summary>
    /// <param name="exception">
    /// The exception that caused the error. <see langword="null"/> indicates a timeout error.
    /// </param>
    /// <param name="logProvider">
    /// The log provider responsible for the error.
    /// </param>
    /// <param name="logEntry">
    /// The log entry that failed to be sent by the log provider.
    /// </param>
    /// <param name="errorHandler">
    /// The object that handles errors that occur during log processing.
    /// </param>
    /// <param name="failureCount">
    /// The number of times this log entry has failed to send (including the error that is
    /// being handled).
    /// </param>
    /// <param name="errorMessageFormat">A format string for the message that describes the error.</param>
    /// <param name="errorMessageArgs">An object array containing zero or more objects to format.</param>
    protected virtual void HandleError(Exception exception, ILogProvider logProvider, LogEntry logEntry,
        IErrorHandler errorHandler, int failureCount, string errorMessageFormat, params object[] errorMessageArgs)
    {
        TraceError(exception, errorMessageFormat, errorMessageArgs);

        var error = new Error(string.Format(errorMessageFormat, errorMessageArgs),
            exception, logProvider, logEntry, failureCount);

        try
        {
            errorHandler.HandleError(error);
        }
        catch (Exception ex)
        {
            TraceSource.TraceEvent(TraceEventType.Warning, ex.HResult,
                "[{0:s}] - Error in error handler.{1}{2}",
                DateTime.Now, Environment.NewLine, ex);
        }

        if (error.ShouldRetry)
        {
            try
            {
                SendToLogProvider(logProvider, logEntry, errorHandler, failureCount);
            }
            catch (Exception ex)
            {
                HandleError(ex, logProvider, logEntry, errorHandler, failureCount + 1,
                    "Error while re-sending log entry {0} to log provider {1}.", logEntry.UniqueId, logProvider);
            }
        }
    }

    private static void TraceError(Exception exception, string messageFormat, object[] messageArgs)
    {
        string traceFormat;
        object[] traceArgs;
        int traceId;

        if (exception is not null)
        {
            traceFormat = string.Concat("[{", messageArgs.Length,
                ":s}] - ",
                messageFormat,
                '{', messageArgs.Length + 1, '}',
                '{', messageArgs.Length + 2, '}');

            traceArgs = new object[messageArgs.Length + 3];
            messageArgs.CopyTo(traceArgs, 0);
            traceArgs[traceArgs.Length - 3] = DateTime.Now;
            traceArgs[traceArgs.Length - 2] = Environment.NewLine;
            traceArgs[traceArgs.Length - 1] = exception;

            traceId = exception.HResult;
        }
        else
        {
            traceFormat = string.Concat("[{", messageArgs.Length,
                ":s}] - ",
                messageFormat);

            traceArgs = new object[messageArgs.Length + 1];
            messageArgs.CopyTo(traceArgs, 0);
            traceArgs[traceArgs.Length - 3] = DateTime.Now;

            traceId = 0;
        }

        TraceSource.TraceEvent(TraceEventType.Error, traceId, traceFormat, traceArgs);
    }

    private class NullErrorHandler : IErrorHandler
    {
        public static readonly IErrorHandler Instance = new NullErrorHandler();
        void IErrorHandler.HandleError(Error error) { }
    }
}
