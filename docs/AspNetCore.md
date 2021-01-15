# RockLib.Logging.AspNetCore package

This package contains context providers and action filters for use by AspNetCore applications.

<!--### Context providers

talk about HttpContextProvider, which contains all the other ones
talk about the other ones-->

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
