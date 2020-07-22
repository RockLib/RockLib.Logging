# How to add RockLib logging to the Microsoft dependency injection system

To add an `ILogger` to the Microsoft.Extensions.DependencyInjection system, call the `.AddLogger()` extension method on an instance of `IServiceCollection`. This extension method return an [`ILoggerBuilder`](#iloggerbuilder-interface) object, which is used to add log providers and context providers to the logger.

There are three overloads of this extension method. The difference between them is how the logger's log processor is registered.

---

| Parameter        | Required | Default     | Type                     | Description                                                                               |
|:-----------------|:---------|:------------|:-------------------------|:------------------------------------------------------------------------------------------|
| logProcessor     | Yes      | N/A         | `ILogProcessor`          | The object that will process log entries on behalf of the logger.                         |
| loggerName       | No       | `"default"` | `string`                 | The name of the logger to build.                                                          |
| configureOptions | No       | `null`      | `Action<ILoggerOptions>` | A delegate to configure the `ILoggerOptions` object that is used to configure the logger. |
| lifetime         | No       | `Transient` | `ServiceLifetime`        | The `ServiceLifetime` of the service.                                                     |

---

| Parameter                | Required | Default     | Type                                    | Description                                                                                          |
|:-------------------------|:---------|:------------|:----------------------------------------|:-----------------------------------------------------------------------------------------------------|
| logProcessorRegistration | Yes      | N/A         | `Func<IServiceProvider, ILogProcessor>` | The method used to create the `ILogProcessor` that will process log entries on behalf of the logger. |
| loggerName               | No       | `"default"` | `string`                                | The name of the logger to build.                                                                     |
| configureOptions         | No       | `null`      | `Action<ILoggerOptions>`                | A delegate to configure the `ILoggerOptions` object that is used to configure the logger.            |
| lifetime                 | No       | `Transient` | `ServiceLifetime`                       | The `ServiceLifetime` of the service.                                                                |

---

| Parameter        | Required | Default      | Type                     | Description                                                                               |
|:-----------------|:---------|:-------------|:-------------------------|:------------------------------------------------------------------------------------------|
| loggerName       | No       | `"default"`  | `string`                 | The name of the logger to build.                                                          |
| configureOptions | No       | `null`       | `Action<ILoggerOptions>` | A delegate to configure the `ILoggerOptions` object that is used to configure the logger. |
| processingMode   | No       | `Background` | `ProcessingMode`         | A value that indicates how the logger will process logs.                                  |
| lifetime         | No       | `Transient`  | `ServiceLifetime`        | The `ServiceLifetime` of the service.                                                     |

## ILoggerBuilder interface

This is the definition of the `ILoggerBuilder` interface:

```c#
public interface ILoggerBuilder
{
    string LoggerName { get; }

    ILoggerBuilder AddLogProvider(Func<IServiceProvider, ILogProvider> logProviderRegistration);

    ILoggerBuilder AddContextProvider(Func<IServiceProvider, IContextProvider> contextProviderRegistration);
}
```

#### LoggerName property
The name of the logger to build.

#### AddLogProvider method
Adds an `ILogProvider` to the logger. The `ILogProvider` is instantiated with the `logProviderRegistration` callback.

| Parameter               | Required | Type   | Description                               |
|:------------------------|:---------|:-------|:------------------------------------------|
| logProviderRegistration | Yes      | `Func` | A method that creates the `ILogProvider`. |

#### AddContextProvider method
Adds an `IContextProvider` to the logger. The `IContextProvider` is instantiated with the `contextProviderRegistration` callback.

| Parameter                   | Required | Type                                       | Description                                   |
|:----------------------------|:---------|:-------------------------------------------|:----------------------------------------------|
| contextProviderRegistration | Yes      | `Func<IServiceProvider, IContextProvider>` | A method that creates the `IContextProvider`. |

## ILoggerBuilder extension methods

The following generic extension methods for `ILoggerBuilder` are defined:

```c#
ILoggerBuilder AddLogProvider<TLogProvider>(this ILoggerBuilder builder, params object[] parameters)
    where TLogProvider : ILogProvider;

ILoggerBuilder AddContextProvider<TContextProvider>(this ILoggerBuilder builder, params object[] parameters)
    where TContextProvider : IContextProvider;
```

#### AddLogProvider\<TLogProvider\> extension method
Adds an `ILogProvider` of type `TLogProvider` to the logger. The `TLogProvider` is instantiated with constructor arguments directly and/or from an `IServiceProvider`.

| Parameter  | Type              | Description                                                                                    |
|:-----------|:------------------|:-----------------------------------------------------------------------------------------------|
| parameters | `params object[]` | Constructor arguments for type `TLogProvider` that are not provided by the `IServiceProvider`. |

#### AddContextProvider extension method
Adds an `IContextProvider` of type `TContextProvider` to the logger. The `TContextProvider` is instantiated with constructor arguments directlry and/or from an `IServiceProvider`.

| Parameter  | Type              | Description                                                                                        |
|:-----------|:------------------|:---------------------------------------------------------------------------------------------------|
| parameters | `params object[]` | Constructor arguments for type `TContextProvider` that are not provided by the `IServiceProvider`. |
