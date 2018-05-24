using RockLib.Logging;
using System;

namespace Example
{
    public class ExampleService
    {
        public ExampleService(ILogger logger)
        {
            Logger = logger;
        }

        public ILogger Logger { get; }

        public void DoSomethingDangerous(int foo, string bar)
        {
            try
            {
                if (foo != 42 && bar != "What do you mean? African or European swallow?")
                {
                    BrokenStaticConstructor.Poke();
                }

                Logger.Info("Made it through the dangerous part.", new { foo, bar });
            }
            catch (Exception ex)
            {
                Logger.Error("Got caught by the dangerous part.", ex, new { foo, bar });
            }
        }
    }
}
