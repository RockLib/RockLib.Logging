# RockLib.Logging.AspNetCore package

This package contains context providers and action filters for use by AspNetCore applications.

### Context providers

To automatically capture information from the current HttpContext for each log sent, call the `AddHttpContextProvider` extension method when adding the logger to a service collection.

*Note: A logger must have `Singleton` lifetime (which is the default) in order for any of the context providers mentioned in this document to function properly.*

```c#
using RockLib.Logging.AspNetCore;
using RockLib.Logging.DependencyInjection;

public void ConfigureServices(IServiceCollection services)
{
    services.AddLogger()
        .AddConsoleLogProvider()
        .AddHttpContextProvider();
}
```

Logs sent by this logger will include the following extended properties:

| Extended property | Description                                                  |
|:------------------|:-------------------------------------------------------------|
| Method            | The request method (GET, POST, etc.)                         |
| Path              | The route pattern (if available), or request path (if not)   |
| UserAgent         | The 'User-Agent' header of the request                       |
| Referer           | The 'Referer' header of the request                          |
| RemoteIpAddress   | The remote IP address of the request's underlying connection |
| X-Forwarded-For   | The 'X-Forwarded-For' header of the request                  |

Logs sent by this logger will also have their `CorrelationId` property set using the `ICorrelationIdAccessor` associated with the HttpContext via the RockLib.DistributedTracing.AspNetCore package's `GetCorrelationIdAccessor` extension method.

---

To customize the name of the correlation id header, configure its options:

```c#
services.Configure<CorrelationIdContextProviderOptions>(options =>
    options.CorrelationIdHeader = "MyCorrelationIdHeader");
```

---

There are also individual context providers available (configuring the correlation id header is the same as above):

```c#
services.AddLogger()
    .AddConsoleLogProvider()
    .AddContextProvider<RequestMethodContextProvider>()
    .AddContextProvider<PathContextProvider>()
    .AddContextProvider<UserAgentContextProvider>()
    .AddContextProvider<ReferrerContextProvider>()
    .AddContextProvider<RemoteIpAddressContextProvider>()
    .AddContextProvider<ForwardedForContextProvider>()
    .AddContextProvider<CorrelationIdContextProvider>();
```

### Logging action filters

To automatically record an info log for a controller action, decorate it with the [InfoLog] attribute. Doing so will result in an info log with a message in the format "Request handled by {actionName}.". The log will also include, when applicable: the exception thrown by the action, and the "ResponseStatusCode", "ResultType", and "ResultObject" extended properties.

```c#
// GET math/triple/4
[InfoLog]
[HttpGet("triple/{num}")]
public int Triple(int num)
{
    return num * 3;
}
```

Using the route "Math/Triple/4" will result in a log entry with the following values:

| LogEntry property                     | Value                                                                                     |
|:--------------------------------------|:------------------------------------------------------------------------------------------|
| Message                               | "Request handled by MyAspNetCoreApp.Controllers.MathController.Triple (MyAspNetCoreApp)." |
| ExtendedProperties.num                | 4                                                                                         |
| ExtendedProperties.ResponseStatusCode | 200                                                                                       |
| ExtendedProperties.ResultType         | "ObjectResult"                                                                            |
| ExtendedProperties.ResultObject       | 12                                                                                        |

---

Instead of decorating the action method, a controller can be decorated with [InfoLog]. In this case, all action methods in the controller will be logged.

```c#
namespace MyAspNetCoreApp.Controllers
{
    [InfoLog]
    [ApiController]
    [Route("[controller]")]
    public class MathController : ControllerBase
    {
    }
}
```

The [InfoLog] action filter can also be registered globally, in `Startup.ConfigureServices` (or wherever you configure your services), as an option in the call to the `AddControllers` extension method:

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers(options => options.Filters.Add(typeof(InfoLogAttribute)));
}
```

##### Optional parameters

The [InfoLog] action filter has two optional parameters.

| Parameter name | Description                                                                           |
|:---------------|:--------------------------------------------------------------------------------------|
| messageFormat  | A format string for the log message. The action name is used as the `{0}` placeholder|
| loggerName     | The name of the logger that is registered with the service collection.                |
