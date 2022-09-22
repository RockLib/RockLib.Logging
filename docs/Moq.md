# How to test logging in an application using RockLib.Logging.Moq

The RockLib.Logging.Moq package makes it easy for applications to verify that logging has taken place. It provides a set of extension methods for setting up and verifying instances of `Mock<ILogger>`. It also provides a custom implementation of `Mock<ILogger>` - `MockLogger` - that automatically sets itself up.

## Extension methods

Note that all extension methods target the `Mock<ILogger>` type.

### SetupLogger extension method

The `SetupLogger` extension methods sets up the provided `Mock<ILogger>` so that it will properly function whenever any logging or safe-logging extension methods are called.

| Parameter Name | Type | Default Value | Description |
|---|---|---|---|
| level | `LogLevel` | `Debug` | The level of the mock logger. Logs below this level are not recorded. |
| name | `string` | `""` | The name of the mock logger. |

Example:

```csharp
private void Example(Mock<ILogger> mockLogger)
{
    mockLogger.SetupLogger(LogLevel.Warn, "TestLogger");
}
```

### VerifyLog extension methods

There are numerous extension methods for verifying that a log was recorded, each of which begins with "Verify". The difference between each extension method is the expected level of the log.

- `VerifyDebug` - Verifies that a `Debug` log was recorded.
- `VerifyInfo` - Verifies that an `Info` log was recorded.
- `VerifyWarn` - Verifies that a `Warn` log was recorded.
- `VerifyError` - Verifies that an `Error` log was recorded.
- `VerifyFatal` - Verifies that a `Fatal` log was recorded.
- `VerifyAudit` - Verifies that an `Audit` log was recorded.
- `VerifyLog` - Verifies that a log was recorded, regardless of level.

### VerifyLog extension method parameters

There are many overloads of each log verification extension method. The difference between them is the additional parameters that are provided. Each parameter defines an additional restriction to be placed on the verification. All overloads include two parameters: `times` and `failMessage`.

| Parameter name | Type | Description |
|---|---|---|
| times | `Moq.Times` | How many times the specified log should have been recorded. Default is `Once`. |
| failMessage | `string` | The message to show if verification fails. |
| message | `string` | The value that the message of a log must equal in order for successful verification to occur. If the value of this parameter starts and ends with / (forward slash), then the contents of the value between the slashes are interpreted as a regular expression and matched against the log message. |
| extendedProperties | `object` | An object representing the extended properties that a log must match in order for successful verification to occur. |
| exception | `Exception` | The exception instance that a log entry's `Exception` must be a reference to in order for successful verification to occur. Pass `null` if `LogEntry.Exception` is expected to be `null`. |
| hasMatchingException | `Expression<Func<Exception, bool>>` | A function evaluated with a log entry's `Exception` that must return `true` in order for successful verification to occur. |

## MockLogger class

The `MockLogger` class, which inherits from `Mock<ILogger>`, exists to further simplify the initialization of mock loggers. All it does is have its constructor call the `SetupLogger` extension method with the parameters passed to it. So instead of this:

```csharp
Mock<ILogger> mockLogger = new Mock<ILogger>();
mockLogger.SetupLogger(LogLevel.Info, "MyLogger");
```

...you can do this:

```csharp
Mock<ILogger> mockLogger = new MockLogger(LogLevel.Info, "MyLogger");
```
