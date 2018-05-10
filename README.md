# RockLib.Logging

*A simple logging library.*

```powershell
PM> Install-Package RockLib.Logging -IncludePrerelease
```

### Table of contents

- [Quick start](#quick-start)
- [ILogger interface](#ilogger-interface)
  - [Extension methods](#extension-methods)
- [Logger class](#logger-class)
- [LogEntry class](#logentry-class)
- [LoggerFactory class](#loggerfactory-class)
  - [Configuration](#configuration)
- [ILogProvider interface](#ilogprovider-interface)

## Quick start

Add a `rocklib.logging` section to your appsettings.json:

```json
"rocklib.logging": {
    "Level": "Info",
    "Providers": { "type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging" }
}
```

Use the `LoggerFactory` class to create instances of the `Logger` class:

```c#
using RockLib.Logging;

Logger logger = LoggerFactory.GetInstance();
```

Inject the `ILogger` interface into your classes (the `Logger` class implements the `ILogger` interface):

```c#
using RockLib.Logging;

public class MyService
{
    private readonly ILogger _logger;
    public MyService(ILogger logger) => _logger = logger;
}
```

With an instance of `ILogger`, call the various [extension methods](#extension-methods): `Debug`, `Info`, `Warn`, `Error`, `Fatal`, and `Audit`. If initializing for a logging operation is expensive, use the `IsDebugEnabled`, `IsInfoEnabled`, `IsWarnEnabled`, `IsErrorEnabled`, `IsFatalEnabled`, or `IsAuditEnabled` extension methods.

```c#
using RockLib.Logging;

var logger = LoggerFactory.GetInstance();

logger.Info("Hello, world!", new { foo = "bar", baz = true });

try
{
    int i = 1, j = 0;
    int k = i / j;
}
catch (Exception ex)
{
    logger.Error("An error occurred in the example code.", ex);
}

if (logger.IsDebugEnabled())
{
    var message = "Some expensive message";
    var extendedProperties = new Dictionary<string, object>();
    // TODO: add lots of expensive items to the dictionary
    logger.Debug(message, extendedProperties);
}
```

## `ILogger` interface

The main interface in the RockLib.Logging package is the `ILogger` interface. It exists in order to make dependency injection and loose coupling easier in applications. It also enables extension methods to be defined that work on any implementation of `ILogger`.

### Extension methods

RockLib.Logging defines a number of extension methods for the `ILogger` interface that make logging easier. There are two versions of the primary extension methods, depending on whether you have an exception or not.

```c#
public static void Debug(this ILogger logger, string message, object extendedProperties = null);
public static void Info(this ILogger logger, string message, object extendedProperties = null);
public static void Warn(this ILogger logger, string message, object extendedProperties = null);
public static void Error(this ILogger logger, string message, object extendedProperties = null);
public static void Fatal(this ILogger logger, string message, object extendedProperties = null);
public static void Audit(this ILogger logger, string message, object extendedProperties = null);

public static void Debug(this ILogger logger, string message, Exception exception, object extendedProperties = null);
public static void Info(this ILogger logger, string message, Exception exception, object extendedProperties = null);
public static void Warn(this ILogger logger, string message, Exception exception, object extendedProperties = null);
public static void Error(this ILogger logger, string message, Exception exception, object extendedProperties = null);
public static void Fatal(this ILogger logger, string message, Exception exception, object extendedProperties = null);
public static void Audit(this ILogger logger, string message, Exception exception, object extendedProperties = null);
```

The optional `extendedProperties` parameter object in each logging method is ultimately mapped to a `LogEntry.ExtendedProperties` with the `LogEntry.SetExtendedProperties(object extendedProperties)` method. See the [LogEntry class](#logentry-class) for details.

If a the creation of the message for a log is expensive, or if the adding of necessary extended properties is expensive, logging operations can be checked so that the expensive logging is only executed if the specifiec log level is met by the logger.

```c#
public static bool IsDebugEnabled(this ILogger logger)
public static bool IsInfoEnabled(this ILogger logger)
public static bool IsWarnEnabled(this ILogger logger)
public static bool IsErrorEnabled(this ILogger logger)
public static bool IsFatalEnabled(this ILogger logger)
public static bool IsAuditEnabled(this ILogger logger)
```

## `Logger` class

The `Logger` class defines the logging logic in the RockLib.Logging package and implements the `ILogger` interface. The class defines a handful of settings along with a collection of `ILogProvider` objects. When its `public void Log(LogEntry logEntry)` method is invoked, it acts as gatekeeper. It only continues if the `IsDisabled` property is false and the log entry's `Level` is greater than or equal to the logger's `Level`. If the log entry passes these gates, it is sent to each of the logger's log providers. For each log provider, the log entry's `Level` must be greater than or equal to a its `Level`.

When a log entry is passed to a log provider's `Task WriteAsync(LogEntry logEntry, CancellationToken token)` method, the resulting task is not waited on. Instead, control is returned immediately to the caller, causing a miminal impact on performance. Before returning, the log provider's task is added to a thread-safe collection, where it is ultimately tracked by a background thread. The purpose of the background thread is to ensure that when the logger's `Dispose` method is called, any in-flight tasks can be waited on before shutting the logger down. I.e., applications won't lose the logs that were being sent right before shutdown.

## `LogEntry` class

The `LogEntry` class is a data class that contains information about a logging operation. While many of the properties of the class are automatically populated upon creation, all values can be changed.

In addition to properties, the `LogEntry` class provides two methods. `LogEntry.GetExceptionData()` returns a formatted string representing the value of the `Exception` property (it returns null if `Exception` is null).

The second method, `LogEntry.SetExtendedProperties(object extendedProperties)`, maps its parameter to the `ExtendedProperties` property. If the object is assignable to `IDictionary<string, object>`, then each item in that dictionary is mapped to an `ExtendedProperty` item. Otherwise, each public instance property of the object are mapped to an `ExtendedProperty` item.

## `LoggerFactory` class

The `LoggerFactory` class provides a central location to retrieve `Logger` objects by name in a case-insensitive manner. Its source collection of loggers comes from configuration by default, but can be set programmatically.

```c#
// Gets the default logger (i.e. the one named "default").
Logger defaultLogger = LoggerFactory.GetInstance();

// Gets the logger named "someName".
Logger namedLogger = LoggerFactory.GetInstance("someName");
```

To set the loggers programmatically, call the `SetLoggers` method at the beginning of your application (the best place is the `Main` method).

```c#
using RockLib.Logging;

class Program
{
    void Main(string args)
    {
        LoggerFactory.SetLoggers(new[] { new ConsoleLogProvider() });

        // TODO: The rest of the application
    }
}
```

### Configuration

The `LoggerFactory` class retrieves its default loggers from configuration using the [RockLib.Configuration](https://github.com/RockLib/RockLib.Configuration/tree/develop/RockLib.Configuration) and [RockLib.Configuration.ObjectFactory](https://github.com/RockLib/RockLib.Configuration/tree/develop/RockLib.Configuration.ObjectFactory) packages. The easiest way add logging to your application by configuration is with an `appsettings.json` file.

*This is a simple configuration defining a single logger with a single log provider.*

```json
{
    "rocklib.logging": {
        "Level": "Error",
        "Providers": { "type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging" }
    }
}
```

*This is a more complex configuration.*

```json
{
    "rocklib.logging": [
        {
            "Level": "Error",
            "Providers": [
                {
                    "type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging",
                    "value": {
                        "template": "{createTime(O)}:{level}:{message}"
                    }
                },
                {
                    "type": "My.Custom.RestLogProvider, MyAssembly",
                    "value": {
                        "endPoint": "https://some/url/"
                    }
                }
            ]
        },
        {
            "Name": "TargetedLogger",
            "Level": "Debug",
            "Providers": { "type": "Another.Custom.LogProvider, Some.Other.Assembly" }
        }
    ]
}
```

For reference, this is the actual code that is used to create the default loggers in the `LoggerFactory` class:

```c#
using RockLib.Configuration;
using RockLib.Configuration.ObjectFactory;
using RockLib.Logging;

Config.Root.GetSection("rocklib.logging").Create<IReadOnlyCollection<Logger>>()
```

## `ILogProvider` interface

The `ILogProvider` interface is the primary mechanism for extinsibility in the RockLib.Logging package, allowing any implementation of the interface to work seamlessly within the logging library. Currently, the package offers three implementations of the interface: `ConsoleLogProvider`, `FileLogProvider`, and `RollingFileLogProvider`. It is expected that many users of the library will create their own implementation of the interface in order to send their logs to their proprietary logging backend (such as loggly, logstash, or splunk).