---
sidebar_position: 15
---

# Log Level Resolvers

The `ILogLevelResolver` interface defines a custom method in which any given `ILogger` can use to retrieve it's `LogLevel` on-demand.

> **Note:** `ILogLevelResolver` does not affect the logging level for other default loggers (such as `Microsoft.Extensions.Logging.ILogger`). It will only affect logs routed through `RockLib.Logging.ILogger`.

## Creating Your Own Resolver

New log level resolvers can be created by implementing the `ILogLevelResolver` interface, and registering the implementation to the DI container. If no DI container is used, you may pass `ILogLevelResolver` directly into the constructor of a `Logger`.

Only one `ILogLevelResolver` may be used per application when DI is used.

### Implementation

The interface `ILogLevelResolver` contains one method - `GetLogLevel()`. This method can return a `LogLevel`, or `null`. When a `LogLevel` is returned, that `LogLevel` will be used for all messages. When `null` is returned, the `ILogger` will fall back to it's primary method of obtaining the logging level (e.g. via an `appsettings.json` configuration).

## Use Cases

This interface can be useful if you would like your application to be able to switch logging levels at runtime. When adding a custom `ILogLevelResolver`, the implementation itself can be setup to pull log levels from wherever you would like.
