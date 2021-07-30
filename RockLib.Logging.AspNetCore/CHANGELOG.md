# RockLib.Logging.AspNetCore Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 3.2.3 - 2021-07-30

#### Changed

-  Updates [RockLib.Logging](https://github.com/RockLib/RockLib.Logging/blob/main/RockLib.Logging/CHANGELOG.md#307---2021-07-30).

## 3.2.2 - 2021-07-27

#### Changed

- Updates [RockLib.Logging](https://github.com/RockLib/RockLib.Logging/blob/main/RockLib.Logging/CHANGELOG.md#306---2021-07-22) and [RockLib.Logging.AspNetCore.Analyzers](https://github.com/RockLib/RockLib.Analyzers/blob/main/Logging.AspNetCore/CHANGELOG.md#101---2021-07-22) packages to latest versions.

## 3.2.1 - 2021-07-13

#### Changed

- Updates RockLib.Logging to latest version, [3.0.4](https://github.com/RockLib/RockLib.Logging/blob/main/RockLib.Logging/CHANGELOG.md#304---2021-07-13).

#### Added

- Adds [RockLib.Logging.AspNetCore.Analyzers](https://github.com/RockLib/RockLib.Analyzers/blob/main/Logging.AspNetCore/CHANGELOG.md#100---2021-07-13) package reference.

## 3.2.0 - 2021-05-20

#### Added

- Adds RouteNotFoundMiddleware for automatically logging when a 404 occurs.
  - Includes extension method to easily register the RouteNotFoundMiddleware to the request pipeline.

## 3.1.3 - 2021-05-16

#### Added

- Adds SourceLink to nuget package.

#### Changed

- Updates RockLib.Logging and RockLib.DistributedTracing.AspNetCore packages to latest versions, which include SourceLink.

----

**Note:** Release notes in the above format are not available for earlier versions of
RockLib.Logging.AspNetCore. What follows below are the original release notes.

----

## 3.1.1 - 2021-04-12

Updates RockLib.Logging package to latest version (3.0.1).

## 3.1.0 - 2021-04-12

LoggingActionFilter elevates the log level when an action has an unhandled exception.

## 3.0.1 - 2021-03-03

- Fixes bug in LoggingActionFilter - ActionArguments were not sanitized.
- Updates RockLib.Logging dependency to latest version.

## 3.0.0 - 2021-02-19

- Adds net5.0 target
- Removes all deprecated functionality.
- Adds several IContextProvider implementations for use in AspNetCore projects.
- Adds an action filter that automatically records info logs for controller actions.

## 3.0.0-alpha01 - 2021-01-15

- Removes all deprecated functionality.
- Adds several IContextProvider implementations for use in AspNetCore projects.
- Adds an action filter that automatically records info logs for controller actions.

## 2.0.5 - 2020-07-30

Updates to align with nuget conventions.

## 2.0.4 - 2020-07-16

Obsoletes everything.

## 2.0.2 - 2019-05-09

No functional changes. Uses RockLib.Configuration.AspNetCore package to set Config.Root instead of doing it manually.

## 2.0.1 - 2019-05-07

Use 2.0.0 versions of Microsoft.AspNetCore packages.

## 2.0.0 - 2019-04-25

- Updates `RockLib.Logging` library to version 2.0.0.
- Renames/flips the `bypassAspNetCoreLogging` parameter to `registerAspNetCoreLogger` in both `UseRockLibLogging` extension methods. The default value is `false`.
- Improvements to the `UseRockLibLogging` extension method that creates an `ILogger` from `LoggerFactory`:
  - The `ILogger` is registered as transient so that implementations of `IContextProvider` are created fresh for each instance of Logger. This will allow an implementation to be created that uses the current `HttpContext` (via `IHttpContextAccessor`) to add context to `LogEntry` objects.
  - Registers a `BackgroundLogProcessor` as a singleton `ILogProcessor` so that instances of `Logger` can share the same log processor.
  - Since the lifecycle of the `Logger` is short, don't create it as a reloading proxy.

## 2.0.0-alpha03 - 2019-04-05

Updates RockLib.Logging version to support RockLib_Logging.

## 2.0.0-alpha02 - 2019-03-27

- Removes unnecessary parameters from UseRockLibLogging extension methods.
- Improvements to the UseRockLibLogging extension method that creates an ILogger fro LoggerFactory.
  - The ILogger is registered as transient so that implementations of IContextProvider are created fresh for each instance of Logger. This will allow an implementation to be created that uses the current HttpContext to add context to LogEntry objects.
  - Registers a BackgroundLogProcessor as a singleton ILogProcessor so that instances of Logger can share the same log processor.
  - Since the lifecycle of the Logger is short, don't create it as a reloading proxy.
- Updates packages to latest versions.

## 2.0.0-alpha01 - 2019-01-08

- Updates RockLib.Logging to version 2.0.0-alpha03.
- Exposes new optional parameters from ConfigurationObjectFactory.

## 1.0.1 - 2018-07-31

Adds thread-safety to scoped logging operations and prevents the disposable scope objects from throwing when disposed multiple times.

## 1.0.0 - 2018-06-09

Initial release.

## 1.0.0-alpha04 - 2018-06-08

Added `bypassAspNetCoreLogging` flag to `UseRockLibLogging` extension methods.

## 1.0.0-alpha03 - 2018-06-06

Improvements to API:
- Rename `UseRockLib` to `UseRockLibLogging`.
- Add `setConfigRoot` parameter to `UseRockLibLogging` that bypasses the side-effect of setting `Config.Root`. The default value is `true` - passing a value of `false` is appropriate when the application has programmatically set the `LoggerFactory.Loggers` property.
- Add overload for `UseRockLibLogging` that takes an `ILogger` parameter. This overload does not have the side-effect of setting `Config.Root`.
- Use lower versions of Microsoft.AspNetCore.Hosting.Abstractions and Microsoft.Extensions.Logging dependencies.

## 1.0.0-alpha02 - 2018-05-30

- Implement `BeginScope` method.
- Improve DI wiring in `UseRockLib` extension method.
  - Ensure that Config.SetRoot is called if possible before calling LoggerFactory.GetInstance.
  - Register RockLib.Logging.ILogger for DI.
- Reduce footprint of public API.

## 1.0.0-alpha01 - 2018-05-24

Initial prerelease.
