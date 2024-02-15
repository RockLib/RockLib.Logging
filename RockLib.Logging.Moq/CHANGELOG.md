# RockLib.Logging.Moq Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 3.0.0-alpha.1 - 2024-02-15

#### Changed
- Finalized 3.0.0 release.
- 
## 3.0.0-alpha.1 - 2024-01-31

#### Changed
- Supported targets: net6.0, net8.0, and net48 - netcoreapp3.1 was removed.
- Updated package references.

## 2.0.1 - 2023-02-22

#### Changed
- Updated RockLib.Logging package reference to `4.0.2`

## 2.0.0 - 2022-11-15
	
#### Added
- Added `.editorconfig` and `Directory.Build.props` files to ensure consistency.

#### Changed
- Supported targets: net6.0, netcoreapp3.1, and net48.
- As the package now uses nullable reference types, some method parameters now specify if they can accept nullable values.

#### Fixed

- Fixes parameter name and doc comments in VerifyLog extension methods.

## 2.0.0-alpha.1 - 2022-11-10
	
#### Added
- Added `.editorconfig` and `Directory.Build.props` files to ensure consistency.

#### Changed
- Supported targets: net6.0, netcoreapp3.1, and net48.
- As the package now uses nullable reference types, some method parameters now specify if they can accept nullable values.

#### Fixed

- Fixes parameter name and doc comments in VerifyLog extension methods.

## 1.2.5 - 2021-08-11

#### Changed

- Changes "Quicken Loans" to "Rocket Mortgage".
- Updates RockLib.Logging to latest version, [3.0.8](https://github.com/RockLib/RockLib.Logging/blob/main/RockLib.Logging/CHANGELOG.md#308---2021-08-11).

## 1.2.4 - 2021-05-06

#### Added

- Adds SourceLink to nuget package.

#### Changed

- Updates RockLib.Logging package to latest version, which includes SourceLink.

----

**Note:** Release notes in the above format are not available for earlier versions of
RockLib.Logging.Moq. What follows below are the original release notes.

----

## 1.2.2 - 2021-02-25

- Improves the error message when logging verification fails.
- Use string equality comparison by default. Match as regex only if the value starts and ends with '/'. For example "/(?i)hello,\s+world[.!?]/".

## 1.2.1 - 2021-02-19

Adds net5.0 target.

## 1.2.0 - 2021-01-08

Adds overloads of the Verify extension methods to verify that a logger logged with a certain exception.

## 1.1.1 - 2020-08-26

Updates RockLib.Logging package to the latest version, 2.1.4.

## 1.1.0 - 2020-08-21

Extended property verification knows what to do with a value of type `object[]`.

## 1.0.2 - 2020-08-14

Adds icon to project and nuget package.

## 1.0.1 - 2020-08-14

Adds icon to project and nuget package.

## 1.0.0 - 2020-08-05

Initial release.
