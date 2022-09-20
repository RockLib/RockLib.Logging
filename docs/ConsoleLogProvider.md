# How to use and configure _ConsoleLogProvider_

The `ConsoleLogProvider` can be instantiated in one of two ways.

## Instantiate with a template:

**template**
  - Type: string
  - Description: The template used to format log entries. See [TemplateLogFormatter](Formatting.md#template) for more information.

**level**
  - Type: LogLevel enum (NotSet, Debug, Info, Warn, Error, Fatal, Audit)
  - Description: The level of the log provider. If `NotSet`, the level of the containing `Logger` is used.

**output**
  - Type: Output enum (StdOut, StdErr)
  - Description: The type of output stream to use.

**timeout**
  - Type: Nullable\<TimeSpan\>
  - Description: The timeout of the log provider.

## Instantiate with a formatter:

**formatter**
  - Type: ILogFormatter
  - Description: An object that formats log entries prior to writing to standard out. See [ILogFormatter](Formatting.md#ilogformatter) for more information.

**level**
  - Type: LogLevel enum (NotSet, Debug, Info, Warn, Error, Fatal, Audit)
  - Description: The level of the log provider. If `NotSet`, the level of the containing `Logger` is used.

**output**
  - Type: Output enum (StdOut, StdErr)
  - Description: The type of output stream to use.

**timeout**
  - Type: Nullable\<TimeSpan\>
  - Description: The timeout of the log provider.

## Adding to Dependency Injection

To add a `ConsoleLogProvider` to a logger, call one of the five `AddConsoleLogProvider` extension method overloads. Four of the overloads are very similar, differing only by how the `ILogFormatter` is specified.

---

## Examples

### Specifying a custom template:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddLogger()
        .AddConsoleLogProvider(
            template:   "{level}: {message}",
            level:      LogLevel.Info,
            output:     ConsoleLogProvider.Output.StdOut,
            timeout:    TimeSpan.FromSeconds(1));
}
```
---

### Specifying a custom _ILogFormatter_:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    IDependency dependency = new MyDependency();
    int parameter = 12345;

    ILogFormatter formatter = new CustomLogFormatter(dependency, parameter);

    services.AddLogger()
        .AddConsoleLogProvider(
            formatter:  formatter,
            level:      LogLevel.Info,
            output:     ConsoleLogProvider.Output.StdOut,
            timeout:    TimeSpan.FromSeconds(1));
}
```
---

### Specifying a custom _ILogFormatter_ generically including custom constructor parameters:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddTransient<IDependency, MyDependency>();

    services.AddLogger()
        .AddConsoleLogProvider<CustomLogFormatter>(
            level:                  LogLevel.Info,
            output:                 ConsoleLogProvider.Output.StdOut,
            timeout:                TimeSpan.FromSeconds(1),
            logFormatterParameters: 12345);
}
```
---

### Specifying a custom `ILogFormatter` with a callback function:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddLogger()
        .AddConsoleLogProvider(
            formatterRegistration:  serviceProvider => new CustomLogFormatter(new MyDependency(), 12345),
            level:                  LogLevel.Info,
            output:                 ConsoleLogProvider.Output.StdOut,
            timeout:                TimeSpan.FromSeconds(1));
}
```
---

### Fully customizing the console log provider with a callback function:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddLogger()
        .AddConsoleLogProvider(options =>
        {
            // Could also set the formatter with one of the other
            // SetFormatter overloads or the SetTemplate method.
            options.SetFormatter<CustomLogFormatter>(12345);
            options.Level = LogLevel.Info;
            options.Output = ConsoleLogProvider.Output.StdOut;
            options.Timeout = TimeSpan.FromSeconds(1);
        });
}
```

## Configuration for _ConsoleLogProvider_

### Specifying a template:

```json
{
  "Rocklib.Logging": {
    "Providers": [
      {
        "Type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging",
        "Value": {
          "Template": "{level}: {message}",
          "Level": "Info",
          "Output": "StdOut",
          "Timeout": "00:00:01"
        }
      }
    ]
  }
}
```

### Specifying an _ILogFormatter_:

```json
{
  "Rocklib.Logging": {
    "Providers": [
      {
        "Type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging",
        "Value": {
          "Formatter": {
            "Type": "MyAssembly.CustomLogFormatter, MyAssembly",
            "Value": {
              "Dependency": {
                "Type": "MyAssembly.MyDependency, MyAssembly"
              },
              "Parameter": 12345
            }
          },
          "Level": "Info",
          "Output": "StdOut",
          "Timeout": "00:00:01"
        }
      }
    ]
  }
}
```
