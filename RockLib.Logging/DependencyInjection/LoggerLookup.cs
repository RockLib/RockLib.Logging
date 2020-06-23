#if !NET451
namespace RockLib.Logging.DependencyInjection
{
    /// <summary>
    /// Represents a method that retrieves an <see cref="ILogger"/> by its name.
    /// </summary>
    /// <param name="name">The name of the <see cref="ILogger"/> to retrieve.</param>
    /// <returns>The matching <see cref="ILogger"/>.</returns>
    public delegate ILogger LoggerLookup(string name);
}
#endif
