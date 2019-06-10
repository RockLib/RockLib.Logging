# File

In this tutorial, we will be building a console application that does basic file logging.

---

Create a .NET Core 2.0 (or above) console application named "FileLoggingTutorial".

---

Add a nuget reference for "RockLib.Logging" to the project.

---

Add a new JSON file to the project named 'appsettings.json'. Set its 'Copy to Output Directory' setting to 'Copy if newer'. Add the following configuration:

```json
{
  "Rocklib.Logging": {
    "Level": "Warn",
    "Providers": [
      {
        "Type": "RockLib.Logging.FileLogProvider, RockLib.Logging",
        "Value": { "File": "log.txt" }
      }
    ]
  }
}
```

---

Edit the `Program.cs` file as follows:

```c#
using System;
using RockLib.Logging;

namespace Example
{
    class Program
    {
        private static ILogger _logger = LoggerFactory.Create();

        static void Main(string[] args)
        {
            if (_logger.IsDebugEnabled())
            {
                _logger.Debug("FileLoggingTutorial application started.");
            }

            Console.WriteLine($"8 / 2 = {Divide(8, 2)}");
            Console.WriteLine($"589 / 19 = {Divide(589, 19)}");
            Console.WriteLine($"4 / 0 = {Divide(4, 0)}");

            if (_logger.IsDebugEnabled())
            {
                _logger.Debug("FileLoggingTutorial application finished.");
            }


            _logger.Dispose();

            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }

        private static int? Divide(int dividend, int divisor)
        {
            if (_logger.IsDebugEnabled())
            {
                _logger.Debug("Divide method called.", new { dividend, divisor });
            }

            try
            {
                return dividend / divisor;
            }
            catch (Exception ex)
            {
                _logger.Error("Error in Divide method.", ex, new { dividend, divisor });
                return null;
            }
        }
    }
}
```

---

Start the app. It should output the following to console:

```
8 / 2 = 4
589 / 19 = 31
4 / 0 =
```

---

Open the log file `log.txt` located in the output directory of the application. Its contents should look similar to this:

```
----------------------------------------------------------------------------------------------------
LOG INFO

Message: Error in Divide method.
Create Time: 2019-04-11T15:45:36.7290529Z
Level: Error
Log ID: 63f5cc4e-fe35-4444-b9d3-697d87353972
User Name: SomeUser
Machine Name: MyComputer
Machine IP Address: 192.168.1.1

EXTENDED PROPERTY INFO

dividend: 4
divisor: 0

EXCEPTION INFO

Type: System.DivideByZeroException
Message: Attempted to divide by zero.
Properties:
   HResult: 0x80020012
Source: FileLoggingTutorial
Stack Trace:
   at FileLoggingTutorial.Program.Divide(Int32 dividend, Int32 divisor) in /Users/SomeUser/Dev/FileLoggingTutorial/Program.cs:line 42
```

---

Note that debug logs were not written because the logger's level is set to "Warn". To enable debug logs, modify the appsettings.json file so the level is "Debug":

```json
{
  "Rocklib.Logging": {
    "Level": "Debug",
    "Providers": [
      {
        "Type": "RockLib.Logging.FileLogProvider, RockLib.Logging",
        "Value": { "File": "log.txt" }
      }
    ]
  }
}
```

When run, the console output of the application is the same, but the log file contains debug logs in addition to error logs.