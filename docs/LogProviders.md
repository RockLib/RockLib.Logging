# Log Providers

The `ILogProvider` interface defines exactly how logs are written; a logger can have zero to many log providers. `RockLib.Logging` comes with three implementations of the `ILogProvider`.

## [ConsoleLogProvider](ConsoleLogProvider.md)

Writes log entries to standard out or standard error.

## [FileLogProvider](FileLogProvider.md)

Writes log entries to a file.

## [RollingFileLogProvider](RollingFileLogProvider.md)

Writes log entries to a file. Log files will then be archived based on time and filesize configuration.

## Creating Your Own LogProvider

New log providers can be created by implementing the `ILogProvider` interface.

## ILogProvider

The `ILogProvider` interface has `Timeout` and `Level` properties, as well as a `WriteAsync` method.

**Timeout Property**
  - Type: TimeSpan
  - Description: If a task returned by the `WriteAsync` method does not complete by the value of the `Timeout` property, the task will be cancelled.

**Level Property**
  - Type: LogLevel enum (Debug, Info, Warn, Error, Fatal, Audit)
  - Description: This value is used by the `Logger` class to determine if it should call this instance's `WriteAsync` method for a given log entry. If the value of this property is higher than a log entry's level, then this log provider is skipped.

**WriteAsync Method**
  - Description: Writes the specified log entry.
  - Parameters:
    - logEntry
      - Type: LogEntry
      - Description: The log entry to write.
    - cancellationToken
      - Type: CancellationToken.
      - Description: The `CancellationToken` to observe.
  - Returns: A task that completes when the log entry has been written.
