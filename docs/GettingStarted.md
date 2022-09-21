---
position: 5
---

# Getting Started

In this tutorial, we will be building a console application with a hosted service that writes logs when started and stopped (the service doesn't actually do anything).

---

Create a .NET Core console application named "LoggingTutorial".

---

Add nuget references for "RockLib.Logging", "Microsoft.Extensions.DependencyInjection" and "Microsoft.Extensions.Hosting" to the project.

---

## Add a new class named 'ExampleService' to the project:

```csharp
using Microsoft.Extensions.Hosting;
using RockLib.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LoggingTutorial
{
    public class ExampleService : IHostedService
    {
        private readonly ILogger _logger;

        public ExampleService(ILogger logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Starting service...");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Stopping service...");
            return Task.CompletedTask;
        }
    }
}
```

---

## Edit the _Program.cs_ file as follows:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RockLib.Logging.DependencyInjection;
using System.Threading.Tasks;

namespace LoggingTutorial
{
    class Program
    {
        static Task Main(string[] args)
        {
            return CreateHostBuilder(args)
                .RunConsoleAsync(options => options.SuppressStatusMessages = true);
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureServices(services =>
            {
                services.AddLogger()
                    .AddConsoleLogProvider(options => options.SetTemplate("[{createTime(O)}] {level} Log: {message}"));

                services.AddHostedService<ExampleService>();
            });
        }
    }
}
```

---

## Start the app

It should output something similar to the following to console:

```
[2020-07-17T15:33:58.9262051Z] Info Log: Starting service...
```

---

## Stop the app

After hitting `<ctrl> + c`, it should output this, then exit:

```
[2020-07-17T15:35:38.5859970Z] Info Log: Stopping service...
```
