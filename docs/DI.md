# How to add RockLib logging to the Microsoft dependency injection system

In order to make it easy to add RockLib logging to the Microsoft dependency injection system, the `RockLib.Logging.DependencyInjection` package is available from [nuget.org](https://www.nuget.org/packages/RockLib.Logging.DependencyInjection). Three extension methods are provided by the package, each extending the `Microsoft.Extensions.DependencyInjection.IServiceCollection` interface.

### AddRockLibLoggerTransient

This extension method adds a *transient* `ILogger` service, created with `LoggerFactory`, to the specified `IServiceCollection`.

Parameter                 | Required \| Default | Type              | Description
------------------------- | ------------------- | ----------------- | -----------
rockLibLoggerName         | No \| `"default"`   | `string`          | The name of the RockLib logger used for logging
defaultTypes              | No \| `null`        | `DefaultTypes`    | An object that defines the default types to be used when a type is not explicitly specified by a configuration section
valueConverters           | No \| `null`        | `ValueConverters` | An object that defines custom converter functions that are used to convert string configuration values to a target type
addLoggerProvider         | No \| `false`       | `bool`            | Whether to also add a singleton `ILoggerProvider` service with a `RockLibLoggerProvider` implementation to the service collection
addBackgroundLogProcessor | No \|  `true`       | `bool`            | Whether to also add a singleton `ILogProcessor` service with a `BackgroundLogProcessor` implementation to the service collection | `true`
reloadOnConfigChange      | No \| `false`       | `bool`            | Whether instances of `ILogger` created by the service collection will reload when their configuration changes

### AddRockLibLoggerSingleton (using LoggerFactory)

This extension method adds a *singleton* `ILogger` service, created with `LoggerFactory`, to the specified `IServiceCollection`.

Parameter                | Required \| Default | Type              | Description
------------------------ | ------------------- | ----------------- | -----------
rockLibLoggerName        | No \|  `"default"`  | `string`          | The name of the RockLib logger used for logging
defaultTypes             | No \| `null`        | `DefaultTypes`    | An object that defines the default types to be used when a type is not explicitly specified by a configuration section
valueConverters          | No \| `null`        | `ValueConverters` | An object that defines custom converter functions that are used to convert string configuration values to a target type
addLoggerProvider        | No \| `false`       | `bool`            | Whether to also add a singleton `ILoggerProvider` service with a `RockLibLoggerProvider` implementation to the service collection
reloadOnConfigChange     | No \| `true`        | `bool`            | Whether instances of `ILogger` created by the service collection will reload when their configuration changes

### AddRockLibLoggerSingleton (using ILogger instance)

This extension method adds a *singleton* `ILogger` service, specified directly, to the specified `IServiceCollection`.

Parameter         | Required \| Default | Type              | Description
----------------- | ------------------- | ----------------- | -----------
logger            | No \| N/A           | `ILogger`         | The instance of <see cref="ILogger"/> to add
addLoggerProvider | No \| `false`       | `bool`            | Whether to also add a singleton `ILoggerProvider` service with a `RockLibLoggerProvider` implementation to the service collection
