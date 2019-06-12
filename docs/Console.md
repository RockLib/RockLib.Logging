# Console

In this tutorial, we will be building a console application that does basic console logging.

---

Create a .NET Core 2.0 (or above) console application named "ConsoleLoggingTutorial".

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
        "Type": "RockLib.Logging.ConsoleLogProvider, RockLib.Logging"
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
                _logger.Debug("ConsoleLoggingTutorial application started.");
            }

            Console.WriteLine($"8 / 2 = {Divide(8, 2)}");
            Console.WriteLine($"589 / 19 = {Divide(589, 19)}");
            Console.WriteLine($"4 / 0 = {Divide(4, 0)}");

            if (_logger.IsDebugEnabled())
            {
                _logger.Debug("ConsoleLoggingTutorial application finished.");
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
----------------------------------------------------------------------------------------------------
LOG INFO

Message: Error in Divide method.
Create Time: 2019-06-12T17:10:55.8322480Z
Level: Error
Log ID: c395d164-ce4d-49e2-8eda-a2bc1da9da61
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
Source: ConsoleLoggingTutorial
Stack Trace:
   at ConsoleLoggingTutorial.Program.Divide(Int32 dividend, Int32 divisor) in /Users/SomeUser/Dev/ConsoleLoggingTutorial/Program.cs:line 42
```

---

You can customize the template used for format the log output.  Update 'appsettings.json' with the following updates:

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

Now when the app is run you should see the following in the console: 

```
8 / 2 = 4
589 / 19 = 31
4 / 0 = 
Error: Error in Divide method.
```

---

Note that debug logs were not written because the logger's level is set to "Warn". To enable debug logs, modify the appsettings.json file so the level is "Debug":

```json
{
  "Rocklib.Logging": {
    "Level": "Debug",
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

When run, the console will now contain debug logs in addition to error logs.

```
Debug: ConsoleLogProvider application started.
8 / 2 = 4
589 / 19 = 31
Debug: Divide method called.
Debug: Divide method called.
Debug: Divide method called.
4 / 0 = 
Error: Error in Divide method.
Debug: ConsoleLogProvider application finished.
```
