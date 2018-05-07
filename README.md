# RockLib.Logging

*A simple logging library.*

```powershell
PM> Install-Package RockLib.Logging
```

### Table of contents

- [Quick start](#quick-start)
- [ILogger interface](#ilogger-interface)
  - [Extension methods](#extension-methods)
- [Logger class](#logger-class)
- [LogEntry class](#logentry-class)
- [LoggerFactory class](#loggerfactory-class)
  - [Configuration](#configuration)

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

The main interface in the RockLib.Logging package is the `ILogger` interface.

### Extension methods

RockLib.Logging defines a number of extension methods to make logging easier.

## `Logger` class

The `Logger` class defines the logging logic in the RockLib.Logging package.

## `LogEntry` class

The `LogEntry` class is a data class that contains information about a logging operation.

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