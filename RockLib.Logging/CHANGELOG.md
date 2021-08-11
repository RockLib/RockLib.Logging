# RockLib.Logging Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased

#### Changed

- Changes "Quicken Loans" to "Rocket Mortgage".
- Updates RockLib.Diagnostics to latest version, [1.0.9](https://github.com/RockLib/RockLib.Diagnostics/blob/main/RockLib.Diagnostics/CHANGELOG.md#109---2021-08-11).
- Updates RockLib.Reflection.Optimized to latest version, [1.3.2](https://github.com/RockLib/RockLib.Reflection.Optimized/blob/main/RockLib.Reflection.Optimized/CHANGELOG.md#132---2021-08-11).

## 3.0.7 - 2021-07-30

#### Removed

- Removed RockLib.Logging.Analyzers since Microsoft.Analysis.CSharp v3.9 is not compatible with .net3.1.

## 3.0.6 - 2021-07-22

#### Changed

- Updates RockLib.Logging.Analyzers to latest version, [1.0.2](https://github.com/RockLib/RockLib.Analyzers/blob/main/Logging/CHANGELOG.md#102---2021-07-21).

## 3.0.5 - 2021-07-13

#### Added

- Adds ability to add [SafeToLog] and [NotSafeToLog] attributes to types and properties at runtime.

## 3.0.4 - 2021-07-13

#### Fixed

- Fixes memory leak when using `LoggerLookup`.

#### Changed

- Updates RockLib.Logging.Analyzers to latest version, [1.0.1](https://github.com/RockLib/RockLib.Analyzers/blob/main/Logging/CHANGELOG.md#101---2021-07-13).

## 3.0.3 - 2021-06-09

#### Added

- Adds RockLib.Logging.Analyzers package reference.

## 3.0.2 - 2021-05-06

#### Added

- Adds SourceLink to nuget package.

#### Changed

- Updates RockLib.Diagnostics and RockLib.Reflection.Optimized packages to latest versions, which include SourceLink.

----

**Note:** Release notes in the above format are not available for earlier versions of
RockLib.Logging. What follows below are the original release notes.

----

## 3.0.1 - 2021-04-12

Fixes bug in DebugLogProvider.

## 3.0.0 - 2021-04-12

- Breaking change: Changes the value of the Logger.DefaultName constant from "default" to an empty string. Ultimately, this is done to make it easier for apps to correctly configure the default logger using Microsoft.Extensions.Options.
- Small improvements to reloading loggers when a reload occurs:
  - If a configureOptions callback was provided, call it before recreating the inner logger.
  - If the old inner logger had an ErrorHandler, copy it to the new logger.
- Adds correlation id, business process id, and business activity to default log templates.
- Adds the DebugLogProvider class.

## 2.3.1 - 2021-02-25

Adds ToString() override to the LogEntry class.

## 2.3.0 - 2021-02-19

- Adds safe logging attributes and extensions.
- Adds net5.0 target

## 2.3.0-alpha02 - 2021-02-19

Add net5.0 target.

## 2.3.0-alpha01 - 2021-01-14

Adds safe logging attributes and extensions.

## 2.2.0 - 2020-09-23

Adds RockLib.Logging.Diagnostics.LoggingTraceListener class.

## 2.1.5 - 2020-08-27

Updates RockLib.Diagnostics dependency for a bug fix.

## 2.1.4 - 2020-08-26

- The `Logger.Log` method does nothing (instead of throwing) when its `LogProcessor` is disposed.
- Fixes a race condition where `BackgroundLogProcessor.Dispose` could return before processing and tracking was finished.
- Event handlers for the static `AppDomain.CurrentDomain.ProcessExit` and `AppDomain.CurrentDomain.DomainUnload` events are no longer registered for disposing every instance of `BackgroundLogProcessor`. The domain event handlers are only registered once - to dispose the static `Logger._backgroundLogProcessor` field.

## 2.1.3 - 2020-08-14

Adds icon to project and nuget package.

## 2.1.2 - 2020-08-11

Fixes a bug when registering a default named logger and adding a console, file, or rolling file log provider that is configured with default named options. This happened because our default logger name, "default", is different than Microsoft's default options name, "" (empty string).

## 2.1.1 - 2020-07-30

Updates to align with nuget conventions.

## 2.1.0 - 2020-07-16

- Adds first-class dependency injection support.
- Logger no longer does anything on Dispose, but the method is kept for backwards compatibility.
- The default values for LogEntry.MachineName and LogEntry.UserName are cached.

## 2.0.0 - 2019-04-19

**Changes to ILogger interface**
- Add `LogProviders` property
  - The collection of log providers had been a private field of the Logger class; now it is exposed in the interface.
- Add `ErrorHandler` property
  - When set, it handles errors from log providers.
  - Can also be set using a delegate by calling the `SetErrorHandler` extension method for the `ILogger` interface.
- Add `ContextProviders` property
  - A collection of objects for customizing (e.g. adding extended properties to) outgoing logs before they are sent.
- Add `LogProcessor` property
  - Instead of the `Logger` class handling the processing of logs, either synchronously or asynchronously depending on the value of the `isSynchronous` constructor parameter, it delegates the main processing of logs to its log processor.
  - Can be set directly with an implementation of `ILogProcessor`, or by specifying the logger's processing mode - `Background`, `Synchronous`, or `FireAndForget`.
- Implement `IDisposable`
  - Previously, the `Logger` class implemented `IDisposable`, but not the `ILogger` interface.

**Changes to LoggerFactory**
- `LoggerFactory` is configuration-based instead of "configuration-based by default, otherwise able to specify collection of logger objects from which to choose from".
  - Has a `Configuration` property whose value determines what loggers the factory can produce.
  - The default configuration comes from the "RockLib.Logging" or "RockLib_Logging" section under `RockLib.Configuration.Config.Root`.
  - Can specify a custom configuration with the `SetConfiguration` method.
- Renames methods to be simpler and more explicit: `CreateFromConfig` -> `Create`; `GetInstance` -> `GetCached`
- Methods return `ILogger` instead of `Logger`
- Adds optional parameters to `LoggerFactory` methods
  - `defaultTypes`, `valueConverters`, and `resolver`
    - LoggerFactory uses the `RockLib.Configuration.ObjectFactory` library to actually create the `ILogger`.
 - These values are used by the library to customize how the logger is created.
  - `reloadOnConfigChange`
    - The `RockLib.Configuration.ObjectFactory` library has the ability to create objects from configuration such that, when the configuration changes, the objects reload themselves automatically. `LoggerFactory` can use this ability.
 - Default is `true`, meaning loggers will reload themselves when their configuration changes. Set to `false` if a config-reloading logger is not desired.

**Changes to LogEntry**
- Add new properties: correlationId, businessProcessId, and businessActivityId
  - These are also exposed as optional parameters in the logging extension methods.
- Make CallerInfo first-class property of LogEntry

**Changes to log providers**
- `FileLogProvider` synchronizes across all instances that use the same file
  - This means all instances of `FileLogProvider` that use the same file can write concurrently.
- `ConsoleLogProvider` can target either stdout or stderr

## 2.0.0-alpha11 - 2019-04-19

- Moves the extension methods from the LoggerFactory class to a separate class, making LoggerFactory's intellisense less cluttered.
- ConsoleLogProvider can write to either stdout or stderr.
- Changes logger error handling from an event to a read/write property of type IErrorHandler. This allows for specialized error handling components to be created (being an event made these kinds of components difficult/weird/impossible to create/use).

## 2.0.0-alpha10 - 2019-04-05

Adds support for RockLib_Logging config section.

## 2.0.0-alpha09 - 2019-03-26

Reorders the extendedProperties parameter in the various logger extensions.

## 2.0.0-alpha08 - 2019-03-26

Adds log processor feature. This allows instances of Logger to be cheaply created depending on the need of the application.

## 2.0.0-alpha07 - 2019-03-21

Adds assembly-level [ConfigSection] attribute.

## 2.0.0-alpha06 - 2019-03-04

Renames the `Error` event in the `Logger` class to `LogProviderError`.

## 2.0.0-alpha05 - 2019-03-01

Adds a collection of `IContextProvider` objects to the `Logger` class that modify the `LogEntry` objects passed to its `Log` method before the log providers send them.

## 2.0.0-alpha04 - 2019-02-28

- Implements new fields in LogEntry
- Add ErrorEvent in Logger
- Fix Github suggested vulnerability

## 2.0.0-alpha03 - 2019-01-04

Make ILogger disposable.

## 2.0.0-alpha02 - 2019-01-03

Adds optional parameters to LoggerFactory methods: defaultTypes, valueConverters, resolver, and reloadOnConfigChange.

## 2.0.0-alpha01 - 2018-10-22

Initial prerelease of 2.x.

## 1.2.0 - 2018-10-05

Add `IsSynchronous` property to `Logger` to allow short lived applications to not need to dispose of a logger to guarantee log delivery.

## 1.1.0 - 2018-08-03

- Adds `LoggerFactory.CreateFromConfig` method, allowing non-cached instances of `Logger` to be created from `LoggerFactory`.
- Try really, really hard to cleanly shut down all instances of `Logger`, even if the user does not dispose them.

## 1.0.1 - 2018-06-29

Adds the ability to set extended properties on a log entry with any dictionary with a string key type. Also adds support for non-generic IDictionary - all items with keys of type string are added.

## 1.0.0 - 2018-06-19

Initial release.

## 1.0.0-alpha08 - 2018-05-24

Small improvements:
- Add parameter validation and corresponding sad-path tests throughout the library.
- LoggerFactory: Publicly expose the method, `CreateDefaultLoggers`, that is used to create the default value for the `Loggers` property.
- LogEntry: Add default constructor and rearrange some parameters from the other constructors to make the class easier and more intuitive to instantiate.
