---
sidebar_position: 10
sidebar_label: 'How to use and configure FileLogProvider'
---

# How to use and configure _FileLogProvider_

The `FileLogProvider` can be instantiated in one of two ways.

## With a template:
**file**
  - Type: string
  - Description: The path to a writable file.

**template**
  - Type: string
  - Description: The template used to format log entries. See [TemplateLogFormatter](Formatting.md#template) for more information.

**level**
  - Type: LogLevel enum (NotSet, Debug, Info, Warn, Error, Fatal, Audit)
  - Description: The level of the log provider. If `NotSet`, the level of the containing `Logger` is used.

**timeout**
  - Type: Nullable\<TimeSpan\>
  - Description: The timeout of the log provider.

## With a formatter:
**file**
  - Type: string
  - Description: The path to a writable file.

**formatter**
  - Type: ILogFormatter
  - Description: An object that formats log entries prior to writing to standard out. See [ILogFormatter](Formatting.md#ilogformatter) for more information.

**level**
  - Type: LogLevel enum (NotSet, Debug, Info, Warn, Error, Fatal, Audit)
  - Description: The level of the log provider. If `NotSet`, the level of the containing `Logger` is used.

**timeout**
  - Type: Nullable\<TimeSpan\>
  - Description: The timeout of the log provider.

## Adding to Dependency Injection

To add a `FileLogProvider` to a logger, call one of the five `AddFileLogProvider` extension method overloads. Four of the overloads are very similar, differing only by how the `ILogFormatter` is specified.

---

### Specifying a custom template:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddLogger()
        .AddFileLogProvider(
            file:       "C:\\my\\path\\log.txt",
            template:   "{level}: {message}",
            level:      LogLevel.Info,
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
        .AddFileLogProvider(
            file:       "C:\\my\\path\\log.txt",
            formatter:  formatter,
            level:      LogLevel.Info,
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
        .AddFileLogProvider<CustomLogFormatter>(
            file:                   "C:\\my\\path\\log.txt",
            level:                  LogLevel.Info,
            timeout:                TimeSpan.FromSeconds(1),
            logFormatterParameters: 12345);
}
```
---

### Specifying a custom _ILogFormatter_ with a callback function:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddLogger()
        .AddFileLogProvider(
            file:                   "C:\\my\\path\\log.txt",
            formatterRegistration:  serviceProvider => new CustomLogFormatter(new MyDependency(), 12345),
            level:                  LogLevel.Info,
            timeout:                TimeSpan.FromSeconds(1));
}
```
---

### Fully customizing the file log provider with a callback function:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddLogger()
        .AddFileLogProvider(options =>
        {
            options.File = "C:\\my\\path\\log.txt";
            // Could also set the formatter with one of the other
            // SetFormatter overloads or the SetTemplate method.
            options.SetFormatter<CustomLogFormatter>(12345);
            options.Level = LogLevel.Info;
            options.Timeout = TimeSpan.FromSeconds(1);
        });
}
```

## Configuration for _FileLogProvider_

### Specifying a template:

```json
{
  "Rocklib.Logging": {
    "Providers": [
      {
        "Type": "RockLib.Logging.FileLogProvider, RockLib.Logging",
        "Value": {
          "File": "C:\\my\\path\\log.txt",
          "Template": "{level}: {message}",
          "Level": "Info",
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
        "Type": "RockLib.Logging.FileLogProvider, RockLib.Logging",
        "Value": {
          "File": "C:\\my\\path\\log.txt",
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
          "Timeout": "00:00:01"
        }
      }
    ]
  }
}
```
