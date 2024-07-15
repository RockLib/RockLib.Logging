# RockLib.Logging

*A simple logging library.*

## Packages

### RockLib.Logging
The main library.

### RockLib.Logging.AspNetCore
RockLib.Logging for AspNetCore. Includes context providers and a logging action filter.

### RockLib.Logging.Microsoft.Extensions
An implementation of Microsoft.Extensions.Logging that uses RockLib.Logging.

### RockLib.Logging.Moq
Extensions for verifying logging operations with Moq.

---

- [Getting started](docs/GettingStarted.md)
- How to:
  - [Perform basic logging operations](docs/LoggingStart.md)
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
  - [Use log level resolver](docs/LogLevelResolver.md)
  - [Format logs](docs/Formatting.md)
  - [Enable tracing for troubleshooting](docs/Tracing.md)
  - [Use LoggingTraceListener to log trace messages](docs/LoggingTraceListener.md)
  - [Automatically capture HttpContext information in AspNetCore apps](docs/AspNetCore.md#context-providers)
  - [Automatically log AspNetCore controller actions](docs/AspNetCore.md#logging-action-filters)
  - [Automatically log 404 http responses](docs/AspNetCore.md#route-not-found-middleware)
  - [Change a logger's settings "on the fly" (in a running application)](docs/Reloading.md)
  - [Test logging in an application using RockLib.Logging.Moq](docs/Moq.md)
- API Reference:
  - [RockLib.Logging](https://www.fuget.org/packages/RockLib.Logging)
  - [RockLib.Logging.AspNetCore](https://www.fuget.org/packages/RockLib.Logging.AspNetCore)
  - [RockLib.Logging.Microsoft.Extensions](https://www.fuget.org/packages/RockLib.Logging.Microsoft.Extensions)
  - [RockLib.Logging.Moq](https://www.fuget.org/packages/RockLib.Logging.Moq)

