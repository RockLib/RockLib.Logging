﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.Logging;

/// <summary>
/// An implementation of <see cref="ILogProvider"/> that writes log entries to a file.
/// </summary>
public class FileLogProvider : ILogProvider
{
    // TODO: Use case insensitive comparer for Windows, case sensitive comparer for Mac/Linux.
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphoreCache = new();

    /// <summary>
    /// The default template.
    /// </summary>
    public const string DefaultTemplate = ConsoleLogProvider.DefaultTemplate;

    /// <summary>
    /// The default timeout.
    /// </summary>
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(3);

    private readonly SemaphoreSlim _semaphore;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileLogProvider"/> class.
    /// </summary>
    /// <param name="file">The file that logs will be written to.</param>
    /// <param name="template">The template used to format log entries.</param>
    /// <param name="level">The level of the log provider.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    public FileLogProvider(string file,
        string template = DefaultTemplate,
        LogLevel level = default,
        TimeSpan? timeout = null)
        : this(file, new TemplateLogFormatter(template ?? DefaultTemplate), level, timeout)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileLogProvider"/> class.
    /// </summary>
    /// <param name="file">The path to a writable file.</param>
    /// <param name="formatter">An object that formats log entries prior to writing to file.</param>
    /// <param name="level">The level of the log provider.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    public FileLogProvider(string file,
        ILogFormatter formatter,
        LogLevel level = default,
        TimeSpan? timeout = null)
    {
        if (!Enum.IsDefined(typeof(LogLevel), level))
            throw new ArgumentException($"Log level is not defined: {level}.", nameof(level));
        if (timeout.HasValue && timeout.Value < TimeSpan.Zero)
            throw new ArgumentException("Timeout cannot be negative.", nameof(timeout));

        File = file ?? throw new ArgumentNullException(nameof(file));
        Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        Level = level;
        Timeout = timeout ?? DefaultTimeout;

        var dir = Path.GetDirectoryName(File);

        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        _semaphore = _semaphoreCache.GetOrAdd(file, f => new SemaphoreSlim(1, 1));
    }

    /// <summary>
    /// Gets the file that logs will be written to.
    /// </summary>
    public string File { get; }

    /// <summary>
    /// Gets an object that formats log entries.
    /// </summary>
    public ILogFormatter Formatter { get; }

    /// <summary>
    /// Gets the log level.
    /// </summary>
    public LogLevel Level { get; }

    /// <summary>
    /// Gets the timeout.
    /// </summary>
    public TimeSpan Timeout { get; }

    /// <summary>
    /// Formats the log entry using the <see cref="Formatter"/> property and writes it the file
    /// specified by the <see cref="File"/> property.
    /// </summary>
    /// <param name="logEntry">The log entry to write.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <returns>A task that completes when the log entry has been written to file.</returns>
    public async Task WriteAsync(LogEntry logEntry, CancellationToken cancellationToken = default)
    {
        var formattedLogEntry = Formatter.Format(logEntry);

        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            await SynchronizedWriteAsync(formattedLogEntry, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Writes a formatted log entry to the file. This method is synchronized, that is,
    /// only one thread will execute this method at any given time.
    /// </summary>
    /// <param name="formattedLogEntry">The formatted log entry.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    protected virtual async Task SynchronizedWriteAsync(string formattedLogEntry, CancellationToken cancellationToken)
    {
        using (var writer = new StreamWriter(File, true))
        {
            await writer.WriteLineAsync(formattedLogEntry).ConfigureAwait(false);
        }
    }
}
