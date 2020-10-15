# How to add RockLib logging to the Microsoft logging system

To add an `ILogger` to the Microsoft.Extensions.Logging system, call the `.AddRockLibLoggerProvider()` extension method on either an instance of `ILoggingBuilder` or `IServiceCollection`. The methods will return the same extended object, to allow for chaining.

There are four overloads of this extension method, two for `ILoggingBuilder` and two for `IServiceCollection`. The difference between the two from each type is the ability to pass in the logger name.

---

## `ILoggingBuilder` Method

The most common way to set this up is to use the `ConfigureLogging` method on a default host builder.

```c#
Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    })
    .ConfigureLogging(logging =>
    {
        logging.AddRockLibLoggerProvider(options => options.IncludeScopes = true);
    });
```

---

## `IServiceCollection` Method

An alternate way to set this up is to use the `ConfigureServices` start up method.

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddRockLibLoggerProvider();
}
```

## Extension Definitions

#### ILoggingBuilder Extension 1

| Parameter         | Required | Default | Type                              | Description                                                  |
|:------------------|:---------|:--------|:----------------------------------|:-------------------------------------------------------------|
| configureOptions  | Yes      | N/A     | `Action<RockLibLoggerOptions>` | A delegate to configure the `RockLibLoggerOptions` object. |

---

#### ILoggingBuilder Extension 2

| Parameter         | Required | Default      | Type                              | Description                                                  |
|:------------------|:---------|:-------------|:----------------------------------|:-------------------------------------------------------------|
| rockLibLoggerName | No       | `"default"` | `string`                         | The name of the `ILogger` that will ultimately records logs. |
| configureOptions  | No       | `null`       | `Action<RockLibLoggerOptions>` | A delegate to configure the `RockLibLoggerOptions` object. |

---

#### IServiceCollection Extension 1

| Parameter         | Required | Default | Type                              | Description                                                  |
|:------------------|:---------|:--------|:----------------------------------|:-------------------------------------------------------------|
| configureOptions  | Yes      | N/A     | `Action<RockLibLoggerOptions>` | A delegate to configure the `RockLibLoggerOptions` object. |

---

#### IServiceCollection Extension 2

| Parameter         | Required | Default      | Type                              | Description                                                  |
|:------------------|:---------|:-------------|:----------------------------------|:-------------------------------------------------------------|
| rockLibLoggerName | No       | `"default"` | `string`                         | The name of the `ILogger` that will ultimately records logs. |
| configureOptions  | No       | `null`       | `Action<RockLibLoggerOptions>` | A delegate to configure the `RockLibLoggerOptions` object. |

---


## `RockLibLoggerOptions` class

This is the definition of the `RockLibLoggerOptions` class:

```c#
public interface ILoggerBuilder
{
    public bool IncludeScopes { get; set; }
}
```

The `IncludeScopes` property determines wether or not a `ScopeProvider` will be used for creating scopes in the logger. Below is an example of setting this property:

```c#
logging.AddRockLibLoggerProvider(options => options.IncludeScopes = true);
```