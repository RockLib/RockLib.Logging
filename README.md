# Rock.Logging

## Table of Contents
* [Nuget](#nuget)
  * Install via [Nuget UI](#install-nuget-via-ui)
  * Install via [Command Line](#install-nuget-via-command-limne)
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
Rock.Logging is available via [NuGet](https://www.nuget.org/packages/Rock.Logging/)

### Install Nuget via UI
If you want to install this package via the NuGet UI, this can be done as well.  Make sure to switch to the QuGet package source.

If you are unsure how to use the UI to reference the package source checkout out these [docs](https://docs.microsoft.com/en-us/nuget/tools/package-manager-ui#package-sources).

### Install Nuget via Command Line

Rock.Logging is available via [NuGet](https://www.nuget.org/packages/Rock.Logging/)

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
  * Provider to write all logged messages to the active console
* Email Log Provider
  * Provider to send logging events to an email address.
* File Log Provider
  * Provider to output the log based on the Formatter to a flat file
* Rolling File Log Provider
  * Provider to output to a flat file, like the File Log Provider, but this provider will roll over files based on size/time.
* Formattable Log Provider
  *
* Http Endpoint Log Provider 
  * Provider to post the logged messages to an HTTP endpoint of your choosing.



