# Using LoggingTraceListener to log trace messages

The `RockLib.Logging.Diagnostics.LoggingTraceListener` is an inheritor of `System.Diagnostics.TraceListener` that sends trace messages to a `RockLib.Logging.ILogger`. It is for troubleshooting *other* libraries that output trace messages - such trace messages will be logged with the `Ilogger` that the application chooses.

> **WARNING:** DO NOT use `LoggingTraceListener` for troubleshooting issues with RockLib.Logging itself!

## RockLib.Diagnostics Configuration

The easiest way to add a `LoggingTraceListener` is with configuration. The following example assumes that the library we're troubleshooting uses a trace source named "my_library_trace_source_name".

```json
{
    "Rocklib.Logging": {
        "Level": "Debug",
        "Providers": { "Type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging" }
    },

    "RockLib.Diagnostics": {
        "Sources": {
            "Name": "my_library_trace_source_name",
            "Switch": {
                "Name": "my_library_trace_source_name",
                "Level": "All"
            },
            "Listeners": { "Type": "RockLib.Logging.Diagnostics.LoggingTraceListener, RockLib.Logging" }
        }
    }
}
```

If the application defines more than one logger, the `LoggingTraceListener` needs to specify the logger name.

```json
{
    "Rocklib.Logging": [
        {
            "Name": "MainLogger",
            "Level": "Warn",
            "Providers": { "Type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging" }
        },
        {
            "Name": "DiagnosticLogger",
            "Level": "Debug",
            "Providers": { "Type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging" }
        }
    ]

    "RockLib.Diagnostics": {
        "Sources": {
            "Name": "my_library_trace_source_name",
            "Switch": {
                "Name": "my_library_trace_source_name",
                "Level": "All"
            },
            "Listeners": {
                "Type": "RockLib.Logging.Diagnostics.LoggingTraceListener, RockLib.Logging",
                "Value": {
                    "LoggerName": "DiagnosticLogger"
                }
            }
        }
    }
}
```
