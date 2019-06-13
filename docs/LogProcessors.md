# Log Processors

`LogProcessors` provide different ways of sending logs to `LogProviders`.

`RockLib.Logging` comes with three `LogProcessors` available for use.

## BackgroundLogProcessor

The `BackgroundLogProcessor` is the default `LogProcessor` used in `RockLib.Logging`.

This `LogProcessor` processes and tracks logs on dedicated non-threadpool background threads. When this is disposed of, it will block until all in-flight logs have finished processing.

This behavior ensures that, on a graceful shutdown of a application, that logs will still be sent to their destination.

## FireAndForgetLogProcessor

The `FireAndForgetLogProcessor` processes logs asynchronously, but without any task tracking.

As its name implies, these logs are not tracked and, in the event that the application shuts down before the logs have been processed, they may be lost. This is best used when logs from this application are not of great importance.

## SynchronousLogProcessor

The `SynchronousLogProcessor` processes logs on the same thread as the caller.

This behavior ensures that logs will be processed before the application can continue work. This is best used when logging is a necessary component of the application's operation.

## Creating Your Own LogProcessor

The above-mentioned implementations of `LogProcessor` should cover the majority of use cases for logging. However, if these do not meet the requirements for your application, you can implement a new log processor by having the new class implement the `ILogProcessor` interface or inherit the `LogProcessor` class.

## ILogProcessor

The `ILogProcessor` interface is available for implementation if custom logic is needed for processing logs. The `IsDisposed` property and `ProcessLogEntry` method both need to be defined.

**Note:** when defining the `ProcessLogEntry` method, `ILogger.Log` should not be called.

## LogProcessor

The abstract `LogProcessor` class implements `ILogProcessor` The `SendToLogProvider` abstract method must be defined.

Additionally, the folllowing virtual members provide base functionality, but are available for override, if necessary:

* `Dispose`
* `ProcessLogEntry`
* `HandleError`
