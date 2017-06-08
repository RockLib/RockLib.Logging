#Rock.Logging

## Table of Contents
* [Nuget](#nuget)
* [Configuration](#configuration)
  * [Configuration Transformations](#configuration-transformations)
* Configuration Examples
  * [Basic Configuration](docs/BasicConfig.md)
  * [Advanced Configuration](docs/AdvancedConfig.md)
    * [Custom Formatter Configuration](docs/AdvancedConfig.md#Custom-sFormatter-Configuration)
    * [IoC Configuration](docs/AdvancedConfig.md#IoC-Configuration)
* [Common Exceptions](#common-exceptions)
* [Trouble Shooting Steps](#trouble-shooting-steps)

# Nuget
Rock.Logging is available via NuGet

How to install from the package manager console:

```
PM> Install-Package Rock.Logging
```


## Logging Levels

* Debug 
* Info
* Warn
* Error
* Fatal
* Audit

## Logging Adapters
* Console Log Provider
* Email Log Provider
* File Log Provider
* Formattable Log Provider
* Http Endpoint Log Provider
* Rolling File Log Provider



