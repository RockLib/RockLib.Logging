using System;
using Microsoft.Extensions.Logging;
using RockLib.Logging.Extensions;

namespace Example.Extensions
{
    class Program
    {
        static void Main(string[] args)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddRockLib();

            var logger = loggerFactory.CreateLogger<Program>();

            logger.LogDebug($"LogDebug logging via Microsoft.Extension.Logging -- {DateTime.Now.Ticks}");
            logger.LogInformation($"LogInformation logging via Microsoft.Extension.Logging -- {DateTime.Now.Ticks}");
            logger.LogWarning($"LogWarning logging via Microsoft.Extension.Logging -- {DateTime.Now.Ticks}");
            logger.LogError($"LogError logging via Microsoft.Extension.Logging -- {DateTime.Now.Ticks}");
            logger.LogCritical($"LogCritical logging via Microsoft.Extension.Logging -- {DateTime.Now.Ticks}");

            Console.WriteLine();
            Console.Write("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
