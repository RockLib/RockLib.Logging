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
  - [ConsoleLogProvider class](#consolelogprovider-class)
  - [FileLogProvider class](#filelogprovider-class)
  - [RollingFileLogProvider class](#rollingfilelogprovider-class)
- [ILogFormatter interface](#ilogformatter-interface)
  - [TemplateLogFormatter class](#templatelogformatter-class)
    - [Template Examples](#template-examples)

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

The `Logger` class defines the logging logic in the RockLib.Logging package and implements the `ILogger` interface. The class defines a handful of settings properties along with a `Providers` property. This property contains a collection of `ILogProvider` objects, and together, they define exactly what happens to a log entry when it is logged.

When a logger's `public void Log(LogEntry)` method is invoked, it acts as gatekeeper. It only continues if its `IsDisabled` property is false and the log entry's `Level` is greater than or equal to the logger's `Level`. If the log entry passes these checks, each log provider may optionally act as an additional gatekeeper - the log entry's `Level` must be greater than or equal to a log provider's `Level` as well. For each log provider that passes all of checks, the log entry is passed to its `Task ILogProvider.WriteAsync(LogEntry, CancellationToken)` method. The resulting task is tracked in a background thread, but not waited on locally. This ensures that the `Logger` has minimal performance impact on applications.

Instances of the `Logger` class are intended to be long-lived, and all of its methods are thread-safe. Each instance is somewhat expensive to initialize and takes up a fair amount of memory (because of the background thread), so be sure to reuse it.

At the end of an application, before it exits, it is important to call `Dispose` on the logger. This blocks until all asynchronously processing logging operations have completed. If the logger is not disposed, when the application shuts down, any in-flight logging operations may be lost or only sent to some of the log providers.

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

When the application is shutting down, the `LoggerFactory.ShutDown` method should be called. This method disposes each logger in the `LoggerFactory.Loggers` property, blocking until all pending logging operations are completed.

```c#
try
{
    // TODO: Run the application.
}
finally
{
    LoggerFactory.ShutDown();
}
```

To set the loggers programmatically, call the `SetLoggers` method at the beginning of your application (the best place is the `Main` method).

```c#
using RockLib.Logging;

class Program
{
    void Main(string args)
    {
        LoggerFactory.SetLoggers(new[] { new ConsoleLogProvider() });

        try
        {
            // TODO: Run the application
        }
        finally
        {
            LoggerFactory.ShutDown();
        }
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
            "Level": "Warn",
            "Providers": [
                {
                    "type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging",
                    "value": {
                        "template": "{createTime(O)}:{level}:{message}"
                    }
                },
                {
                    "type": "RockLib.Logging.RollingFileLogProvider, RockLib.Logging",
                    "value": {
                        "file": "test.log",
                        "template": "{createTime(O)}:{level}:{message}",
                        "maxFileSizeKilobytes": 1048576,
                        "maxArchiveCount": 1827,
                        "rolloverPeriod": "Daily"
                    }
                }
            ]
        },
        {
            "Name": "TargetedLogger",
            "Level": "Debug",
            "Providers": {
                "type": "Custom.LogProvider, Some.Assembly",
                "value": {
                    "endPoint": "https://some/url/"
                }
            }
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

```c#
public interface ILogProvider
{
    TimeSpan Timeout { get; }

    LogLevel Level { get; }

    Task WriteAsync(LogEntry logEntry, CancellationToken cancellationToken);
}
```

The `Logger` class waits on the task returned from the `WriteAsync` method for the length of time defined by the `Timeout` property. If the task is not completed by that time, the task is cancelled.

The `Level` property allows an instance of `ILogProvider` to have a higher logging level than its `Logger`.

### `ConsoleLogProvider` class

The `ConsoleLogProvider` class writes a log entry to standard out according to its [`ILogFormatter`](#ilogformatter-interface).

```c#
public const string DefaultTemplate;

// Constructor 1
public ConsoleLogProvider(
    string template = DefaultTemplate,
    LogLevel level = default(LogLevel),
    TimeSpan? timeout = null)
    : this(new TemplateLogFormatter(template), level, timeout);

// Constructor 2
public ConsoleLogProvider(
    ILogFormatter formatter,
    LogLevel level = default(LogLevel),
    TimeSpan? timeout = null);
```

These constructors allow the following configuration snippets to be used to instantiate a `ConsoleLogProvider`.

*Calls Constructor 1 with minimal parameters specified:*

```json
{
    "type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging"
}
```

*Calls Constructor 1 with all parameters specified:*

```json
{
    "type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging",
    "value": {
        "template": "{createTime(O)}:{level}:{message}",
        "level": "Info",
        "timeout": "00:00:05"
    }
}
```

*Calls Constructor 2 with minimal parameters specified:*

```json
{
    "type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging",
    "value": {
        "formatter": {
            "type": "RockLib.Logging.TemplateLogFormatter, RockLib.Logging",
            "value": {
                "template": "{createTime(O)}:{level}:{message}"
            }
        }
    }
}
```

*Calls Constructor 2 with all parameters specified:*

```json
{
    "type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging",
    "value": {
        "formatter": {
            "type": "RockLib.Logging.TemplateLogFormatter, RockLib.Logging",
            "value": {
                "template": "{createTime(O)}:{level}:{message}"
            }
        },
        "level": "Info",
        "timeout": "00:00:05"
    }
}
```

### `FileLogProvider` class

The `FileLogProvider` class writes a log entry to a file according to its [`ILogFormatter`](#ilogformatter-interface).

```c#
public const string DefaultTemplate;

// Constructor 1
public FileLogProvider(
    string file,
    string template = DefaultTemplate,
    LogLevel level = default(LogLevel),
    TimeSpan? timeout = null)
    : this(file, new TemplateLogFormatter(template), level, timeout);

// Constructor 2
public FileLogProvider(
    string file,
    ILogFormatter formatter,
    LogLevel level = default(LogLevel),
    TimeSpan? timeout = null);
```

These constructors allow the following configuration snippets to be used to instantiate a `FileLogProvider`.

*Calls Constructor 1 with minimal parameters specified:*

```json
{
    "type": "RockLib.Logging.FileLogProvider, RockLib.Logging",
    "value": {
        "file": "c:/path/to/log.txt"
    }
}
```

*Calls Constructor 1 with all parameters specified:*

```json
{
    "type": "RockLib.Logging.FileLogProvider, RockLib.Logging",
    "value": {
        "file": "c:/path/to/log.txt",
        "template": "{createTime(O)}:{level}:{message}",
        "level": "Info",
        "timeout": "00:00:05"
    }
}
```

*Calls Constructor 2 with minimal parameters specified:*

```json
{
    "type": "RockLib.Logging.FileLogProvider, RockLib.Logging",
    "value": {
        "file": "c:/path/to/log.txt",
        "formatter": {
            "type": "RockLib.Logging.TemplateLogFormatter, RockLib.Logging",
            "value": {
                "template": "{createTime(O)}:{level}:{message}"
            }
        }
    }
}
```

*Calls Constructor 2 with all parameters specified:*

```json
{
    "type": "RockLib.Logging.FileLogProvider, RockLib.Logging",
    "value": {
        "file": "c:/path/to/log.txt",
        "formatter": {
            "type": "RockLib.Logging.TemplateLogFormatter, RockLib.Logging",
            "value": {
                "template": "{createTime(O)}:{level}:{message}"
            }
        },
        "level": "Info",
        "timeout": "00:00:05"
    }
}
```

### `RollingFileLogProvider` class

The `RollingFileLogProvider` class writes a log entry to a file according to its [`ILogFormatter`](#ilogformatter-interface). If the file size reaches `MaxFileSizeKilobytes` or time has passed according to `RolloverPeriod`, then the current file is archived (i.e. renamed with numeric suffix). If the number of archive files exceeds `MaxArchiveCount`, then archive files are pruned, oldest first.

```c#
public const string DefaultTemplate;
public const int DefaultMaxFileSizeKilobytes;
public const int DefaultMaxArchiveCount;
public const RolloverPeriod DefaultRolloverPeriod;

// Constructor 1
public RollingFileLogProvider(
    string file,
    string template = DefaultTemplate,
    LogLevel level = default(LogLevel),
    TimeSpan? timeout = null,
    int maxFileSizeKilobytes = DefaultMaxFileSizeKilobytes,
    int maxArchiveCount = DefaultMaxArchiveCount,
    RolloverPeriod rolloverPeriod = DefaultRolloverPeriod)
    : this(file, new TemplateLogFormatter(template), level, timeout, maxFileSizeKilobytes, maxArchiveCount, rolloverPeriod);

// Constructor 2
public RollingFileLogProvider(
    string file,
    ILogFormatter formatter,
    LogLevel level = default(LogLevel),
    TimeSpan? timeout = null,
    int maxFileSizeKilobytes = DefaultMaxFileSizeKilobytes,
    int maxArchiveCount = DefaultMaxArchiveCount,
    RolloverPeriod rolloverPeriod = DefaultRolloverPeriod);
```

These constructors allow the following configuration snippets to be used to instantiate a `RollingFileLogProvider`.

*Calls Constructor 1 with minimal parameters specified:*

```json
{
    "type": "RockLib.Logging.RollingFileLogProvider, RockLib.Logging",
    "value": {
        "file": "c:/path/to/log.txt"
    }
}
```

*Calls Constructor 1 with all parameters specified:*

```json
{
    "type": "RockLib.Logging.RollingFileLogProvider, RockLib.Logging",
    "value": {
        "file": "c:/path/to/log.txt",
        "template": "{createTime(O)}:{level}:{message}",
        "level": "Info",
        "timeout": "00:00:05",
        "maxFileSizeKilobytes": 2048,
        "maxArchiveCount": 20,
        "rolloverPeriod": "Daily"
    }
}
```

*Calls Constructor 2 with minimal parameters specified:*

```json
{
    "type": "RockLib.Logging.RollingFileLogProvider, RockLib.Logging",
    "value": {
        "file": "c:/path/to/log.txt",
        "formatter": {
            "type": "RockLib.Logging.TemplateLogFormatter, RockLib.Logging",
            "value": {
                "template": "{createTime(O)}:{level}:{message}"
            }
        }
    }
}
```

*Calls Constructor 2 with all parameters specified:*

```json
{
    "type": "RockLib.Logging.RollingFileLogProvider, RockLib.Logging",
    "value": {
        "file": "c:/path/to/log.txt",
        "formatter": {
            "type": "RockLib.Logging.TemplateLogFormatter, RockLib.Logging",
            "value": {
                "template": "{createTime(O)}:{level}:{message}"
            }
        },
        "level": "Info",
        "timeout": "00:00:05",
        "maxFileSizeKilobytes": 2048,
        "maxArchiveCount": 20,
        "rolloverPeriod": "Daily"
    }
}
```

## `ILogFormatter` interface

This simple interface defines a method that converts a `LogEntry` to a `string`.

```c#
public interface ILogFormatter
{
    string Format(LogEntry entry);
}
```

### `TemplateLogFormatter` class

This implementation of `ILogFormatter` formats a log entry with a format string containing replacements tokens - text surrounded by curly braces. There are three types of tokens: simple, date/time, and extended properties.

These are the default simple tokens, each corresponding to a property of the `LogEntry` class:

- `{message}`
- `{level}`
- `{exception}`
  - Replaced with the return value of `LogEntry.GetExceptionData()`
- `{userName}`
- `{machineName}`
- `{machineIpAddress}`
- `{uniqueId}`
- `{newLine}`
- `{tab}`

There is only one date/time token: `{createTime}`. Date/time tokens have optional parentheses after the token name, containing a legal date/time format. For example `{createTime(O)}` would format a log entry's `CreateTime` with the standard "O" (round-trip) format.

The extended properties token is a little more complex. There are 4 variants.

1. `{extendedProperties(beginning of sub-template{key}sub-template, continued{value}end of sub-template)}`  
   For each extended property in the log entry, replace the sub-template's `{key}` and `{value}` tokens with the extended property key and value.
2. `{extendedProperties(beginning of sub-template{key}?sub-template, continued{value}end of sub-template)}`  
   Just like variant 1, except for a `?` after the `{key}` token. With this variant, the key is not rendered for any of the extended properties.
3. `{extendedProperties(beginning of sub-template{extended_property_name}sub-template, continued{value}end of sub-template)}`  
   Like variant 1, except it only renders a single specific extended property.
4. `{extendedProperties(beginning of sub-template{extended_property_name}sub-template, continued{value}end of sub-template)}`  
   A combination of variants 2 and 3. It renders only a single specific extended property and does not render the extended property key.

Applications can register additional simple tokens by calling the static `TemplateLogFormatter.AddSimpleTokenHandler` or `TemplateLogFormatter.AddExtendedPropertyTokenHandler` methods.

#### Template Examples

Each of the examples uses the following log entry:

```c#
var logEntry = new LogEntry(
    LogLevel.Info,
    "Hello, world!",
    new Exception("Uh-oh."),
    new { Foo = "abc", Bar = 123 });
```

| Description  | Template | Example Output |
| --- | --- | --- |
| Simple tokens | `"{level}: {message}{newLine}Exception: {exception}"`  | `"Info: Hello, world!`<br/>`Exception: Type: System.Exception`<br/>`Message: Uh-oh.`<br/>`Properties:`<br/>`   HResult: 0x80131500"` |
| DateTime token | `"Create time: {createTime(O)}"`  | `"Create time: 2018-05-17T03:12:38.4619092Z"` |
| Extended properies token, variant 1 | `"Extended Properties:{newLine}{extendedProperties(- {key}: {value})}"`  | `"Extended Properties:`<br/>`Foo: abc`<br/>`Bar: 123"` |
| Extended properies token, variant 2 | `"Extended Properties:{newLine}{extendedProperties(- {key}?{value})}"`  | `"Extended Properties:`<br/>`- abc`<br/>`- 123"` |
| Extended properies token, variant 3 | `"{extendedProperties(- {Foo}: {value})}"`  | `"- Foo: abc"` |
| Extended properies token, variant 4 | `"{extendedProperties(- {Foo}?{value})}"`  | `"- abc"` |