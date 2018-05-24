using System.IO;
using System.Threading.Tasks;

namespace Example
{
    /// <summary>
    /// This class throws a fairly complex exception in its static constructor, making it ideal for
    /// showing how exception messages are formatted in RockLib.Logging.
    /// </summary>
    internal static class BrokenStaticConstructor
    {
        static BrokenStaticConstructor() => MultipleBadThings().Wait();

        public static void Poke() { }

        private static Task MultipleBadThings()
        {
            var badThings = new[]
            {
                BadThing1(),
                BadThing2(),
                BadThing3()
            };

            return Task.WhenAll(badThings);
        }

        private static async Task BadThing1()
        {
            await Task.Yield();
            string.Format("There is no argument for the replacement token at index 0: {0}");
        }

        private static async Task BadThing2()
        {
            await Task.Yield();
            int i = 1, j = 0, k;
            k = i / j;
        }

        private static async Task BadThing3()
        {
            await Task.Yield();
            File.OpenRead(null);
        }
    }
}
