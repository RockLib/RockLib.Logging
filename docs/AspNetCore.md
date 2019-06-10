# How to Enable Logging for ASP.NET Core Applications

A configured logger instance can be used to automatically log requests made to a ASP.NET Core application.

In the following example, calling the `UseRockLibLogging` extension method with no parameters on the application's `WebHost` will cause all requests made to the web application to be logged at `Info` level to the unnamed, default logger:

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseRockLibLogging()
            .Build();

The `UseRockLibLogging` method adds an instance (specified or unspecified) of `RockLib.Logging.Logger` and an instance of `Microsoft.Extensions.Logging.ILoggerProvider` that uses that logger to the specified `Microsoft.Extensions.DependencyInjection.IServiceCollection` as singletons.

## Log Entry

Using this, the following properties are logged per request:

Property            | Description
------------------- | -----------
Message             | The HTTP method and URI path of the request
Create Time         | The time stamp of the received request
Level               | The logging level of the entry
Log ID              | The log entry's GUID
User Name           | The username under which the request was received
Machine Name        | The name of the machine receiving the request
Machine IP Address  | The local IP address of the machine receiving the request

## UseRockLibLogging method

There are two versions of the `UseRockLibLogging` method outlined below.

---

This version does not require any arguments and will use the default or unnamed logger, unless otherwise specified:

Parameter                | Required | Type              | Description | Default
------------------------ | -------- | ----------------- | ----------- | -------
rockLibLoggerName        | No       | `string`          | The name of the RockLib logger used for logging | `"default"`
defaultTypes             | No       | `DefaultTypes`    | An object that defines the default types to be used when a type is not explicitly specified by a configuration section  | `null`
valueConverters          | No       | `ValueConverters` | An object that defines custom converter functions that are used to convert string configuration values to a target type | `null`
setRootConfig            | No       | `bool`            | Whether to call the `RockLib.Configuration.Config.SetRoot` method when this method is called | `true`
registerAspNetCoreLogger | No       | `bool`            | Whether to register a RockLib `Microsoft.Extensions.Logging.ILoggerProvider` with the DI system | `false`

This version requires that a `RockLib.Logging.Logger` is provided as an argument:

Parameter                | Required | Type      | Description | Default
------------------------ | -------- | --------- | ----------- | -------
logger                   | Yes      | `ILogger` | The RockLib logger used for logging | N/A
registerAspNetCoreLogger | No       | `bool`    | Whether to register a RockLib `Microsoft.Extensions.Logging.ILoggerProvider` with the DI system | `false`
