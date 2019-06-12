# Context Providers

By implementing the `RockLib.Logging.IContextProvider` interface, defining the `AddContext` method, and providing it to a `Logger`, you are able to automatically modify a `LogEntry` whenever it is written to its destination.

The following implementation of `IContextProvider` will add the version of the operating system (OS) under which the application is running as an extended property to any `LogEntry` emitted by the logger:

```C#
   public class OsContextProvider : IContextProvider
    {
        public void AddContext(LogEntry logEntry)
        {
            logEntry.SetExtendedProperties(new {OSVersion = System.Environment.OSVersion.VersionString});
        }
    }
```

Context providers are passed to the `Logger` upon instantiation via an array of type `IContextProvider`. For example, the following code will provide a new instance of the above context provider to a new `Logger`:

```C#
var logger = new Logger(contextProviders: new [] { new OsContextProvider() });
```

Alternatively, context providers can be assigned to a `Logger` by means of configuration. In the following example, we will assign our context provider to the default `Logger`:

```C#
{
  "RockLib.Logging": {
    "LogProviders": {
        "Type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging"
    },
    "ContextProviders": {
        "Type": "MyProject.OsContextProvider, MyProject"
    }
  }
}
```
