---
sidebar_position: 20
---

# Changing a logger's settings "on the fly"

It is generally a bad idea to run an application in production while logging at the `Debug` (or arguably the `Info`) level. Especially when the logging comes at a price per log. Millions or billions of debug or info logs could be quite expensive. On the other hand, production issues can and do arise, and having those `Debug` or `Info` logs available for diagnostics is incredibly valuable. A solution to this problem is to normally log at `Warn`, but have the ability to change the level to `Info` or `Debug` on-the-fly, should the need arise.

To accomplish this, RockLib.Logging binds to configuration. Microsoft.Extensions.Configuration supports automatically reloading itself when its settings change - this is the mechanism that RockLib hooks into to keep its loggers up-to-date. The Json congiruation provider supports this reloading capability, so it is easy to verify that your loggers have the most up-to-date settings on a developer machine: edit the appsettings.json file while the app is running and set a breakpoint to examine your logger - it should stay up-to-date.

### Up-to-date Loggers

When registering and defining a logger with DI extension methods, in order to have up-to-date loggers, bind the logger's `LoggerOptions` to a configuration section:

```csharp
services.AddLogger().AddConsoleLogProvider();
services.Configure<LoggerOptions>(Configuration.GetSection("MyLogger"));
```

```json
{
  "MyLogger": {
    "Level": "Warn"
  }
}
```

---

It is also possible to register a logger with DI, but define it in configuration using a standard `LoggerFactory` configuration section. In this case, it is not necessary to bind options.

```csharp
// An empty logger builder will create the logger defined in configuration using COF.
services.AddLogger();
```

```json
{
  "Rocklib.Logging": {
    "Level": "Warn",
    "Providers": { "Type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging" }
  }
}
```

### Reloading Loggers

For applications with short-lived, transient or scopped loggers (like ASP.NET Core), there's nothing else to do. Since loggers are created at the time they are used, and since they are created with the most recent configuration, they will always be up-to-date. However, for singleton or other long-lived loggers, we need reloading loggers - they should be notified of configuration changes so they can update themselves accordingly.

---

When registering and defining a logger with DI, in order to have reloading loggers, make sure the `ReloadOnChange` setting of the options is true:

```csharp
services.AddLogger().AddConsoleLogProvider();
services.Configure<LoggerOptions>(Configuration.GetSection("MyLogger"));
```

```json
{
  "MyLogger": {
    "ReloadOnChange": true,
    "Level": "Warn"
  }
}
```

---

When registering with DI and defining in configuration with a `LoggerFactory` section, specifiy the `reloadOnChange` parameter like this:

```json
{
  "Rocklib.Logging": {
    "ReloadOnChange": true,
    "Value": {
      "Level": "Warn",
      "Providers": { "Type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging" }
    }
  }
}
```

---

When using LoggerFactory or the extension methods defined by the LoggerFactoryExtensions class, there is a parameter, `reloadOnConfigChange`, which is true by default. Set the parameter to false to *disable* reloading.

```csharp
ILogger logger = LoggerFactory.CreateLogger(reloadOnConfigChange: false);
```
