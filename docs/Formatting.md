---
sidebar_position: 16
---

# Formatting Logs

By implementing the `RockLib.Logging.ILogFormatter` interface, defining the `Format` method, and providing it to a log provider (such as a `FileLogProvider`, a `ConsoleLogProvider` or a `RollingFileLogProvider`), you are able to transform a `LogEntry` object into a `string` emitted by the log provider.

As an example, the following implementation of `ILogFormatter` will output `LogEntry.Message` followed by the Log `LogEntry.CreateTime`, with each separated by a hyphen.

```csharp
public class SimpleMessageFormatter : ILogFormatter
{
	public string Format(LogEntry logEntry)
	{
		// output Message - CreateTime (in ISO8601 format)
		return $"{logEntry.Message} - {logEntry.CreateTime.ToString("o")}";
	}
}
```

This would result in the following message being output by the consuming log provider when it is prompted to log:
```
SAMPLE LOG MESSAGE - 2025-06-10T16:20:57.7876207Z
```

Log formatters are passed to the Log provider via the constructor. For example, the following code will provide a new instance of the above message formatter to a new `FileLogProvider`:

```csharp
var fileLogProvider = new FileLogProvider("log.txt", new SimpleMessageFormatter());
```
