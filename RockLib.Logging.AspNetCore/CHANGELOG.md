# RockLib.Logging.AspNetCore Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 3.2.0

#### Added

- Adds RouteNotFoundMiddleware for automatically logging when a 404 occurs.
  - Includes extension method to easily register the RouteNotFoundMiddleware to the request pipeline.

## 3.1.3

#### Added

- Adds SourceLink to nuget package.

#### Changed

- Updates RockLib.Logging and RockLib.DistributedTracing.AspNetCore packages to latest versions, which include SourceLink.

----

**Note:** Release notes in the above format are not available for earlier versions of
RockLib.Logging.AspNetCore. What follows below are the original release notes.

----

## 3.1.1

Updates RockLib.Logging package to latest version (3.0.1).

## 3.1.0

LoggingActionFilter elevates the log level when an action has an unhandled exception.

## 3.0.1

- Fixes bug in LoggingActionFilter - ActionArguments were not sanitized.
- Updates RockLib.Logging dependency to latest version.

## 3.0.0

- Adds net5.0 target
- Removes all deprecated functionality.
- Adds several IContextProvider implementations for use in AspNetCore projects.
- Adds an action filter that automatically records info logs for controller actions.

## 3.0.0-alpha01

- Removes all deprecated functionality.
- Adds several IContextProvider implementations for use in AspNetCore projects.
- Adds an action filter that automatically records info logs for controller actions.

## 2.0.5

Updates to align with nuget conventions.

## 2.0.4

Obsoletes everything.

## 2.0.2

No functional changes. Uses RockLib.Configuration.AspNetCore package to set Config.Root instead of doing it manually.

## 2.0.1

Use 2.0.0 versions of Microsoft.AspNetCore packages.

## 2.0.0

- Updates `RockLib.Logging` library to version 2.0.0.
- Renames/flips the `bypassAspNetCoreLogging` parameter to `registerAspNetCoreLogger` in both `UseRockLibLogging` extension methods. The default value is `false`.
- Improvements to the `UseRockLibLogging` extension method that creates an `ILogger` from `LoggerFactory`:
  - The `ILogger` is registered as transient so that implementations of `IContextProvider` are created fresh for each instance of Logger. This will allow an implementation to be created that uses the current `HttpContext` (via `IHttpContextAccessor`) to add context to `LogEntry` objects.
  - Registers a `BackgroundLogProcessor` as a singleton `ILogProcessor` so that instances of `Logger` can share the same log processor.
  - Since the lifecycle of the `Logger` is short, don't create it as a reloading proxy.

## 2.0.0-alpha03

Updates RockLib.Logging version to support RockLib_Logging.

## 2.0.0-alpha02

- Removes unnecessary parameters from UseRockLibLogging extension methods.
- Improvements to the UseRockLibLogging extension method that creates an ILogger fro LoggerFactory.
  - The ILogger is registered as transient so that implementations of IContextProvider are created fresh for each instance of Logger. This will allow an implementation to be created that uses the current HttpContext to add context to LogEntry objects.
  - Registers a BackgroundLogProcessor as a singleton ILogProcessor so that instances of Logger can share the same log processor.
  - Since the lifecycle of the Logger is short, don't create it as a reloading proxy.
- Updates packages to latest versions.

## 2.0.0-alpha01

- Updates RockLib.Logging to version 2.0.0-alpha03.
- Exposes new optional parameters from ConfigurationObjectFactory.

## 1.0.1

Adds thread-safety to scoped logging operations and prevents the disposable scope objects from throwing when disposed multiple times.

## 1.0.0

Initial release.

## 1.0.0-alpha04

Added `bypassAspNetCoreLogging` flag to `UseRockLibLogging` extension methods.

## 1.0.0-alpha03

Improvements to API:
- Rename `UseRockLib` to `UseRockLibLogging`.
- Add `setConfigRoot` parameter to `UseRockLibLogging` that bypasses the side-effect of setting `Config.Root`. The default value is `true` - passing a value of `false` is appropriate when the application has programmatically set the `LoggerFactory.Loggers` property.
- Add overload for `UseRockLibLogging` that takes an `ILogger` parameter. This overload does not have the side-effect of setting `Config.Root`.
- Use lower versions of Microsoft.AspNetCore.Hosting.Abstractions and Microsoft.Extensions.Logging dependencies.

## 1.0.0-alpha02

- Implement `BeginScope` method.
- Improve DI wiring in `UseRockLib` extension method.
  - Ensure that Config.SetRoot is called if possible before calling LoggerFactory.GetInstance.
  - Register RockLib.Logging.ILogger for DI.
- Reduce footprint of public API.

## 1.0.0-alpha01

Initial prerelease.
