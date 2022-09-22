# How to configure and use LoggerFactory

Logger instances can be created and retrieved using the static `LoggerFactory` class. This class uses a `Microsoft.Extensions.Configuration.IConfiguration` that defines the loggers it can create. It has methods for creating logger instances, where a new logger is returned each time. It also has methods for getting a cached logger, where the same logger is returned each time. Loggers can be created/retrieved by name. If the name is not specified, the "default" (unnamed) logger is created/retrieved.

## Specifying configuration

The `LoggerFactory.Configuration` property defines the configuration that `LoggerFactory` uses. By default, this property has a value obtained by calling  `RockLib.Configuration.Config.Root.GetCompositeSection("RockLib_Logging", "RockLib.Logging")`. Configuration can be changed by calling the `LoggerFactory.SetConfiguration` method. The configuration passed to the `SetConfiguration` method must follow the format of the `"RockLib.Logging"` configuration section as described below.

An easy mistake to make when specifying configuration is to pass in the configuration root when logging is defined in a sub-section. In this case, be sure to call `GetSection` on the configuration root, passing in the name of the logging sub-section.

## Configuration format

The `LoggerFactory.Configuration` property should contain one or more objects of type `Logger`, specifically its constructor parameters (see the [Logger docs](Logger.md) for a listing of all constructor parameters) and read/write properties.

---

This `appsettings.json` file has a default (unnamed) logger object defined:

```json
{
  "RockLib.Logging": {
    "LogProviders": {
      "Type": "RockLib.Logging.FileLogProvider, RockLib.Logging",
      "Value": { "File": "example_app.log" }
    }
  }
}
```

---

This `appsettings.json` shows multiple logger objects. Note that the first logger is unnamed, while the other is named `"File"`:

```json
{
  "RockLib.Logging": [
    {
      "LogProviders": { "Type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging" }
    },
    {
      "Name": "File",
      "Level": "Warn",
      "LogProviders": {
        "Type": "RockLib.Logging.FileLogProvider, RockLib.Logging",
        "Value": { "File": "example_app.log" }
      }
    }
  ]
}
```

---

A logger with more than one log provider can be specified:

```json
{
  "RockLib.Logging": {
    "LogProviders": [
      { "Type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging" },
      {
        "Type": "RockLib.Logging.FileLogProvider, RockLib.Logging",
        "Value": { "File": "example_app.log" }
      }
    ]
  }
}
```

---

This is a configuration that has a logger with all settings specified:

```json
{
  "RockLib.Logging": {
    "Name": "Example",
    "Level": "Error",
    "LogProviders": { "Type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging" },
    "IsDisabled": false,
    "ProcessingMode": "FireAndForget",
    "ContextProviders": { "Type": "MyProject.MyContextProvider, MyProject" },
    "ErrorHandler": { "Type": "MyProject.MyErrorHandler, MyProject" }
  }
}
```
> This configuration assumes there is an assembly loaded named "MyProject" that has a `MyProject.MyContextProvider` class that implements `RockLib.Logging.IContextProvider` and a `MyProject.MyErrorHandler` class that implements `RockLib.Logging.IErrorHandler`.

## Create method

The `Create` method creates a new logger every time it is called, regardless of the `name` parameter. If the `name` parameter is provided, then `LoggerFactory` creates a logger from the first one defined in its configuration with a matching name. Otherwise, a "default" logger is created, defined by the first logger in configuration without a `name` (or with a `name` of `"default"`).

```csharp
ILogger defaultLogger = LoggerFactory.Create(); // Creates a default logger.
ILogger fooLogger = LoggerFactory.Create("foo"); // Creates a logger named "foo".
```

## GetCached method

The `GetCached` method returns a cached logger. Each call with the same name (or each call without a name) returns the *same* logger instance. Note that this is done lazily - `LoggerFactory` doesn't create the logger until it is requested for the first time.

```csharp
ILogger defaultLogger = LoggerFactory.GetCached(); // Gets or creates the cached default logger.
ILogger fooLogger = LoggerFactory.GetCached("foo"); // Gets or creates the cached logger named "foo".
```
