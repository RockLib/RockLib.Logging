using RockLib.Logging;
using System;

namespace Example
{
    partial class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Logger logger = LoggerFactory.GetInstance();

                var service = new ExampleService(logger);

                service.DoSomethingDangerous(123, "abc");
                service.DoSomethingDangerous(42, "What do you mean? African or European swallow?");
            }
            finally
            {
                LoggerFactory.ShutDown();
            }

            Console.WriteLine();
            Console.Write("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
