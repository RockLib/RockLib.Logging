# How to perform basic logging operations

Logging operations start with an instance of the `ILogger` interface (to obtain one, see the [Logger](Logger.md) and [LoggerFactory](LoggerFactory.md) docs) and use either its `Log` method or one of the various logging extension methods.

## Extension methods

There are two versions of each logging extension method: one that has an `Exception` parameter, and one that does not. The parameters for all the extension methods are otherwise identical.

Parameter          | Required | Description
------------------ | -------- | -----------
message            | Yes      | The log message.
exception          | Yes*     | The `Exception` associated with the current logging operation.
extendedProperties | No       | An object that represents the extended properties of the log entry.
correlationId      | No       | The ID used to corralate a transaction across many service calls for this log entry.
businessProcessId  | No       | The business process ID.
businessActivityId | No       | The business activity ID.
callerMemberName   | No**     | The method or property name of the caller.
callerFilePath     | No**     | The path of the source file that contains the caller.
callerLineNumber   | No**     | The line number in the source file at which this method is called.

> Only one version of the extension methods includes an `exception` parameter. Values should **never** be provided for these parameters (the compiler provides these values).*

These extension methods correspond to the six log levels, which indicate the type or severity of a log. The log levels are (from low to high):

- `Debug`: Fine-grained informational events that are most useful to debug an application.
- `Info`: Informational messages that highlight the progress of the application at coarse-grained level.
- `Warn`: Potentially harmful situations.
- `Error`: Error events that might still allow the application to continue running.
- `Fatal`: Very severe error events that will presumably lead the application to abort.
- `Audit`: Critical informational messages, such as when a user views sensitive information.

### Enabled extension methods

In order to save resources, any logging operation (especially at lower log levels) that involves string formatting/concatenation or extended properties should ensure that logging is enabled for the specified log level first. These extension methods are:

```csharp
logger.IsDebugEnabled()
logger.IsInfoEnabled()
logger.IsWarnEnabled()
logger.IsErrorEnabled()
logger.IsFatalEnabled()
logger.IsAuditEnabled()
```

## Examples

---

### Simple informational logging with just a message:

```csharp
logger.Debug("Entered Foo method.");

logger.Info("Bar request was successful.");

logger.Audit($"User '{username}' is viewing Baz.");
```

---

### Error logs with a message and an exception:

```csharp
// Assuming that the 'ex' variable is a caught Exception...

logger.Fatal("Unable to start application.", ex);

logger.Warn("Request failed using primary url. Attempting backup url...", ex);

logger.Error("Request failed using backup url. Operation aborted.", ex);
```

---

### Logs can add additional information with an anonymous extended properties object:

```csharp
if (logger.IsDebugEnabled())
    logger.Debug("Entered Foo method.", new { bar = barParameter });

logger.Error("Request failed, operation aborted.", ex, new { ContentType = context.Request.ContentType, Path = context.Request.Path });
```
*Note that any object can be used for extended properties - each public property is added to the log's extended properties.*

---

### If extended properties need to be built up, use a dictionary:

```csharp
if (logger.IsDebugEnabled())
{
    Dictionary<string, object> extendedProperties = new Dictionary<string, object>();

    extendedProperties.Add("bar", barParameter);

    logger.Debug("Entered Foo method.", extendedProperties);
}

Dictionary<string, object> extendedProperties = new Dictionary<string, object>();

extendedProperties.Add("ContentType", context.Request.ContentType);
extendedProperties.Add("Path", context.Request.Path);

logger.Error("Request failed, operation aborted.", ex, extendedProperties);
```

---

### Include a correlation ID, business process ID, business activity ID:

```csharp
string correlationId = ...;
string businessProcessId = ...;
string businessActivityId = ...;

logger.Info("Received Foo request.", correlationId: correlationId, businessProcessId: businessProcessId, businessActivityId: businessActivityId);
```

---

### Use the _Log_ method directory
Instantiate a _LogEntry_ object and pass it in:

```c#
LogEntry logEntry = new LogEntry("Foo method starting...", LogLevel.Debug, new { bar = "abc" });

logger.Log(logEntry);
```
