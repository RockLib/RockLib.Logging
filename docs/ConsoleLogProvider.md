# How to use and configure `ConsoleLogProvider`

The `ConsoleLogProvider` can be instantiated one of two ways.

With a template:
- template
  - Type: string
  - Description: The template used to format log entries. See [TemplateLogFormatter](Formatting.md#template) for more information.
- level
  - Type: LogLevel enum (NotSet, Debug, Info, Warn, Error, Fatal, Audit)
  - Description: The level of the log provider. If `NotSet`, the level of the containing `Logger` is used.
- output
  - Type: Output enum (StdOut, StdErr)
  - Description: The type of output stream to use.
- timeout
  - Type: Nullable<TimeSpan>
  - Description: The timeout of the log provider.

Or with a formatter:
- formatter
  - Type: ILogFormatter
  - Description: An object that formats log entries prior to writing to standard out. See [ILogFormatter](Formatting.md#ilogformatter) for more information.
- level
  - Type: LogLevel enum (NotSet, Debug, Info, Warn, Error, Fatal, Audit)
  - Description: The level of the log provider. If `NotSet`, the level of the containing `Logger` is used.
- output
  - Type: Output enum (StdOut, StdErr)
  - Description: The type of output stream to use.
- timeout
  - Type: Nullable<TimeSpan>
  - Description: The timeout of the log provider.

## Configuration for `ConsoleLogProvider`

With the default template:

```json
{
  "Rocklib.Logging": {
    "Level": "Warn",
    "Providers": [
      {
        "Type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging"
      }
    ]
  }
}
```

With a custom template:

```json
{
  "Rocklib.Logging": {
    "Level": "Warn",
    "Providers": [
      {
        "Type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging",
        "Value": {
         "Template": "{level}: {message}"
        }
      }
    ]
  }
}
```

Or with a custom `ILogFormatter` (Assumes the class `MyAssembly.MyCustomFormatter` exists in the `MyAssembly` assembly):

```json
{
  "Rocklib.Logging": {
    "Level": "Warn",
    "Providers": [
      {
        "Type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging",
        "Value": {
         "Formatter": {
             "Type" : "MyAssembly.MyCustomFormatter, MyAssembly"
         }
        }
      }
    ]
  }
}
```

## Example application using a `ConsoleLogProvider` with a default template

```c#
using System;
using RockLib.Logging;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = LoggerFactory.Create();
            var dividend = 4;
            var divisor = 0;
            try
            {
                Divide(dividend, divisor);
            }
            catch (Exception ex)
            {
                logger.Error("Error in Divide method.", ex, new { dividend, divisor });
            }
            Console.ReadKey();
        }

        private static int? Divide(int dividend, int divisor)
        {
            return dividend / divisor;
        }
    }
}
```

---

It should output the following to console:

```
----------------------------------------------------------------------------------------------------
LOG INFO

Message: Error in Divide method.
Create Time: 2019-08-07T21:08:52.5975104Z
Level: Error
Log ID: 2e8a06a2-408d-4a65-a794-f7811c020308
User Name: XXXXXXXX
Machine Name: XXXXXXXX
Machine IP Address: XX.XX.XXX.XX
XXXX:XXXX:XXXXX:XXXX::XXX
XXX.XXX.X.XXX

EXTENDED PROPERTY INFO

dividend: 4
divisor: 0

EXCEPTION INFO

Type: System.DivideByZeroException
Message: Attempted to divide by zero.
Properties:
   HResult: 0x80020012
Source: Example.Console.netcoreapp2.0
Stack Trace:
   at Example.Program.Divide(Int32 dividend, Int32 divisor) in C:\Code\GitHub\RockLib.Logging\Example.Console.netcoreapp2.0\Program.cs:line 26
   at Example.Program.Main(String[] args) in C:\Code\GitHub\RockLib.Logging\Example.Console.netcoreapp2.0\Program.cs:line 15
```
