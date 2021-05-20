# RockLib.Logging

*A simple logging library.*

### RockLib.Logging [![Build status](https://ci.appveyor.com/api/projects/status/y06g87sp3p3q5gb4?svg=true)](https://ci.appveyor.com/project/RockLib/rocklib-logging) [![NuGet](https://img.shields.io/nuget/vpre/RockLib.Logging.svg)](https://www.nuget.org/packages/RockLib.Logging)

The main library.

### RockLib.Logging.AspNetCore [![Build status](https://ci.appveyor.com/api/projects/status/olwpmvt6lw5au265?svg=true)](https://ci.appveyor.com/project/RockLib/rocklib-logging-8gd5t) [![NuGet](https://img.shields.io/nuget/vpre/RockLib.Logging.AspNetCore.svg)](https://www.nuget.org/packages/RockLib.Logging.AspNetCore)

RockLib.Logging for AspNetCore. Includes context providers and a logging action filter.

### RockLib.Logging.Microsoft.Extensions [![Build status](https://ci.appveyor.com/api/projects/status/bp0e4mk8escvm2wl?svg=true)](https://ci.appveyor.com/project/RockLib/rocklib-logging-microsoft-extensions) [![NuGet](https://img.shields.io/nuget/vpre/RockLib.Logging.Microsoft.Extensions.svg)](https://www.nuget.org/packages/RockLib.Logging.Microsoft.Extensions)

An implementation of Microsoft.Extensions.Logging that uses RockLib.Logging.

### RockLib.Logging.Moq [![Build status](https://ci.appveyor.com/api/projects/status/67qvujjho7wncp7u?svg=true)](https://ci.appveyor.com/project/RockLib/rocklib-logging-59g8r) [![NuGet](https://img.shields.io/nuget/vpre/RockLib.Logging.Moq.svg)](https://www.nuget.org/packages/RockLib.Logging.Moq)

Extensions for verifying logging operations with Moq.

---

- [Getting started](docs/GettingStarted.md)
- How to:
  - [Perform basic logging operations](docs/Logging.md)
  - [Instantiate and configure a logger](docs/Logger.md)
  - [Add log extended properties "safely" (i.e. automatically remove PII)](docs/SafeLogging.md)
  - [Configure and use LoggerFactory](docs/LoggerFactory.md)
  - [Add RockLib logging to the Microsoft dependency injection system](docs/DI.md)
  - [Add RockLib logging to the Microsoft logging system](docs/Microsoft.md)
  - [Use log providers](docs/LogProviders.md)
    - [ConsoleLogProvider](docs/ConsoleLogProvider.md)
    - [FileLogProvider](docs/FileLogProvider.md)
    - [RollingFileLogProvider](docs/RollingFileLogProvider.md)
  - [Handle log provider errors](docs/LogProviderErrors.md)
  - [Use context providers](docs/ContextProviders.md)
  - [Use log processors / processing mode](docs/LogProcessors.md)
  - [Format logs](docs/Formatting.md)
  - [Enable tracing for troubleshooting](docs/Tracing.md)
  - [Use LoggingTraceListener to log trace messages](docs/LoggingTraceListener.md)
  - [Automatically capture HttpContext information in AspNetCore apps](docs/AspNetCore.md#context-providers)
  - [Automatically log AspNetCore controller actions](docs/AspNetCore.md#logging-action-filters)
  - [Automatically log 404 http responses](docs/AspNetCore.md#route-not-found-middleware)
  - [Change a logger's settings "on the fly" (in a running application)](docs/Reloading.md) 🆕
- API Reference:
  - [RockLib.Logging](https://www.fuget.org/packages/RockLib.Logging)
  - [RockLib.Logging.AspNetCore](https://www.fuget.org/packages/RockLib.Logging.AspNetCore)
  - [RockLib.Logging.Microsoft.Extensions](https://www.fuget.org/packages/RockLib.Logging.Microsoft.Extensions)
  - [RockLib.Logging.Moq](https://www.fuget.org/packages/RockLib.Logging.Moq)
