# Enable Tracing For Troubleshooting

It is possible for an implementation of `ILogProvider` to throw an exception in its `WriteAsync` method (in fact implementations should throw any exception instead of catching and handling it). If an exception is thrown, it will caught by the log processor, passed to the logger's `ErrorHandler`, and if a `TraceSource` exists with the name "rocklib.logging", a trace event is sent to it.

In addition, if the `TraceSource` is defined and is tracing is at `Information`, a trace event is sent for each successful log processing.

## To set a _TraceSource_, add this to your configuration:

```json
"RockLib.Diagnostics": {
    "Sources": {
        "Name": "rocklib.logging",
        "Switch": {
            "Name": "rocklib.logging",
            "Level": "All"
        },
        "Listeners": {
            "Name": "rocklib.logging",
            "LogFileName": "C:\\my\\path\\rocklib_logging.log"
        }
    }
}
```

## To set the _TraceSource_ programmatically:

```csharp
Tracing.Settings = new DiagnosticsSettings(
    sources: new System.Diagnostics.TraceSource[]
    {
        new System.Diagnostics.TraceSource(name: "rocklib.logging")
        {
            Switch = new System.Diagnostics.SourceSwitch(name: "rocklib.logging")
            {
                Level = System.Diagnostics.SourceLevels.All
            },
            Listeners =
            {
                new System.Diagnostics.DefaultTraceListener
                {
                    Name = "rocklib.logging",
                    LogFileName = "C:\\my\\path\\rocklib_logging.log"
                }
            }
        }
    });
```
