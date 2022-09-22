# Handle Log Provider Errors

By default, errors produced by log providers are only traced, see [Tracing](Tracing.md). In order to directly handle the errors, the `ILogger.ErrorHandler` can be set, either directly or through an extension method.

## Error Type

**Message**
  - Type: string
  - Description: Gets the message that describes the error.

**Exception**
  - Type: Exception
  - Description: Gets the exception responsible for the error.

**LogProvider**
  - Type: ILogProvider
  - Description: Gets the log provider that failed to write the log entry.

**LogEntry**
  - Type: LogEntry
  - Description: Gets the log entry that failed to write.

**FailureCount**
  - Type: int
  - Description: Gets the number of times the log provider has failed to write the log entry.

**IsTimeout**
  - Type: bool
  - Description: Gets a value indicating whether the error was a result of timing out.

**Timestamp**
  - Type: DateTime
  - Description: Gets the time that the error event occurred.

**ShouldRetry**
  - Type: bool
  - Description: Gets or sets a value indicating whether the log provider should attempt to send the log entry again.

## SetErrorHandler Extension

The `SetErrorHandler` extension uses a `DelegateErrorHandler` behind the scenes. This will invoke whatever action is passed into it with the given `Error`.

```csharp
logger.SetErrorHandler(e =>
{
    // Do something with the error here.
})
```

## Directly Setting ErrorHandler Property

In order to directly set the `ErrorHandler` property, there needs to be a new implementation of the `IErrorHandler` interface. There are currently no public implementations of the interface.

```csharp
logger.ErrorHandler = new ExampleErrorHandler();

public class ExampleErrorHandler : IErrorHandler
{
    public void HandleError(Error error)
    {
        // Do something with the error here.
    }
}
```

## Adding Retry On Error

One possibility for handling the errors is to retry sending to the provider. If the `ShouldRetry` property is set to `true` when `HandleError` is called, the provider will be retried. Each attempt will increment the `FailureCount` property.

### Simple example of adding retry:

```csharp
logger.SetErrorHandler(e =>
{
    if (e.FailureCount < 4)
        e.ShouldRetry = true;
})
```
